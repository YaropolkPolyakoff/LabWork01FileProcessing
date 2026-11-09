using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Контракт для основной службы зоопарка.
    /// Позволяет подменять реализацию бизнес-логики зоопарка без изменения вызывающих сервисов.
    /// </summary>
    public interface IZooService
    {
        /// <summary>
        /// Возвращает текущий экземпляр зоопарка.
        /// </summary>
        Zoo GetZoo();

        /// <summary>
        /// Возвращает форматированный список животных зоопарка.
        /// </summary>
        string GetAnimalsInfo();

        /// <summary>
        /// Создаёт покупку с набором билетов и рассчитывает итоговую стоимость.
        /// </summary>
        Purchase CreatePurchase(Customer customer, List<Ticket> tickets, DateTime purchaseDate);

        /// <summary>
        /// Создаёт билет указанного типа.
        /// </summary>
        Ticket CreateTicket(TicketType type);

        /// <summary>
        /// Подтверждает покупку, переводя её в статус оплачена.
        /// </summary>
        void ConfirmPurchase(Purchase purchase);

        /// <summary>
        /// Генерирует текстовое подтверждение покупки.
        /// </summary>
        string GeneratePurchaseConfirmation(Purchase purchase);
    }
}
