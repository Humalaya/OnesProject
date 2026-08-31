using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using backend.Application.Repositories;
using backend.Application.Services;
using backend.Domain.Entities;

namespace backend.Application.Features.AuthFeature.Commands.Register
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
        public bool EmailVerified { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _configuration = configuration;
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
                CreatedAt = DateTime.UtcNow,
                EmailVerified = false,
                EmailVerificationToken = Guid.NewGuid().ToString("N"),
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:4200";
            var verificationLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-email?token={user.EmailVerificationToken}";
            await _emailSender.SendWelcomeEmailAsync(user, verificationLink, cancellationToken);

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
