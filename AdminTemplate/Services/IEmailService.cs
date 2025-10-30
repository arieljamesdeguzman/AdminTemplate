using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendBulkEmailAsync(List<string> recipients, string subject, string body);
    }
}