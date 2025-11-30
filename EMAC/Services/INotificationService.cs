using System.Threading.Tasks;

namespace EMAC.Services
{
    public interface INotificationService
    {
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendWhatsAppAsync(string phoneNumber, string message);
    }


    public class DevelopmentNotificationService : INotificationService
    {
        private readonly ILogger<DevelopmentNotificationService> _logger;

        public DevelopmentNotificationService(ILogger<DevelopmentNotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string phoneNumber, string message)
        {
            _logger.LogInformation($"[SMS] To: {phoneNumber} | Message: {message}");
            return Task.CompletedTask;
        }

        public Task SendWhatsAppAsync(string phoneNumber, string message)
        {
            _logger.LogInformation($"[WhatsApp] To: {phoneNumber} | Message: {message}");
            return Task.CompletedTask;
        }
    }
}