using System;

namespace ZooTicketSystem.Models
{
    /// <summary>
    /// Представляет покупателя билетов
    /// </summary>
    public class Customer
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public Customer(string name, int age, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя покупателя не может быть пустым");
            }
            
            // Бизнес-правило 7: Валидация возраста (0-120 лет)
            if (age < 0)
            {
                throw new ArgumentException("Возраст не может быть отрицательным");
            }
            
            if (age > 120)
            {
                throw new ArgumentException("Возраст не может превышать 120 лет");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email не может быть пустым");
            }

            Name = name;
            Age = age;
            Email = email;
            Phone = phone ?? "Не указан";
        }

        /// <summary>
        /// Возвращает социальную категорию клиента на основе его возраста.
        /// Определение возрастной категории — исключительная обязанность сущности Customer.
        /// </summary>
        public string GetAgeCategory()
        {
            if (Age < 12)
                return "Ребенок";
            if (Age <= 18)
                return "Студент";
            return "Взрослый";
        }
    }
}

