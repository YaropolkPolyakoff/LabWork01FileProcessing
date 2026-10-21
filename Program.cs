using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;
using ZooTicketSystem.Services;
using ZooTicketSystem.Utils;

namespace ZooTicketSystem
{
    class Program
    {
        // Сохраняем аргумент для списка животных
        private static string animalListArgument = "";
        private static ConsoleUI consoleUI = new ConsoleUI();
        private static InputParser inputParser = new InputParser();
        private static InputReader inputReader = new InputReader(new InputParser());
        
        static void Main(string[] inputArguments)
        {
            // Установка кодировки консоли для правильного отображения кириллицы
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.InputEncoding = System.Text.Encoding.UTF8;
            }
            catch 
            { 
                /* Игнорируем ошибки установки кодировки на некоторых системах */ 
            }
            
            // Читаем аргумент для выбора списка животных
            if (inputArguments != null && inputArguments.Length > 0)
            {
                animalListArgument = inputArguments[0];
            }
            
            consoleUI.DisplayStartupHint(animalListArgument);
            consoleUI.DisplayAppHeader();

            try
            {
                ExecuteInteractiveZooBookingSystem();
                consoleUI.DisplayWorkCompleted();
            }
            catch (FileNotFoundException errorInfo)
            {
                DisplayFileError(errorInfo);
            }
            catch (ArgumentException errorInfo)
            {
                DisplayValidationError(errorInfo);
            }
            catch (InvalidOperationException errorInfo)
            {
                DisplayBusinessError(errorInfo);
            }
            catch (Exception errorInfo)
            {
                consoleUI.DisplayCriticalError(errorInfo.GetType().Name, errorInfo.Message);
            }

            consoleUI.DisplayPauseBeforeExit();
        }

        static void ExecuteInteractiveZooBookingSystem()
        {
            ZooService mainService = new ZooService();

            SetupZoo(mainService);
            Customer buyer = GetCustomerFromConsole();
            List<Ticket> orderedTickets = SelectTicketsInteractively(mainService);
            
            if (orderedTickets.Count == 0)
            {
                consoleUI.DisplayNoTicketsSelected();
                return;
            }
            
            Purchase transaction = ExecuteTransaction(mainService, buyer, orderedTickets);
            
            if (!ConfirmPayment(transaction))
            {
                consoleUI.DisplayPaymentCancelled();
                return;
            }
            
            mainService.ConfirmPurchase(transaction);
            consoleUI.DisplayTransactionSuccess();
            OutputResults(mainService, transaction);
        }

        static void SetupZoo(ZooService svc)
        {
            consoleUI.DisplayStepHeader("Шаг 1: Инициализация зоопарка");
            
            // Загружаем животных по простому аргументу (не требуем путь к файлу)
            List<Animal> animalList = FileHelper.LoadAnimalsByType(animalListArgument);
            
            Zoo facility = svc.GetZoo();
            
            foreach (Animal animal in animalList)
            {
                facility.AddAnimal(animal);
            }
            
            consoleUI.DisplayAnimalsLoaded(animalList.Count);
            
            if (animalList.Count > 0)
            {
                consoleUI.DisplayAnimalsInfo(svc.GetAnimalsInfo());
            }
        }

        static Customer GetCustomerFromConsole()
        {
            consoleUI.DisplayStepHeader("Шаг 2: Загрузка данных клиента");
            
            string name = inputReader.ReadNonEmptyLine(
                "Введите ваше имя: ",
                "Имя не может быть пустым. Введите ваше имя: ");
            
            int age = inputReader.ReadPositiveInteger(
                "Введите ваш возраст: ",
                "Введите корректный возраст (положительное число): ");
            
            string email = inputReader.ReadEmail(
                "Введите ваш email: ",
                "Введите корректный email: ");
            
            string phone = inputReader.ReadOptionalLine(
                "Введите ваш телефон (или нажмите Enter для пропуска): ");
            
            Customer person = new Customer(name, age, email, phone);
            string category;
            if (person.Age < 12)
                category = "Ребенок";
            else if (person.Age <= 18)
                category = "Студент";
            else
                category = "Взрослый";
            consoleUI.DisplayCustomerInfo(person.Name, person.Age, category);
            return person;
        }

        static List<Ticket> SelectTicketsInteractively(ZooService zooService)
        {
            consoleUI.DisplayStepHeader("Шаг 3: Формирование билетов");
            consoleUI.DisplayTicketOptions();
            
            List<Ticket> resultTickets = new List<Ticket>();
            
            while (true)
            {
                string input = inputReader.ReadOptionalLine(
                    "\nВыберите тип билета (1-4) или 0 для завершения: ");
                
                if (input == "0")
                {
                    break;
                }
                
                if (inputParser.TryParseTicketTypeChoice(input, out TicketType selectedType))
                {
                    Ticket ticket = zooService.CreateTicket(selectedType);
                    resultTickets.Add(ticket);
                    consoleUI.DisplayTicketAdded(ticket.Type, ticket.BasePrice);
                }
                else
                {
                    consoleUI.DisplayInvalidTicketChoice();
                }
            }
            
            consoleUI.DisplayTicketsCount(resultTickets.Count);
            return resultTickets;
        }

        static bool ConfirmPayment(Purchase order)
        {
            consoleUI.DisplayOrderSummary(order);
            
            string confirmation = inputReader.ReadOptionalLine(
                "\nПодтвердите оплату (да/нет): ");
            
            return inputParser.ParseYesNoConfirmation(confirmation);
        }

        static Purchase ExecuteTransaction(ZooService svc, Customer buyer, List<Ticket> items)
        {
            Purchase order = svc.CreatePurchase(buyer, items, DateTime.Now);
            consoleUI.DisplayTransactionHeader(order.ConfirmationNumber);
            return order;
        }

        static void OutputResults(ZooService svc, Purchase order)
        {
            consoleUI.DisplayResultsHeader();
            string document = svc.GeneratePurchaseConfirmation(order);
            consoleUI.DisplayDocument(document);
            
            string response = inputReader.ReadOptionalLine(
                "\nСохранить подтверждение в файл? (да/нет): ");
            
            if (inputParser.ParseYesNoConfirmation(response))
            {
                string filename = inputReader.ReadOptionalLine(
                    "Введите имя файла (или нажмите Enter для 'purchase_confirmation.txt'): ");
                
                if (string.IsNullOrEmpty(filename))
                {
                    filename = "purchase_confirmation.txt";
                }
                
                try
                {
                    FileHelper.WriteOutputToFile(filename, document);
                    consoleUI.DisplayFileSaved(filename);
                }
                catch (Exception ex)
                {
                    consoleUI.DisplayFileSaveError(ex.Message);
                }
            }
        }

        static void DisplayFileError(FileNotFoundException e) 
        { 
            consoleUI.DisplayFileError(e.Message);
        }
        
        static void DisplayValidationError(ArgumentException e) 
        { 
            consoleUI.DisplayValidationError(e.Message);
        }
        
        static void DisplayBusinessError(InvalidOperationException e) 
        { 
            consoleUI.DisplayBusinessError(e.Message);
        }
    }
}

