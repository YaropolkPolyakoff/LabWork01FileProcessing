using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;
using ZooTicketSystem.Services;
using ZooTicketSystem.Utils;

namespace ZooTicketSystem
{
    class Program
    {
        static void Main(string[] inputArguments)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            WriteHeader();

            try
            {
                CheckInputArguments(inputArguments);
                ExecuteZooBookingSystem(inputArguments[0], inputArguments[1], inputArguments[2]);
                WriteFooter();
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
                DisplayCriticalError(errorInfo);
            }

            PauseBeforeExit();
        }

        static void WriteHeader()
        {
            Console.WriteLine("###############################################");
            Console.WriteLine("#  Zoo Ticket Booking System v1.0           #");
            Console.WriteLine("###############################################");
            Console.WriteLine();
        }

        static void CheckInputArguments(string[] arguments)
        {
            if (arguments.Length != 3)
            {
                Console.WriteLine("Требуется 3 аргумента");
                Console.WriteLine("Формат: программа <файл_животных> <файл_клиента> <файл_заказа>");
                throw new ArgumentException("Неверное число параметров");
            }
        }

        static void ExecuteZooBookingSystem(string animalsPath, string customerPath, string orderPath)
        {
            ZooService mainService = new ZooService();
            ValidationService validationEngine = new ValidationService();

            SetupZoo(mainService, animalsPath);
            Customer buyer = GetBuyer(customerPath);
            List<Ticket> orderedTickets = CreateTickets(validationEngine, orderPath);
            Purchase transaction = ExecuteTransaction(mainService, buyer, orderedTickets);
            OutputResults(mainService, transaction);
        }

        static void SetupZoo(ZooService svc, string filepath)
        {
            Console.WriteLine(">>> Шаг 1: Инициализация зоопарка");
            List<Animal> animalList = FileHelper.ReadAnimalsFromFile(filepath);
            Zoo facility = svc.GetZoo();
            
            for (int counter = 0; counter < animalList.Count; counter++)
                facility.AddAnimal(animalList[counter]);
            
            Console.WriteLine("Добавлено существ: " + animalList.Count);
            Console.WriteLine(svc.GetAnimalsInfo());
        }

        static Customer GetBuyer(string filepath)
        {
            Console.WriteLine(">>> Шаг 2: Загрузка данных клиента");
            Customer person = FileHelper.ReadCustomerFromFile(filepath);
            Console.WriteLine("Клиент: " + person.Name + " [" + person.Age + " лет] - " + person.GetCategory());
            return person;
        }

        static List<Ticket> CreateTickets(ValidationService validator, string filepath)
        {
            Console.WriteLine(">>> Шаг 3: Формирование билетов");
            List<string> requestedTypes = FileHelper.ReadPurchaseRequestFromFile(filepath);
            List<Ticket> resultTickets = new List<Ticket>();
            DateTime timeNow = DateTime.Now;
            DateTime expiryDate = timeNow.AddDays(7);
            
            for (int position = 0; position < requestedTypes.Count; position++)
            {
                TicketType category = validator.ParseTicketType(requestedTypes[position]);
                decimal cost = Ticket.GetBasePriceByType(category);
                resultTickets.Add(new Ticket(category, cost, timeNow, expiryDate));
            }
            
            Console.WriteLine("Оформлено билетов: " + resultTickets.Count);
            return resultTickets;
        }

        static Purchase ExecuteTransaction(ZooService svc, Customer buyer, List<Ticket> items)
        {
            Console.WriteLine(">>> Шаг 4: Обработка транзакции");
            Purchase order = svc.CreatePurchase(buyer, items, DateTime.Now);
            Console.WriteLine("ID транзакции: " + order.ConfirmationNumber);
            Console.WriteLine("Итого к оплате: " + order.TotalAmount + " руб.");
            svc.ConfirmPurchase(order);
            Console.WriteLine("Транзакция завершена");
            return order;
        }

        static void OutputResults(ZooService svc, Purchase order)
        {
            Console.WriteLine(">>> Шаг 5: Генерация документа");
            string document = svc.GeneratePurchaseConfirmation(order);
            Console.WriteLine(document);
            FileHelper.WriteOutputToFile("purchase_confirmation.txt", document);
            Console.WriteLine("Документ сохранен: purchase_confirmation.txt");
        }

        static void WriteFooter() { Console.WriteLine("\n*** Работа завершена ***\n"); }
        static void DisplayFileError(FileNotFoundException e) { Console.WriteLine("\n!!! Файл отсутствует: " + e.Message); }
        static void DisplayValidationError(ArgumentException e) { Console.WriteLine("\n!!! Ошибка валидации: " + e.Message); }
        static void DisplayBusinessError(InvalidOperationException e) { Console.WriteLine("\n!!! Бизнес-ошибка: " + e.Message); }
        static void DisplayCriticalError(Exception e) { Console.WriteLine("\n!!! Критическая ошибка [" + e.GetType().Name + "]: " + e.Message); }
        static void PauseBeforeExit() { Console.WriteLine("\nДля выхода нажмите любую клавишу"); Console.ReadKey(); }
    }
}

