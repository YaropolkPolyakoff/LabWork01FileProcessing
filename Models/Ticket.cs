using System;

namespace ZooTicketSystem.Models
{
    public class Ticket
    {
        public TicketType Type { get; set; }
        public decimal BasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ValidDate { get; set; }
        public PurchaseStatus Status { get; set; }

        public Ticket(TicketType type, decimal basePrice, DateTime purchaseDate, DateTime validDate)
        {
            if (basePrice <= 0)
                throw new ArgumentException("Цена билета должна быть положительной");
            
            if (validDate < purchaseDate)
                throw new ArgumentException("Дата действия билета не может быть раньше даты покупки");

            Type = type;
            BasePrice = basePrice;
            PurchaseDate = purchaseDate;
            ValidDate = validDate;
            Status = PurchaseStatus.Pending;
        }

        public static decimal GetBasePriceByType(TicketType type)
        {
            decimal price;
            
            if (type == TicketType.Adult)
                price = 500m;
            else if (type == TicketType.Child)
                price = 250m;
            else if (type == TicketType.Student)
                price = 350m;
            else if (type == TicketType.Family)
                price = 1500m;
            else
                throw new ArgumentException("Неизвестный тип билета");
            
            return price;
        }

        public string GetStatusText()
        {
            if (Status == PurchaseStatus.Pending)
                return "Ожидает подтверждения";
            else if (Status == PurchaseStatus.Confirmed)
                return "Подтверждена";
            else if (Status == PurchaseStatus.Cancelled)
                return "Отменена";
            else if (Status == PurchaseStatus.Used)
                return "Использована";
            return "Неизвестно";
        }

        public override string ToString()
        {
            return "Билет: " + Type + "\n" +
                   "  Базовая цена: " + BasePrice.ToString("C") + "\n" +
                   "  Дата покупки: " + PurchaseDate.ToString("dd.MM.yyyy") + "\n" +
                   "  Действителен: " + ValidDate.ToString("dd.MM.yyyy") + "\n" +
                   "  Статус: " + GetStatusText();
        }
    }
}

