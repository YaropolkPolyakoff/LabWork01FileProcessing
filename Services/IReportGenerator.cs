using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Контракт для генератора итоговых документов (чеков/отчётов).
    /// Единственная ответственность: форматирование и генерация текстовых документов по покупке.
    /// Изолирует логику вёрстки документов от бизнес-логики сервиса зоопарка (SRP).
    /// </summary>
    public interface IReportGenerator
    {
        /// <summary>
        /// Генерирует форматированный текстовый чек-подтверждение покупки билетов.
        /// </summary>
        /// <param name="purchase">Покупка, для которой формируется документ.</param>
        /// <returns>Строка с текстовым представлением чека подтверждения.</returns>
        string GeneratePurchaseConfirmation(Purchase purchase);
    }
}
