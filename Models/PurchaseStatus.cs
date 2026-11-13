namespace ZooTicketSystem.Models
{
    /// <summary>
    /// Статус покупки
    /// </summary>
    public enum PurchaseStatus
    {
        Pending,    // Ожидает обработки
        Confirmed,  // Подтверждена
        Cancelled,  // Отменена
        Used        // Использована
    }
}
