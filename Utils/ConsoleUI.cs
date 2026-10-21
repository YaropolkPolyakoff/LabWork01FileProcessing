using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Utils
{
    /// <summary>
    /// Отвечает за вывод информации в консоль
    /// </summary>
    public class ConsoleUI
    {
        public void DisplayStepHeader(string stepTitle)
        {
            Console.WriteLine($">>> {stepTitle}");
            Console.WriteLine();
        }

        public void DisplayTicketOptions()
        {
            Console.WriteLine("\nДоступные типы билетов:");
            Console.WriteLine("1. Adult - Взрослый билет (500 руб.)");
            Console.WriteLine("2. Child - Детский билет (250 руб.)");
            Console.WriteLine("3. Student - Студенческий билет (350 руб.)");
            Console.WriteLine("4. Family - Семейный билет (1500 руб.)");
            Console.WriteLine("0. Завершить выбор");
        }

        public void DisplayTicketAdded(TicketType type, decimal price)
        {
            Console.WriteLine($"Добавлен билет: {type} - {price} руб.");
        }

        public void DisplayTicketsCount(int count)
        {
            Console.WriteLine($"Оформлено билетов: {count}");
        }

        public void DisplayOrderSummary(Purchase order)
        {
            Console.WriteLine("\n>>> Шаг 4: Подтверждение оплаты");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"Покупатель: {order.Customer.Name}");
            Console.WriteLine($"Количество билетов: {order.Tickets.Count}");
            
            Console.WriteLine("\nСостав заказа:");
            for (int i = 0; i < order.Tickets.Count; i++)
            {
                Ticket ticket = order.Tickets[i];
                Console.WriteLine($"  {i + 1}. {ticket.Type} - {ticket.BasePrice} руб.");
            }
            
            Console.WriteLine($"\nИТОГО К ОПЛАТЕ: {order.TotalAmount} руб.");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public void DisplayArgumentHint(string animalListArgument)
        {
            if (string.IsNullOrEmpty(animalListArgument))
            {
                Console.WriteLine("──────────────────────────────────────────────");
                Console.WriteLine("Совет: Вы можете выбрать список животных,");
                Console.WriteLine("указав команду при запуске программы:");
                Console.WriteLine("  • zoo - полный список животных зоопарка");
                Console.WriteLine("  • default - базовый список (3 животных)");
                Console.WriteLine();
                Console.WriteLine("Пример: ZooTicketSystem.exe zoo");
                Console.WriteLine("──────────────────────────────────────────────");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Выбран список животных: " + animalListArgument);
                Console.WriteLine();
            }
        }

        public void DisplayWelcomeBanner()
        {
            Console.WriteLine("###############################################");
            Console.WriteLine("#  Zoo Ticket Booking System v1.0           #");
            Console.WriteLine("###############################################");
            Console.WriteLine();
        }

        public void DisplayWorkCompleted()
        {
            Console.WriteLine("\n*** Работа завершена ***\n");
        }

        public void DisplayNoTicketsSelected()
        {
            Console.WriteLine("Не выбрано ни одного билета. Завершение работы.");
        }

        public void DisplayPaymentCancelled()
        {
            Console.WriteLine("Оплата отменена пользователем.");
        }

        public void DisplayTransactionSuccess()
        {
            Console.WriteLine("Транзакция завершена успешно!");
        }

        public void DisplayZooSetupHeader()
        {
            Console.WriteLine(">>> Шаг 1: Инициализация зоопарка");
            Console.WriteLine();
        }

        public void DisplayAnimalsAdded(int count)
        {
            Console.WriteLine("Добавлено животных: " + count);
            Console.WriteLine();
        }

        public void DisplayAnimalsInfo(string info)
        {
            Console.WriteLine(info);
        }

        public void DisplayCustomerHeader()
        {
            Console.WriteLine(">>> Шаг 2: Загрузка данных клиента");
        }

        public void DisplayCustomerCategory(Customer person, string category)
        {
            Console.WriteLine($"\nКлиент: {person.Name} [{person.Age} лет] - {category}");
        }

        public void DisplayInvalidTicketChoice()
        {
            Console.WriteLine("Некорректный выбор. Попробуйте снова.");
        }

        public void DisplayTransactionHeader()
        {
            Console.WriteLine("\n>>> Расчет стоимости");
        }

        public void DisplayTransactionId(string id)
        {
            Console.WriteLine("ID транзакции: " + id);
        }

        public void DisplayOutputHeader()
        {
            Console.WriteLine(">>> Шаг 5: Генерация документа");
        }

        public void DisplayDocument(string document)
        {
            Console.WriteLine(document);
        }

        public void DisplayFileSaved(string filename)
        {
            Console.WriteLine("Документ сохранен: " + filename);
        }

        public void DisplayFileSaveError(string message)
        {
            Console.WriteLine("Ошибка при сохранении файла: " + message);
        }

        public void DisplayFileError(string message)
        {
            Console.WriteLine("\n!!! Файл отсутствует: " + message);
        }

        public void DisplayValidationError(string message)
        {
            Console.WriteLine("\n!!! Ошибка валидации: " + message);
        }

        public void DisplayBusinessError(string message)
        {
            Console.WriteLine("\n!!! Бизнес-ошибка: " + message);
        }

        public void DisplayCriticalError(string typeName, string message)
        {
            Console.WriteLine("\n!!! Критическая ошибка [" + typeName + "]: " + message);
        }

        public void PauseBeforeExit()
        {
            Console.WriteLine("\nДля выхода нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}
