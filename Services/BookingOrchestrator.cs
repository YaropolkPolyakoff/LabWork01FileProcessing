using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;
using ZooTicketSystem.Utils;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Координирует шаги бизнес-сценария бронирования билетов в зоопарк.
    /// Единственная ответственность: управление последовательностью шагов бронирования.
    /// </summary>
    public class BookingOrchestrator
    {
        private readonly string _animalListArgument;
        private readonly ConsoleUI _consoleUI;
        private readonly InputParser _inputParser;
        private readonly InputReader _inputReader;

        public BookingOrchestrator(
            string animalListArgument,
            ConsoleUI consoleUI,
            InputParser inputParser,
            InputReader inputReader)
        {
            _animalListArgument = animalListArgument;
            _consoleUI = consoleUI;
            _inputParser = inputParser;
            _inputReader = inputReader;
        }

        /// <summary>
        /// Запускает полный сценарий бронирования билетов.
        /// </summary>
        public void Run()
        {
            ZooService mainService = new ZooService();

            SetupZoo(mainService);
            Customer buyer = GetCustomerFromConsole();
            List<Ticket> orderedTickets = SelectTicketsInteractively(mainService);

            if (orderedTickets.Count == 0)
            {
                _consoleUI.DisplayNoTicketsSelected();
                return;
            }

            Purchase transaction = ExecuteTransaction(mainService, buyer, orderedTickets);

            if (!ConfirmPayment(transaction))
            {
                _consoleUI.DisplayPaymentCancelled();
                return;
            }

            mainService.ConfirmPurchase(transaction);
            _consoleUI.DisplayTransactionSuccess();
            OutputResults(mainService, transaction);
        }

        private void SetupZoo(ZooService svc)
        {
            _consoleUI.DisplayZooSetupHeader();
            List<Animal> animalList = FileHelper.LoadAnimalsByType(_animalListArgument);

            Zoo facility = svc.GetZoo();
            foreach (Animal animal in animalList)
            {
                facility.AddAnimal(animal);
            }

            _consoleUI.DisplayAnimalsAdded(animalList.Count);

            if (animalList.Count > 0)
            {
                _consoleUI.DisplayAnimalsInfo(svc.GetAnimalsInfo());
            }
        }

        private Customer GetCustomerFromConsole()
        {
            _consoleUI.DisplayCustomerHeader();
            string name = _inputReader.ReadNonEmptyLine(
                "Введите ваше имя: ",
                "Имя не может быть пустым. Введите ваше имя: ");

            int age = _inputReader.ReadPositiveInteger(
                "Введите ваш возраст: ",
                "Введите корректный возраст (положительное число): ");

            string email = _inputReader.ReadEmail(
                "Введите ваш email: ",
                "Введите корректный email: ");

            string phone = _inputReader.ReadOptionalLine(
                "Введите ваш телефон (или нажмите Enter для пропуска): ");

            Customer person = new Customer(name, age, email, phone);
            string category = _consoleUI.GetCustomerCategory(person);
            _consoleUI.DisplayCustomerCategory(person, category);
            return person;
        }

        private List<Ticket> SelectTicketsInteractively(ZooService zooService)
        {
            _consoleUI.DisplayStepHeader("Шаг 3: Формирование билетов");
            _consoleUI.DisplayTicketOptions();

            List<Ticket> resultTickets = new List<Ticket>();

            while (true)
            {
                string input = _inputReader.ReadOptionalLine(
                    "\nВыберите тип билета (1-4) или 0 для завершения: ");

                if (input == "0")
                {
                    break;
                }

                if (_inputParser.TryParseTicketTypeChoice(input, out TicketType selectedType))
                {
                    Ticket ticket = zooService.CreateTicket(selectedType);
                    resultTickets.Add(ticket);
                    _consoleUI.DisplayTicketAdded(ticket.Type, ticket.BasePrice);
                }
                else
                {
                    _consoleUI.DisplayInvalidTicketChoice();
                }
            }

            _consoleUI.DisplayTicketsCount(resultTickets.Count);
            return resultTickets;
        }

        private Purchase ExecuteTransaction(ZooService svc, Customer buyer, List<Ticket> items)
        {
            _consoleUI.DisplayTransactionHeader();
            Purchase order = svc.CreatePurchase(buyer, items, DateTime.Now);
            _consoleUI.DisplayTransactionId(order.ConfirmationNumber);
            return order;
        }

        private bool ConfirmPayment(Purchase order)
        {
            _consoleUI.DisplayOrderSummary(order);

            string confirmation = _inputReader.ReadOptionalLine(
                "\nПодтвердите оплату (да/нет): ");

            return _inputParser.ParseYesNoConfirmation(confirmation);
        }

        private void OutputResults(ZooService svc, Purchase order)
        {
            _consoleUI.DisplayOutputHeader();
            string document = svc.GeneratePurchaseConfirmation(order);
            _consoleUI.DisplayDocument(document);

            string response = _inputReader.ReadOptionalLine(
                "\nСохранить подтверждение в файл? (да/нет): ");

            if (_inputParser.ParseYesNoConfirmation(response))
            {
                string filename = _inputReader.ReadOptionalLine(
                    "Введите имя файла (или нажмите Enter для 'purchase_confirmation.txt'): ");

                if (string.IsNullOrEmpty(filename))
                {
                    filename = "purchase_confirmation.txt";
                }

                try
                {
                    FileHelper.WriteOutputToFile(filename, document);
                    _consoleUI.DisplayFileSaved(filename);
                }
                catch (Exception ex)
                {
                    _consoleUI.DisplayFileSaveError(ex.Message);
                }
            }
        }
    }
}
