using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using backend.Application.Services;
using backend.Domain.Entities;

namespace backend.Persistence.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(User user, string verificationLink, CancellationToken cancellationToken = default)
        {
            var smtp = _configuration.GetSection("Smtp");
            var appSettings = _configuration.GetSection("AppSettings");
            var appName = appSettings["AppName"] ?? "Our App";
            var logoUrl = appSettings["LogoUrl"];

            var host = smtp["Host"];
            var user_ = smtp["User"];

            if (string.IsNullOrWhiteSpace(host) || host!.Contains("example.com", StringComparison.OrdinalIgnoreCase))
            {
                // SMTP hasn't been configured yet (placeholder values) - don't block registration on it.
                _logger.LogWarning(
                    "Smtp is not configured (Smtp:Host is a placeholder). Skipping welcome email to {Email}. " +
                    "Verification link would have been: {Link}", user.Email, verificationLink);
                return;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(appName, smtp["From"] ?? user_ ?? "no-reply@example.com"));
                message.To.Add(new MailboxAddress(user.Username, user.Email));
                message.Subject = $"{appName} uygulamasına hoşgeldiniz";

                var logoHtml = string.IsNullOrWhiteSpace(logoUrl)
                    ? string.Empty
                    : $"<img src=\"{WebUtility.HtmlEncode(logoUrl)}\" alt=\"{WebUtility.HtmlEncode(appName)}\" style=\"height:48px;margin-bottom:16px;\" />";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:0 auto;"">
                          {logoHtml}
                          <h2>{WebUtility.HtmlEncode(appName)} uygulamasına hoşgeldiniz, {WebUtility.HtmlEncode(user.Username)}!</h2>
                          <p>Hesabınızı oluşturduğunuz için teşekkürler. E-posta adresinizi doğrulamak için aşağıdaki butona tıklayın:</p>
                          <p style=""margin:24px 0;"">
                            <a href=""{WebUtility.HtmlEncode(verificationLink)}""
                               style=""background:#3498db;color:#fff;padding:12px 20px;border-radius:8px;text-decoration:none;display:inline-block;"">
                              E-postamı Doğrula
                            </a>
                          </p>
                          <p style=""color:#888;font-size:0.85rem;"">Bu bağlantı işe yaramazsa, tarayıcınıza şunu yapıştırın: {WebUtility.HtmlEncode(verificationLink)}</p>
                        </div>"
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                var port = int.TryParse(smtp["Port"], out var parsedPort) ? parsedPort : 587;
                var enableSsl = bool.TryParse(smtp["EnableSsl"], out var parsedSsl) ? parsedSsl : true;

                await client.ConnectAsync(host, port, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

                if (!string.IsNullOrWhiteSpace(user_))
                {
                    await client.AuthenticateAsync(user_, smtp["Password"] ?? string.Empty, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                // Never let a mail delivery failure break registration.
                _logger.LogWarning(ex, "Failed to send welcome/verification email to {Email}", user.Email);
            }
        }
    }
}
