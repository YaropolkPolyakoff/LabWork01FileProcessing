using System;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Utils
{
    /// <summary>
    /// Отвечает за анализ и парсинг пользовательского ввода
    /// </summary>
    public class InputParser
    {
        /// <summary>
        /// Парсит выбор типа билета из пользовательского ввода
        /// </summary>
        public bool TryParseTicketTypeChoice(string input, out TicketType ticketType)
        {
            ticketType = TicketType.Adult;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string normalized = input.Trim();

            switch (normalized)
            {
                case "1":
                    ticketType = TicketType.Adult;
                    return true;
                case "2":
                    ticketType = TicketType.Child;
                    return true;
                case "3":
                    ticketType = TicketType.Student;
                    return true;
                case "4":
                    ticketType = TicketType.Family;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Парсит подтверждение да/нет на русском и английском
        /// </summary>
        public bool ParseYesNoConfirmation(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Используем инвариантную культуру для корректной работы с кириллицей
            string normalized = input.Trim().ToLowerInvariant();

            return normalized == "да" ||
                   normalized == "д" ||
                   normalized == "yes" || 
                   normalized == "y";
        }

        /// <summary>
        /// Пытается распарсить положительное целое число
        /// </summary>
        public bool TryParsePositiveInteger(string input, out int value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            if (int.TryParse(input.Trim(), out value))
            {
                return value > 0;
            }

            return false;
        }
    }
}
