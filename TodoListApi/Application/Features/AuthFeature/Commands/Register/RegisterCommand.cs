using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TodoListApi.Application.Repositories;
using TodoListApi.Application.Services;
using TodoListApi.Domain.Entities;

namespace TodoListApi.Application.Features.AuthFeature.Commands.Register
{
    public class RegisterCommandRequest : IRequest<AuthResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public RegisterCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
                throw new InvalidOperationException("A user with this email already exists.");

            if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken: cancellationToken))
                throw new InvalidOperationException("This username is already taken.");

            var user = new User
            {
                ID = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                Token = _tokenService.GenerateToken(user),
                UserId = user.ID,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }
    }
}
