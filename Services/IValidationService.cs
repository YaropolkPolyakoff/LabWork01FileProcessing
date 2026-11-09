using System;
using System.Collections.Generic;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Контракт для службы валидации запросов на покупку билетов.
    /// Позволяет подменять реализацию правил валидации без изменения вызывающих сервисов.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Проверяет корректность запроса на покупку.
        /// </summary>
        void ValidatePurchaseRequest(Customer customer, List<Ticket> tickets, DateTime purchaseDate);

        /// <summary>
        /// Разбирает строку и возвращает тип билета.
        /// </summary>
        TicketType ParseTicketType(string input);
    }
}
