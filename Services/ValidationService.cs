using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    public class ValidationService
    {
        public void ValidatePurchaseRequest(Customer customer, List<Ticket> tickets, DateTime purchaseDate, Zoo zoo)
        {
            if (customer == null)
                throw new ArgumentException("Информация о покупателе отсутствует");
            
            if (tickets == null || tickets.Count == 0)
                throw new ArgumentException("Список билетов пуст");
            
            if (tickets.Count > 10)
                throw new ArgumentException("Превышено максимальное количество билетов (10) за одну покупку");
            
            if (!zoo.IsOpenOnDay(purchaseDate))
                throw new InvalidOperationException("Зоопарк закрыт в понедельники. Выберите другой день.");
            
            int idx = 0;
            while (idx < tickets.Count)
            {
                Ticket ticket = tickets[idx];
                
                if (ticket.ValidDate < purchaseDate)
                    throw new ArgumentException("Дата действия билета не может быть раньше даты покупки");
                
                if (ticket.BasePrice <= 0)
                    throw new ArgumentException("Цена билета должна быть положительной");
                
                idx++;
            }
        }

        public TicketType ParseTicketType(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Тип билета не указан");
            
            string normalized = input.Trim().ToLower();
            
            if (normalized == "adult" || normalized == "взрослый")
                return TicketType.Adult;
            
            if (normalized == "child" || normalized == "детский")
                return TicketType.Child;
            
            if (normalized == "student" || normalized == "студенческий")
                return TicketType.Student;
            
            if (normalized == "family" || normalized == "семейный")
                return TicketType.Family;
            
            throw new ArgumentException("Неизвестный тип билета: " + input);
        }
    }
}

