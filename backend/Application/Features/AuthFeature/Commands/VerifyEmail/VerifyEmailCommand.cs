using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.AuthFeature.Commands.VerifyEmail
{
    public class VerifyEmailCommandRequest : IRequest<VerifyEmailResult>
    {
        public string Token { get; set; } = string.Empty;
    }

    public enum VerifyEmailResult
    {
        Success,
        InvalidToken,
        Expired
    }

    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommandRequest, VerifyEmailResult>
    {
        private readonly IUserRepository _userRepository;

        public VerifyEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<VerifyEmailResult> Handle(VerifyEmailCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByVerificationTokenAsync(request.Token, cancellationToken);
            if (user == null) return VerifyEmailResult.InvalidToken;

            if (user.EmailVerificationTokenExpiresAt == null || user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
                return VerifyEmailResult.Expired;

            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiresAt = null;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return VerifyEmailResult.Success;
        }
    }
}
