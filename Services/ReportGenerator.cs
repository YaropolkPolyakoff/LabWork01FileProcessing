using System;
using System.Text;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Специализированный сервис для генерации итоговых текстовых документов (чеков/отчётов).
    /// Единственная ответственность: форматирование и вёрстка строковых документов по покупке.
    /// Выделен из ZooService согласно принципу единственной ответственности (SRP).
    /// </summary>
    public class ReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Генерирует форматированный текстовый чек-подтверждение покупки билетов.
        /// </summary>
        /// <param name="purchase">Покупка, для которой формируется документ.</param>
        /// <returns>Строка с текстовым представлением чека подтверждения.</returns>
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
