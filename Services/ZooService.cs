using System;
using System.Collections.Generic;
using System.Text;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    public class ZooService
    {
        private Zoo zoo;
        private PricingService pricingService;
        private ValidationService validationService;

        public ZooService()
        {
            zoo = new Zoo("Центральный зоопарк", 9, 18);
            pricingService = new PricingService();
            validationService = new ValidationService();
        }

        public Zoo GetZoo()
        {
            return zoo;
        }

        public string GetAnimalsInfo()
        {
            if (zoo.Animals == null || zoo.Animals.Count == 0)
                return "В зоопарке пока нет животных";
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ЖИВОТНЫЕ ЗООПАРКА ===");
            sb.AppendLine("");
            
            int index = 0;
            while (index < zoo.Animals.Count)
            {
                sb.AppendLine(zoo.Animals[index].ToString());
                sb.AppendLine("");
                index++;
            }
            
            return sb.ToString();
        }

        public Purchase CreatePurchase(Customer customer, List<Ticket> tickets, DateTime purchaseDate)
        {
            validationService.ValidatePurchaseRequest(customer, tickets, purchaseDate, zoo);
            
            Purchase purchase = new Purchase(customer, tickets, purchaseDate);
            
            decimal totalPrice = pricingService.CalculateTotalPrice(purchase, zoo);
            purchase.TotalAmount = totalPrice;
            
            return purchase;
        }

        public void ConfirmPurchase(Purchase purchase)
        {
            if (purchase == null)
                throw new ArgumentException("Покупка не может быть null");
            
            if (!purchase.CanConfirm())
                throw new InvalidOperationException("Невозможно подтвердить покупку со статусом: " + purchase.Status);
            
            purchase.Confirm();
        }

        public string GeneratePurchaseConfirmation(Purchase purchase)
        {
            if (purchase == null)
                throw new ArgumentException("Покупка не может быть null");
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("╔════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║          ПОДТВЕРЖДЕНИЕ ПОКУПКИ БИЛЕТОВ В ЗООПАРК          ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════════╝");
            sb.AppendLine("");
            sb.AppendLine("Номер подтверждения: " + purchase.ConfirmationNumber);
            sb.AppendLine("Дата покупки: " + purchase.PurchaseDate.ToString("dd.MM.yyyy HH:mm"));
            sb.AppendLine("Покупатель: " + purchase.Customer.Name);
            sb.AppendLine("Email: " + purchase.Customer.Email);
            sb.AppendLine("Количество билетов: " + purchase.Tickets.Count);
            sb.AppendLine("ОБЩАЯ СУММА: " + purchase.TotalAmount.ToString("C") + " руб.");
            sb.AppendLine("Статус: " + purchase.Status);
            return sb.ToString();
        }
    }
}

