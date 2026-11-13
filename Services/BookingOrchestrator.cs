using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;

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
        private readonly IUserInterface _userInterface;
        private readonly IReportGenerator _reportGenerator;

        public BookingOrchestrator(
            string animalListArgument,
            IZooService zooService,
            IUserInterface userInterface,
            IReportGenerator reportGenerator)
        {
            _animalListArgument = animalListArgument;
            _zooService = zooService ?? throw new ArgumentNullException(nameof(zooService));
            _userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
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
                _userInterface.DisplayNoTicketsSelected();
                return;
            }

            Purchase transaction = ExecuteTransaction(_zooService, buyer, orderedTickets);

            if (!ConfirmPayment(transaction))
            {
                _userInterface.DisplayPaymentCancelled();
                return;
            }

            _zooService.ConfirmPurchase(transaction);
            _userInterface.DisplayTransactionSuccess();
            OutputResults(_zooService, transaction);
        }

        private void SetupZoo(IZooService svc)
        {
            _userInterface.DisplayZooSetupHeader();
            List<Animal> animalList = FileHelper.LoadAnimalsByType(_animalListArgument);

            Zoo facility = svc.GetZoo();
            foreach (Animal animal in animalList)
            {
                facility.AddAnimal(animal);
            }

            _userInterface.DisplayAnimalsAdded(animalList.Count);

            if (animalList.Count > 0)
            {
                _userInterface.DisplayAnimalsInfo(svc.GetAnimalsInfo());
            }
        }

        private Customer GetCustomerFromConsole()
        {
            _userInterface.DisplayCustomerHeader();
            string name = _userInterface.ReadNonEmptyLine(
                "Введите ваше имя: ",
                "Имя не может быть пустым. Введите ваше имя: ");

            int age = _userInterface.ReadPositiveInteger(
                "Введите ваш возраст: ",
                "Введите корректный возраст (положительное число): ");

            string email = _userInterface.ReadEmail(
                "Введите ваш email: ",
                "Введите корректный email: ");

            string phone = _userInterface.ReadOptionalLine(
                "Введите ваш телефон (или нажмите Enter для пропуска): ");

            Customer person = new Customer(name, age, email, phone);
            string category = _userInterface.GetCustomerCategory(person);
            _userInterface.DisplayCustomerCategory(person, category);
            return person;
        }

        private List<Ticket> SelectTicketsInteractively(IZooService zooService)
        {
            _userInterface.DisplayStepHeader("Шаг 3: Формирование билетов");
            _userInterface.DisplayTicketOptions();

            List<Ticket> resultTickets = new List<Ticket>();

            while (true)
            {
                string input = _userInterface.ReadOptionalLine(
                    "\nВыберите тип билета (1-4) или 0 для завершения: ");

                if (input == "0")
                {
                    break;
                }

                if (_userInterface.TryReadTicketTypeChoice(input, out TicketType selectedType))
                {
                    Ticket ticket = zooService.CreateTicket(selectedType.Value);
                    resultTickets.Add(ticket);
                    _userInterface.DisplayTicketAdded(ticket.Type, ticket.BasePrice);
                }
                else
                {
                    _userInterface.DisplayInvalidTicketChoice();
                }
            }

            _userInterface.DisplayTicketsCount(resultTickets.Count);
            return resultTickets;
        }

        private Purchase ExecuteTransaction(IZooService svc, Customer buyer, List<Ticket> items)
        {
            _userInterface.DisplayTransactionHeader();
            Purchase order = svc.CreatePurchase(buyer, items, DateTime.Now);
            _userInterface.DisplayTransactionId(order.ConfirmationNumber);
            return order;
        }

        private bool ConfirmPayment(Purchase order)
        {
            _userInterface.DisplayOrderSummary(order);
            return _userInterface.ReadYesNoConfirmation("\nПодтвердите оплату (да/нет): ");
        }

        private void OutputResults(IZooService svc, Purchase order)
        {
            _userInterface.DisplayOutputHeader();
            string document = _reportGenerator.GeneratePurchaseConfirmation(order);
            _userInterface.DisplayDocument(document);

            if (_userInterface.ReadYesNoConfirmation("\nСохранить подтверждение в файл? (да/нет): "))
            {
                string filename = _userInterface.ReadOptionalLine(
                    "Введите имя файла (или нажмите Enter для 'purchase_confirmation.txt'): ");

                if (string.IsNullOrEmpty(filename))
                {
                    filename = "purchase_confirmation.txt";
                }

                try
                {
                    FileHelper.WriteOutputToFile(filename, document);
                    _userInterface.DisplayFileSaved(filename);
                }
                catch (Exception ex)
                {
                    _userInterface.DisplayFileSaveError(ex.Message);
                }
            }
        }
    }
}
