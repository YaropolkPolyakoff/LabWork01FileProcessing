using System;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    public class PricingService
    {
        public decimal CalculateDiscountedPrice(decimal basePrice, Customer customer)
        {
            decimal price = basePrice;
            
            if (customer.Age < 12)
            {
                price = price * 0.5m;
            }
            else if (customer.Age >= 12 && customer.Age <= 18)
            {
                price = price * 0.7m;
            }
            
            return price;
        }

        public decimal CalculateGroupDiscount(decimal totalPrice, int ticketCount)
        {
            if (ticketCount >= 5)
            {
                return totalPrice * 0.8m;
            }
            return totalPrice;
        }

        public decimal ApplyWeekendSurcharge(decimal price, DateTime date)
        {
            if (date.DayOfWeek == System.DayOfWeek.Saturday || 
                date.DayOfWeek == System.DayOfWeek.Sunday)
            {
                return price * 1.15m;
            }
            return price;
        }

        public decimal CalculateTotalPrice(Purchase purchase, Zoo zoo)
        {
            if (purchase == null)
                throw new ArgumentException("Покупка не может быть null");
            
            decimal totalPrice = 0;
            
            foreach (var ticket in purchase.Tickets)
            {
                decimal ticketPrice = ticket.BasePrice;
                
                ticketPrice = CalculateDiscountedPrice(ticketPrice, purchase.Customer);
                
                ticketPrice = ApplyWeekendSurcharge(ticketPrice, ticket.ValidDate);
                
                totalPrice += ticketPrice;
            }
            
            totalPrice = CalculateGroupDiscount(totalPrice, purchase.Tickets.Count);
            
            if (totalPrice < 0)
                throw new InvalidOperationException("Итоговая цена не может быть отрицательной");
            
            return Math.Round(totalPrice, 2);
        }
    }
}

