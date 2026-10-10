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
    }
}
