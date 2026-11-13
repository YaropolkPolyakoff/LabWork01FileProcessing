using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    public class ZooService : IZooService
    {
        private Zoo zoo;
        private readonly IPricingService pricingService;
        private readonly ValidationService validationService;

        public ZooService(IPricingService pricingService, ValidationService validationService)
        {
            zoo = new Zoo("Центральный зоопарк", 9, 18);
            this.pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
            this.validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public Zoo GetZoo()
        {
            return zoo;
        }

        public string GetAnimalsInfo()
        {
            if (zoo.Animals == null || zoo.Animals.Count == 0)
            {
                return "В зоопарке пока нет животных";
            }
            
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
            if (!zoo.IsOpenOnDay(purchaseDate))
            {
                string closedDayName = GetDayNameInRussian(purchaseDate.DayOfWeek);
                throw new InvalidOperationException(
                    $"Зоопарк закрыт в {closedDayName}. Выберите другой день.");
            }

            validationService.ValidatePurchaseRequest(customer, tickets, purchaseDate);

            string confirmationNumber = Purchase.GenerateConfirmationNumber();
            
            Purchase purchase = new Purchase(customer, tickets, purchaseDate, confirmationNumber);
            
            decimal totalPrice = pricingService.CalculateTotalPrice(purchase);
            purchase.SetTotalAmount(totalPrice);
            
            return purchase;
        }
        public Ticket CreateTicket(TicketType type)
        {
            DateTime issueDate = DateTime.Now;
            DateTime expiryDate = issueDate.AddDays(7);
            decimal price = Ticket.GetBasePriceByType(type);
            return new Ticket(type, price, issueDate, expiryDate);
        }


        public void ConfirmPurchase(Purchase purchase)
        {
            if (purchase == null)
                throw new ArgumentException("Покупка не может быть null");
            {
            
            }
            if (!purchase.CanConfirm())
                throw new InvalidOperationException("Невозможно подтвердить покупку со статусом: " + purchase.Status);
            {
            
            }
            purchase.Confirm();
        }

        private static string GetDayNameInRussian(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return "понедельник";
                case DayOfWeek.Tuesday:
                    return "вторник";
                case DayOfWeek.Wednesday:
                    return "среду";
                case DayOfWeek.Thursday:
                    return "четверг";
                case DayOfWeek.Friday:
                    return "пятницу";
                case DayOfWeek.Saturday:
                    return "субботу";
                case DayOfWeek.Sunday:
                    return "воскресенье";
                default:
                    return day.ToString();
            }
        }
    }
}

