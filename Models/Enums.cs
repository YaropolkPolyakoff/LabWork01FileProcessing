namespace ZooTicketSystem.Models
{
    /// <summary>
    /// Тип билета
    /// </summary>
    public enum TicketType
    {
        Adult,      // Взрослый
        Child,      // Детский
        Student,    // Студенческий
        Family      // Семейный (2 взрослых + 2 детей)
    }

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

    /// <summary>
    /// День недели
    /// </summary>
    public enum DayOfWeek
    {
        Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
    }
}

