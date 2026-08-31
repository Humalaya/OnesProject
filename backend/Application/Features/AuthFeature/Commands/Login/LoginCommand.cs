using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Features.AuthFeature.Commands.Register;
using backend.Application.Repositories;
using backend.Application.Services;

namespace backend.Application.Features.AuthFeature.Commands.Login
{
    public class LoginCommandRequest : IRequest<AuthResponse?>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, AuthResponse?>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse?> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            return new AuthResponse
            {
                Token = _tokenService.GenerateToken(user),
                UserId = user.ID,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                EmailVerified = user.EmailVerified
            };
        }
    }
}
