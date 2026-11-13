using System;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Services
{
    /// <summary>
    /// Абстракция пользовательского взаимодействия.
    /// Позволяет слою сервисов зависеть от интерфейса, а не от конкретной реализации UI.
    /// </summary>
    public interface IUserInterface
    {
        // ──────────────────── Ввод данных ────────────────────
        string ReadNonEmptyLine(string prompt, string errorMessage);
        int ReadPositiveInteger(string prompt, string errorMessage);
        string ReadEmail(string prompt, string errorMessage);
        string ReadOptionalLine(string prompt);
        bool ReadYesNoConfirmation(string prompt);
        bool TryReadTicketTypeChoice(string input, out TicketType ticketType);

        // ──────────────────── Вывод данных ────────────────────
        void DisplayStepHeader(string stepTitle);
        void DisplayTicketOptions();
        void DisplayTicketAdded(TicketType type, decimal price);
        void DisplayTicketsCount(int count);
        void DisplayOrderSummary(Purchase order);
        void DisplayNoTicketsSelected();
        void DisplayPaymentCancelled();
        void DisplayTransactionSuccess();
        void DisplayZooSetupHeader();
        void DisplayAnimalsAdded(int count);
        void DisplayAnimalsInfo(string info);
        void DisplayCustomerHeader();
        string GetCustomerCategory(Customer person);
        void DisplayCustomerCategory(Customer person, string category);
        void DisplayInvalidTicketChoice();
        void DisplayTransactionHeader();
        void DisplayTransactionId(string id);
        void DisplayOutputHeader();
        void DisplayDocument(string document);
        void DisplayFileSaved(string filename);
        void DisplayFileSaveError(string message);
        void DisplayFileError(string message);
        void DisplayValidationError(string message);
        void DisplayBusinessError(string message);
        void DisplayCriticalError(string typeName, string message);
        void DisplayWorkCompleted();
        void PauseBeforeExit();
    }
}
