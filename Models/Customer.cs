using System;

namespace ZooTicketSystem.Models
{
    /// <summary>
    /// Представляет покупателя билетов
    /// </summary>
    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Customer(string name, int age, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя покупателя не может быть пустым");
            
            // Бизнес-правило 7: Валидация возраста (0-120 лет)
            if (age < 0)
                throw new ArgumentException("Возраст не может быть отрицательным");
            
            if (age > 120)
                throw new ArgumentException("Возраст не может превышать 120 лет");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым");

            Name = name;
            Age = age;
            Email = email;
            Phone = phone ?? "Не указан";
        }

        /// <summary>
        /// Определяет категорию покупателя для скидок
        /// </summary>
        public string GetCategory()
        {
            // Бизнес-правило 1: Дети (до 12 лет) - 50% скидка
            if (Age < 12)
                return "Ребенок";
            
            // Бизнес-правило 2: Студенты (12-18 лет) - 30% скидка
            if (Age >= 12 && Age <= 18)
                return "Студент";
            
            return "Взрослый";
        }
    }
}

