using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;

namespace backend.Application.Services
{
    public interface IEmailSender
    {
        Task SendWelcomeEmailAsync(User user, string verificationLink, CancellationToken cancellationToken = default);
    }
}
