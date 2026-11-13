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
        private readonly IZooService _zooService;
        private readonly ConsoleUI _consoleUI;
        private readonly IReportGenerator _reportGenerator;

        public BookingOrchestrator(
            string animalListArgument,
            IZooService zooService,
            ConsoleUI consoleUI,
            IReportGenerator reportGenerator)
        {
            _animalListArgument = animalListArgument;
            _zooService = zooService ?? throw new ArgumentNullException(nameof(zooService));
            _consoleUI = consoleUI;
            _reportGenerator = reportGenerator ?? throw new ArgumentNullException(nameof(reportGenerator));
        }

        /// <summary>
        /// Запускает полный сценарий бронирования билетов.
        /// </summary>
        public void Run()
        {
            SetupZoo(_zooService);
            Customer buyer = GetCustomerFromConsole();
            List<Ticket> orderedTickets = SelectTicketsInteractively(_zooService);

            if (orderedTickets.Count == 0)
            {
                _consoleUI.DisplayNoTicketsSelected();
                return;
            }

            Purchase transaction = ExecuteTransaction(_zooService, buyer, orderedTickets);

            if (!ConfirmPayment(transaction))
            {
                _consoleUI.DisplayPaymentCancelled();
                return;
            }

            _zooService.ConfirmPurchase(transaction);
            _consoleUI.DisplayTransactionSuccess();
            OutputResults(_zooService, transaction);
        }

        private void SetupZoo(IZooService svc)
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
            string name = _consoleUI.ReadNonEmptyLine(
                "Введите ваше имя: ",
                "Имя не может быть пустым. Введите ваше имя: ");

            int age = _consoleUI.ReadPositiveInteger(
                "Введите ваш возраст: ",
                "Введите корректный возраст (положительное число): ");

            string email = _consoleUI.ReadEmail(
                "Введите ваш email: ",
                "Введите корректный email: ");

            string phone = _consoleUI.ReadOptionalLine(
                "Введите ваш телефон (или нажмите Enter для пропуска): ");

            Customer person = new Customer(name, age, email, phone);
            string category = _consoleUI.GetCustomerCategory(person);
            _consoleUI.DisplayCustomerCategory(person, category);
            return person;
        }

        private List<Ticket> SelectTicketsInteractively(IZooService zooService)
        {
            _consoleUI.DisplayStepHeader("Шаг 3: Формирование билетов");
            _consoleUI.DisplayTicketOptions();

            List<Ticket> resultTickets = new List<Ticket>();

            while (true)
            {
                string input = _consoleUI.ReadOptionalLine(
                    $"\nВыберите тип билета (1-{Enum.GetValues<TicketType>().Length}) или 0 для завершения: ");

                if (input == "0")
                {
                    break;
                }

                TicketType? selectedType = _consoleUI.ParseTicketTypeChoice(input);
                if (selectedType.HasValue)
                {
                    Ticket ticket = zooService.CreateTicket(selectedType.Value);
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

        private Purchase ExecuteTransaction(IZooService svc, Customer buyer, List<Ticket> items)
        {
            _consoleUI.DisplayTransactionHeader();
            Purchase order = svc.CreatePurchase(buyer, items, DateTime.Now);
            _consoleUI.DisplayTransactionId(order.ConfirmationNumber);
            return order;
        }

        private bool ConfirmPayment(Purchase order)
        {
            _consoleUI.DisplayOrderSummary(order);
            return _consoleUI.ReadYesNoConfirmation("\nПодтвердите оплату (да/нет): ");
        }

        private void OutputResults(IZooService svc, Purchase order)
        {
            _consoleUI.DisplayOutputHeader();
            string document = _reportGenerator.GeneratePurchaseConfirmation(order);
            _consoleUI.DisplayDocument(document);

            if (_consoleUI.ReadYesNoConfirmation("\nСохранить подтверждение в файл? (да/нет): "))
            {
                string filename = _consoleUI.ReadOptionalLine(
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
