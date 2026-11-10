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
        /// Вычисляет общую стоимость покупки с учётом всех бизнес-правил.
        /// </summary>
        decimal CalculateTotalPrice(Purchase purchase);
    }
}
