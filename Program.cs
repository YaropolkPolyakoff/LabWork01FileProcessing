using System;
using ZooTicketSystem.Models;
using ZooTicketSystem.Services;
using ZooTicketSystem.Utils;

namespace ZooTicketSystem
{
    /// <summary>
    /// Точка входа приложения.
    /// Единственная ответственность: инициализация окружения, запуск сценария
    /// и обработка необработанных исключений верхнего уровня.
    /// </summary>
    class Program
    {
        static void Main(string[] inputArguments)
        {
            // Инициализация кодировки консоли для правильного отображения кириллицы
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.InputEncoding = System.Text.Encoding.UTF8;
            }
            catch
            {
                /* Игнорируем ошибки установки кодировки на некоторых системах */
            }

            string animalListArgument = (inputArguments != null && inputArguments.Length > 0)
                ? inputArguments[0]
                : "";

            ConsoleUI consoleUI = new ConsoleUI();
            InputParser inputParser = new InputParser();
            InputReader inputReader = new InputReader(new InputParser());

            consoleUI.DisplayArgumentHint(animalListArgument);
            consoleUI.DisplayWelcomeBanner();

            // Composition root: создаём конкретные реализации и внедряем их через интерфейсы (DIP)
            IPricingService pricingService = new PricingService();
            IValidationService validationService = new ValidationService();
            IZooService zooService = new ZooService(pricingService, validationService);
            IReportGenerator reportGenerator = new ReportGenerator();

            BookingOrchestrator orchestrator = new BookingOrchestrator(
                animalListArgument,
                zooService,
                consoleUI,
                reportGenerator,
                inputParser,
                inputReader);

            try
            {
                orchestrator.Run();
                consoleUI.DisplayWorkCompleted();
            }
            catch (FileNotFoundException errorInfo)
            {
                consoleUI.DisplayFileError(errorInfo.Message);
            }
            catch (ArgumentException errorInfo)
            {
                consoleUI.DisplayValidationError(errorInfo.Message);
            }
            catch (InvalidOperationException errorInfo)
            {
                consoleUI.DisplayBusinessError(errorInfo.Message);
            }
            catch (Exception errorInfo)
            {
                consoleUI.DisplayCriticalError(errorInfo.GetType().Name, errorInfo.Message);
            }

            consoleUI.PauseBeforeExit();
        }
    }
}

