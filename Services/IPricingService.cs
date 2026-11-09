using System;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Контракт для службы расчёта стоимости билетов.
    /// Позволяет подменять реализацию алгоритма ценообразования без изменения вызывающих сервисов.
    /// </summary>
    public interface IPricingService
    {
        /// <summary>
        /// Вычисляет цену с учетом возрастной скидки.
        /// </summary>
        decimal CalculateDiscountedPrice(decimal basePrice, Customer customer);

        /// <summary>
        /// Применяет оптовую скидку при групповой покупке.
        /// </summary>
        decimal CalculateGroupDiscount(decimal totalPrice, int ticketCount);

        /// <summary>
        /// Применяет наценку выходного дня.
        /// </summary>
        decimal ApplyWeekendSurcharge(decimal price, DateTime date);

        /// <summary>
        /// Вычисляет общую стоимость покупки с учётом всех бизнес-правил.
        /// </summary>
        decimal CalculateTotalPrice(Purchase purchase);
    }
}
