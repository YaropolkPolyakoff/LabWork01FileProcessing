using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZooTicketSystem.Models
{
    public class Purchase
    {
        public Customer Customer { get; private set; }
        public List<Ticket> Tickets { get; private set; }
        public decimal TotalAmount { get; private set; }
        public PurchaseStatus Status { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public string ConfirmationNumber { get; private set; }

        public Purchase(Customer customer, List<Ticket> tickets, DateTime purchaseDate, string confirmationNumber)
        {
            if (customer == null)
            {
                throw new ArgumentException("Покупатель не может быть null");
            }
            
            if (tickets == null || tickets.Count == 0)
            {
                throw new ArgumentException("Список билетов не может быть пустым");
            }
            
            if (tickets.Count > 10)
            {
                throw new ArgumentException("Нельзя купить более 10 билетов за одну покупку");
            }

            if (string.IsNullOrWhiteSpace(confirmationNumber))
            {
                throw new ArgumentException("Номер подтверждения не может быть пустым");
            }

            Customer = customer;
            Tickets = tickets;
            PurchaseDate = purchaseDate;
            Status = PurchaseStatus.Pending;
            ConfirmationNumber = confirmationNumber;
            TotalAmount = 0;
        }

        /// <summary>
        /// Устанавливает итоговую сумму покупки.
        /// Вызывается сервисом после расчёта цены до подтверждения покупки.
        /// </summary>
        public void SetTotalAmount(decimal amount)
        {
            TotalAmount = amount;
        }

        public static string GenerateConfirmationNumber()
        {
            // Using Guid ensures uniqueness without needing Random
            Random random = new Random(Guid.NewGuid().GetHashCode());
            return "ZOO-" + DateTime.Now.ToString("yyyyMMdd") + "-" + random.Next(1000, 9999);
        }

        public bool CanConfirm()
        {
            return Status == PurchaseStatus.Pending;
        }

        public void Confirm()
        {
            if (!CanConfirm())
            {
                throw new InvalidOperationException("Невозможно подтвердить покупку со статусом: " + Status);
            }
            
            Status = PurchaseStatus.Confirmed;
            foreach (var ticket in Tickets)
            {
                ticket.SetStatus(PurchaseStatus.Confirmed);
            }
        }

        public void Cancel()
        {
            if (Status == PurchaseStatus.Used)
                throw new InvalidOperationException("Нельзя отменить использованную покупку");
            {
            
            }
            Status = PurchaseStatus.Cancelled;
            foreach (var ticket in Tickets)
            {
                ticket.SetStatus(PurchaseStatus.Cancelled);
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

