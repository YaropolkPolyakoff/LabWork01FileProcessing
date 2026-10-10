using System;

namespace ZooTicketSystem.Utils
{
    /// <summary>
    /// Отвечает за чтение данных из консоли с валидацией
    /// </summary>
    public class InputReader
    {
        private readonly InputParser _parser;

        public InputReader(InputParser parser)
        {
            _parser = parser;
        }

        /// <summary>
        /// Читает непустую строку из консоли
        /// </summary>
        public string ReadNonEmptyLine(string prompt, string errorMessage)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();

            while (string.IsNullOrWhiteSpace(input))
            {
                Console.Write(errorMessage);
                input = Console.ReadLine()?.Trim();
            }

            return input;
        }

        /// <summary>
        /// Читает положительное целое число из консоли
        /// </summary>
        public int ReadPositiveInteger(string prompt, string errorMessage)
        {
            Console.Write(prompt);
            int value = 0;

            while (value <= 0)
            {
                string input = Console.ReadLine()?.Trim();
                if (!_parser.TryParsePositiveInteger(input, out value))
                {
                    Console.Write(errorMessage);
                }
            }

            return value;
        }

        /// <summary>
        /// Читает email с базовой валидацией
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
        /// Читает необязательную строку из консоли
        /// </summary>
        public string ReadOptionalLine(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim();
        }
    }
}
