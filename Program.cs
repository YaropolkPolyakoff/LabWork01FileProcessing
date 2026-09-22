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
            
            WriteHeader();

            try
            {
                ExecuteInteractiveZooBookingSystem();
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

        static void ExecuteInteractiveZooBookingSystem()
        {
            ZooService mainService = new ZooService();
            ValidationService validationEngine = new ValidationService();

            SetupZoo(mainService);
            Customer buyer = GetCustomerFromConsole();
            List<Ticket> orderedTickets = SelectTicketsInteractively(validationEngine);
            
            if (orderedTickets.Count == 0)
            {
                Console.WriteLine("Не выбрано ни одного билета. Завершение работы.");
                return;
            }
            
            Purchase transaction = ExecuteTransaction(mainService, buyer, orderedTickets);
            
            if (!ConfirmPayment(transaction))
            {
                Console.WriteLine("Оплата отменена пользователем.");
                return;
            }
            
            mainService.ConfirmPurchase(transaction);
            Console.WriteLine("Транзакция завершена успешно!");
            OutputResults(mainService, transaction);
        }

        static void SetupZoo(ZooService svc)
        {
            Console.WriteLine(">>> Шаг 1: Инициализация зоопарка");
            
            Console.WriteLine("Хотите загрузить животных из файла? (да/нет):");
            string response = Console.ReadLine()?.Trim().ToLower();
            
            List<Animal> animalList = new List<Animal>();
            
            if (response == "да" || response == "yes" || response == "y")
            {
                Console.Write("Введите путь к файлу с животными: ");
                string filepath = Console.ReadLine()?.Trim();
                
                if (!string.IsNullOrEmpty(filepath))
                {
                    try
                    {
                        animalList = FileHelper.ReadAnimalsFromFile(filepath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при загрузке: " + ex.Message);
                        Console.WriteLine("Продолжаем без животных.");
                    }
                }
            }
            else
            {
                // Добавляем несколько животных по умолчанию
                animalList.Add(new Animal("Лев Симба", "Лев африканский", "Саванна", "Царь зверей", 5));
                animalList.Add(new Animal("Слон Дамбо", "Слон индийский", "Тропический лес", "Умный гигант", 12));
                animalList.Add(new Animal("Пингвин Коля", "Пингвин императорский", "Антарктида", "Смешной птиц", 3));
                Console.WriteLine("Загружены животные по умолчанию.");
            }
            
            Zoo facility = svc.GetZoo();
            
            for (int counter = 0; counter < animalList.Count; counter++)
            {
                facility.AddAnimal(animalList[counter]);
            }
            
            Console.WriteLine("Добавлено существ: " + animalList.Count);
            
            if (animalList.Count > 0)
            {
                Console.WriteLine(svc.GetAnimalsInfo());
            }
        }

        static Customer GetCustomerFromConsole()
        {
            Console.WriteLine(">>> Шаг 2: Загрузка данных клиента");
            
            Console.Write("Введите ваше имя: ");
            string name = Console.ReadLine()?.Trim();
            
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Write("Имя не может быть пустым. Введите ваше имя: ");
                name = Console.ReadLine()?.Trim();
            }
            
            Console.Write("Введите ваш возраст: ");
            int age = 0;
            while (age <= 0)
            {
                string ageInput = Console.ReadLine()?.Trim();
                if (!int.TryParse(ageInput, out age) || age <= 0)
                {
                    Console.Write("Введите корректный возраст (положительное число): ");
                }
            }
            
            Console.Write("Введите ваш email: ");
            string email = Console.ReadLine()?.Trim();
            
            while (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                Console.Write("Введите корректный email: ");
                email = Console.ReadLine()?.Trim();
            }
            
            Console.Write("Введите ваш телефон (или нажмите Enter для пропуска): ");
            string phone = Console.ReadLine()?.Trim();
            
            Customer person = new Customer(name, age, email, phone);
            Console.WriteLine("\nКлиент: " + person.Name + " [" + person.Age + " лет] - " + person.GetCategory());
            return person;
        }

        static List<Ticket> SelectTicketsInteractively(ValidationService validator)
        {
            Console.WriteLine(">>> Шаг 3: Формирование билетов");
            
            Console.WriteLine("\nДоступные типы билетов:");
            Console.WriteLine("1. Adult - Взрослый билет (500 руб.)");
            Console.WriteLine("2. Child - Детский билет (250 руб.)");
            Console.WriteLine("3. Student - Студенческий билет (350 руб.)");
            Console.WriteLine("4. Family - Семейный билет (1500 руб.)");
            Console.WriteLine("0. Завершить выбор");
            
            List<Ticket> resultTickets = new List<Ticket>();
            DateTime timeNow = DateTime.Now;
            DateTime expiryDate = timeNow.AddDays(7);
            
            bool continueSelection = true;
            
            while (continueSelection)
            {
                Console.Write("\nВыберите тип билета (1-4) или 0 для завершения: ");
                string input = Console.ReadLine()?.Trim();
                
                if (input == "0")
                {
                    continueSelection = false;
                    continue;
                }
                
                TicketType category;
                
                if (input == "1")
                {
                    category = TicketType.Adult;
                }
                else if (input == "2")
                {
                    category = TicketType.Child;
                }
                else if (input == "3")
                {
                    category = TicketType.Student;
                }
                else if (input == "4")
                {
                    category = TicketType.Family;
                }
                else
                {
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    continue;
                }
                
                decimal cost = Ticket.GetBasePriceByType(category);
                resultTickets.Add(new Ticket(category, cost, timeNow, expiryDate));
                Console.WriteLine("Добавлен билет: " + category + " - " + cost + " руб.");
            }
            
            Console.WriteLine("Оформлено билетов: " + resultTickets.Count);
            return resultTickets;
        }

        static bool ConfirmPayment(Purchase order)
        {
            Console.WriteLine("\n>>> Шаг 4: Подтверждение оплаты");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Покупатель: " + order.Customer.Name);
            Console.WriteLine("Количество билетов: " + order.Tickets.Count);
            
            Console.WriteLine("\nСостав заказа:");
            for (int i = 0; i < order.Tickets.Count; i++)
            {
                Ticket ticket = order.Tickets[i];
                Console.WriteLine("  " + (i + 1) + ". " + ticket.Type + " - " + ticket.BasePrice + " руб.");
            }
            
            Console.WriteLine("\nИТОГО К ОПЛАТЕ: " + order.TotalAmount + " руб.");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            Console.Write("\nПодтвердите оплату (да/нет): ");
            string confirmation = Console.ReadLine()?.Trim().ToLower();
            
            return (confirmation == "да" || confirmation == "yes" || confirmation == "y");
        }

        static Purchase ExecuteTransaction(ZooService svc, Customer buyer, List<Ticket> items)
        {
            Console.WriteLine("\n>>> Расчет стоимости");
            Purchase order = svc.CreatePurchase(buyer, items, DateTime.Now);
            Console.WriteLine("ID транзакции: " + order.ConfirmationNumber);
            return order;
        }

        static void OutputResults(ZooService svc, Purchase order)
        {
            Console.WriteLine(">>> Шаг 5: Генерация документа");
            string document = svc.GeneratePurchaseConfirmation(order);
            Console.WriteLine(document);
            
            Console.Write("\nСохранить подтверждение в файл? (да/нет): ");
            string response = Console.ReadLine()?.Trim().ToLower();
            
            if (response == "да" || response == "yes" || response == "y")
            {
                Console.Write("Введите имя файла (или нажмите Enter для 'purchase_confirmation.txt'): ");
                string filename = Console.ReadLine()?.Trim();
                
                if (string.IsNullOrEmpty(filename))
                {
                    filename = "purchase_confirmation.txt";
                }
                
                try
                {
                    FileHelper.WriteOutputToFile(filename, document);
                    Console.WriteLine("Документ сохранен: " + filename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при сохранении файла: " + ex.Message);
                }
            }
        }

        static void WriteFooter() 
        { 
            Console.WriteLine("\n*** Работа завершена ***\n"); 
        }
        
        static void DisplayFileError(FileNotFoundException e) 
        { 
            Console.WriteLine("\n!!! Файл отсутствует: " + e.Message); 
        }
        
        static void DisplayValidationError(ArgumentException e) 
        { 
            Console.WriteLine("\n!!! Ошибка валидации: " + e.Message); 
        }
        
        static void DisplayBusinessError(InvalidOperationException e) 
        { 
            Console.WriteLine("\n!!! Бизнес-ошибка: " + e.Message); 
        }
        
        static void DisplayCriticalError(Exception e) 
        { 
            Console.WriteLine("\n!!! Критическая ошибка [" + e.GetType().Name + "]: " + e.Message); 
        }
        
        static void PauseBeforeExit() 
        { 
            Console.WriteLine("\nДля выхода нажмите любую клавишу"); 
            Console.ReadKey(); 
        }
    }
}

