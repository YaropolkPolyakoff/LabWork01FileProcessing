using System;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    public class PricingService
    {
        /// <summary>
        /// Вычисляет цену с учетом возрастной скидки
        /// Бизнес-правило 1: Дети (до 12 лет) - 50% скидка
        /// Бизнес-правило 2: Студенты (12-18 лет) - 30% скидка
        /// </summary>
        /// <param name="basePrice">Базовая цена билета</param>
        /// <param name="customer">Покупатель</param>
        /// <returns>Цена с учетом возрастной скидки</returns>
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

        /// <summary>
        /// Применяет оптовую скидку при групповой покупке
        /// Бизнес-правило 3: Групповая скидка 20% при покупке 5+ билетов
        /// </summary>
        /// <param name="totalPrice">Общая сумма до скидки</param>
        /// <param name="ticketCount">Количество билетов</param>
        public decimal CalculateGroupDiscount(decimal totalPrice, int ticketCount)
        {
            if (ticketCount >= 5)
            {
                return totalPrice * 0.8m;
            }
            return totalPrice;
        }

        /// <summary>
        /// Применяет наценку выходного дня
        /// Бизнес-правило 4: Наценка 15% в выходные дни (суббота, воскресенье)
        /// </summary>
        /// <param name="price">Цена до наценки</param>
        /// <param name="date">Дата посещения</param>
        public decimal ApplyWeekendSurcharge(decimal price, DateTime date)
        {
            if (date.DayOfWeek == System.DayOfWeek.Saturday || 
                date.DayOfWeek == System.DayOfWeek.Sunday)
            {
                return price * 1.15m;
            }
            return price;
        }

        /// <summary>
        /// Вычисляет итоговую цену одного билета с учетом всех индивидуальных правил
        /// (возрастная скидка + наценка выходного дня)
        /// </summary>
        /// <param name="ticket">Билет</param>
        /// <param name="customer">Покупатель</param>
        /// <returns>Итоговая цена билета</returns>
        private decimal CalculateTicketPrice(Ticket ticket, Customer customer)
        {
            decimal ticketPrice = ticket.BasePrice;
            
            // Применяем возрастную скидку
            ticketPrice = CalculateDiscountedPrice(ticketPrice, customer);
            
            // Применяем наценку выходного дня
            ticketPrice = ApplyWeekendSurcharge(ticketPrice, ticket.ValidDate);
            
            return ticketPrice;
        }

        /// <summary>
        /// Вычисляет промежуточную сумму для всех билетов
        /// Атомарный метод: только перебор корзины и накопление суммы
        /// </summary>
        /// <param name="purchase">Покупка с билетами</param>
        /// <returns>Промежуточная сумма</returns>
        private decimal CalculateSubtotal(Purchase purchase)
        {
            decimal subtotal = 0;
            
            foreach (var ticket in purchase.Tickets)
            {
                subtotal += CalculateTicketPrice(ticket, purchase.Customer);
            }
            
            return subtotal;
        }

        /// <summary>
        /// Применяет оптовый коэффициент (групповую скидку) к промежуточной сумме
        /// </summary>
        /// <param name="subtotal">Промежуточная сумма</param>
        /// <param name="ticketCount">Количество билетов</param>
        /// <returns>Сумма с учетом оптового коэффициента</returns>
        private decimal ApplyBulkDiscount(decimal subtotal, int ticketCount)
        {
            return CalculateGroupDiscount(subtotal, ticketCount);
        }

        /// <summary>
        /// Валидирует итоговую цену
        /// </summary>
        /// <param name="totalPrice">Итоговая цена для проверки</param>
        /// <exception cref="InvalidOperationException">Если цена отрицательная</exception>
        private void ValidateTotalPrice(decimal totalPrice)
        {
            if (totalPrice < 0)
            {
                throw new InvalidOperationException("Итоговая цена не может быть отрицательной");
            }
        }

        /// <summary>
        /// Вычисляет общую стоимость покупки
        /// Метод-координатор: оркеструет вызовы атомарных методов
        /// </summary>
        /// <param name="purchase">Покупка</param>
        /// <param name="zoo">Зоопарк</param>
        /// <returns>Итоговая сумма покупки</returns>
        public decimal CalculateTotalPrice(Purchase purchase, Zoo zoo)
        {
            if (purchase == null)
            {
                throw new ArgumentException("Покупка не может быть null");
            }
            
            // Шаг 1: Вычисляем промежуточную сумму (перебор корзины + индивидуальные правила)
            decimal subtotal = CalculateSubtotal(purchase);
            
            // Шаг 2: Применяем оптовый коэффициент (групповую скидку)
            decimal totalPrice = ApplyBulkDiscount(subtotal, purchase.Tickets.Count);
            
            // Шаг 3: Валидируем итоговую цену
            ValidateTotalPrice(totalPrice);
            
            // Шаг 4: Округляем до 2 знаков после запятой
            return Math.Round(totalPrice, 2);
        }
    }
}

