using Microsoft.Extensions.Options;
using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Filters
{
    public class LoggingService
    {
        private readonly AppSettings _appSettings;
        private readonly string logFilePath;
        private readonly string errorFilePath;

        public LoggingService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            Directory.CreateDirectory(_appSettings.LogFilePath);

            logFilePath = Path.Combine(
                _appSettings.LogFilePath,
                "log.txt");

            errorFilePath = Path.Combine(
                _appSettings.LogFilePath,
                "error.txt");
        }

        private void EnsureLogInfrastructure()
        {
            Directory.CreateDirectory(
                _appSettings.LogFilePath);

            if (!File.Exists(logFilePath))
                using (File.Create(logFilePath)) { }

            if (!File.Exists(errorFilePath))
                using (File.Create(errorFilePath)) { }
        }

        public async Task LogAsync(string message)
        {
            try
            {
                EnsureLogInfrastructure();

                string logMessage =
                    $"------------------------------------------------------------------------------{Environment.NewLine}" +
                    $"{DateTime.Now}: {message}{Environment.NewLine}";

                await File.AppendAllTextAsync(
                    logFilePath,
                    logMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Log writing failed: {ex.Message}");
            }
        }

        public async Task LogErrorAsync(string errorMessage)
        {
            try
            {
                EnsureLogInfrastructure();

                string errorLogMessage =
                    $"------------------------------------------------------------------------------{Environment.NewLine}" +
                    $"{DateTime.Now}: ERROR - {errorMessage}{Environment.NewLine}{Environment.NewLine}";

                await File.AppendAllTextAsync(
                    errorFilePath,
                    errorLogMessage);
            }
            catch (Exception ex)
            {
                throw new CustomException(
                    $"Error log writing failed: {ex.Message}");
            }
        }
    }
}