using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ProfileFeature.Commands.ChangePassword
{
    public class ChangePasswordCommandRequest : IRequest<bool>
    {
        public Guid UserID { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommandRequest, bool>
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
