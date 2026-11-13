using System;
using ZooTicketSystem.Models;
using System.Collections.Generic;
using ZooTicketSystem.Services;

namespace ZooTicketSystem.Utils
{
    /// <summary>
    /// Отвечает за вывод информации в консоль
    /// </summary>
    public class ConsoleUI : IUserInterface
    {
        // ──────────────────── Ввод данных из консоли ────────────────────

        /// <summary>
        /// Запрашивает строку, пока пользователь не введёт непустое значение.
        /// </summary>
        public string ReadNonEmptyLine(string prompt, string errorMessage)
        {
            Console.Write(prompt);
            string text = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(text))
            {
                Console.Write(errorMessage);
                text = Console.ReadLine()?.Trim();
            }
            return text;
        }

        /// <summary>
        /// Запрашивает целое положительное число, повторяя запрос при ошибке.
        /// Использует встроенный int.TryParse вместо отдельного класса-парсера.
        /// </summary>
        public int ReadPositiveInteger(string prompt, string errorMessage)
        {
            Console.Write(prompt);
            int parsed = 0;
            while (parsed <= 0)
            {
                string raw = Console.ReadLine()?.Trim();
                if (int.TryParse(raw, out parsed) && parsed > 0)
                {
                    break;
                }
                parsed = 0;
                Console.Write(errorMessage);
            }
            return parsed;
        }

        /// <summary>
        /// Запрашивает email-адрес, проверяя наличие символа «@».
        /// </summary>
        public string ReadEmail(string prompt, string errorMessage)
        {
            Console.Write(prompt);
            string email = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                Console.Write(errorMessage);
                email = Console.ReadLine()?.Trim();
            }
            return email;
        }

        /// <summary>
        /// Запрашивает строку, допуская пустой ввод.
        /// </summary>
        public string ReadOptionalLine(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim();
        }

        /// <summary>
        /// Запрашивает подтверждение и возвращает true для «да/д/yes/y».
        /// </summary>
        public bool ReadYesNoConfirmation(string prompt)
        {
            string answer = ReadOptionalLine(prompt) ?? string.Empty;
            return answer.Equals("да",  StringComparison.CurrentCultureIgnoreCase)
                || answer.Equals("д",   StringComparison.CurrentCultureIgnoreCase)
                || answer.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
                || answer.Equals("y",   StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Разбирает строку с порядковым номером (1..N) в значение TicketType.
        /// Список типов получается динамически через Enum.GetValues, поэтому
        /// добавление нового значения в перечисление не требует правок этого метода.
        /// Возвращает найденный TicketType, либо null при некорректном вводе.
        /// </summary>
        public TicketType? ParseTicketTypeChoice(string input)
        {
            TicketType[] values = Enum.GetValues<TicketType>();
            if (int.TryParse(input?.Trim(), out int index) && index >= 1 && index <= values.Length)
            {
                return values[index - 1];
            }
            return null;
        }

        // ──────────────────── Вывод данных в консоль ────────────────────

        public void DisplayStepHeader(string stepTitle)
        {
            Console.WriteLine($">>> {stepTitle}");
            Console.WriteLine();
        }

        public void DisplayTicketOptions()
        {
            Console.WriteLine("\nДоступные типы билетов:");
            TicketType[] values = Enum.GetValues<TicketType>();
            for (int i = 0; i < values.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {values[i]}");
            }
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

        /// <summary>
        /// Возвращает социальную категорию клиента, делегируя определение самому объекту Customer.
        /// </summary>
        public string GetCustomerCategory(Customer person)
        {
            return person.GetAgeCategory();
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
