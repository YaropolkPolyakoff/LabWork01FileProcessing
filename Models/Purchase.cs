using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZooTicketSystem.Models
{
    public class Purchase
    {
        public Customer Customer { get; set; }
        public List<Ticket> Tickets { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseStatus Status { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ConfirmationNumber { get; set; }

        public Purchase(Customer customer, List<Ticket> tickets, DateTime purchaseDate)
        {
            if (customer == null)
                throw new ArgumentException("Покупатель не может быть null");
            
            if (tickets == null || tickets.Count == 0)
                throw new ArgumentException("Список билетов не может быть пустым");
            
            if (tickets.Count > 10)
                throw new ArgumentException("Нельзя купить более 10 билетов за одну покупку");

            Customer = customer;
            Tickets = tickets;
            PurchaseDate = purchaseDate;
            Status = PurchaseStatus.Pending;
            ConfirmationNumber = GenerateConfirmationNumber();
            TotalAmount = 0;
        }

        private string GenerateConfirmationNumber()
        {
            Random random = new Random();
            return "ZOO-" + DateTime.Now.ToString("yyyyMMdd") + "-" + random.Next(1000, 9999);
        }

        public bool CanConfirm()
        {
            return Status == PurchaseStatus.Pending;
        }

        public void Confirm()
        {
            if (!CanConfirm())
                throw new InvalidOperationException("Невозможно подтвердить покупку со статусом: " + Status);
            
            Status = PurchaseStatus.Confirmed;
            foreach (var ticket in Tickets)
            {
                ticket.Status = PurchaseStatus.Confirmed;
            }
        }

        public void Cancel()
        {
            if (Status == PurchaseStatus.Used)
                throw new InvalidOperationException("Нельзя отменить использованную покупку");
            
            Status = PurchaseStatus.Cancelled;
            foreach (var ticket in Tickets)
            {
                ticket.Status = PurchaseStatus.Cancelled;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ПОДТВЕРЖДЕНИЕ ПОКУПКИ ===");
            sb.AppendLine("Номер подтверждения: " + ConfirmationNumber);
            sb.AppendLine("Покупатель: " + Customer.Name + " (" + Customer.Email + ")");
            sb.AppendLine("Количество билетов: " + Tickets.Count);
            sb.AppendLine("Общая сумма: " + TotalAmount.ToString("C"));
            sb.AppendLine("Статус: " + Status);
            return sb.ToString();
        }
    }
}

