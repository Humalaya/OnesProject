using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using backend.Application.Repositories;
using backend.Application.Services;

namespace backend.Application.Features.ProfileFeature.Commands.ResendVerification
{
    public class ResendVerificationCommandRequest : IRequest<bool>
    {
        public Guid UserID { get; set; }
    }

    public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommandRequest, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public ResendVerificationCommandHandler(IUserRepository userRepository, IEmailSender emailSender, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<bool> Handle(ResendVerificationCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null) return false;

            if (user.EmailVerified) return true;

            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:4200";
            var verificationLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-email?token={user.EmailVerificationToken}";
            await _emailSender.SendWelcomeEmailAsync(user, verificationLink, cancellationToken);

            return true;
        }
    }
}
