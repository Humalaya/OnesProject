using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ProfileFeature.Queries.GetProfile
{
    public class GetProfileQueryRequest : IRequest<GetProfileQueryResponse?>
    {
        public Guid UserID { get; set; }
    }

    public class GetProfileQueryResponse
    {
        public Guid ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQueryRequest, GetProfileQueryResponse?>
    {
        private readonly IUserRepository _userRepository;

        public GetProfileQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetProfileQueryResponse?> Handle(GetProfileQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null) return null;

            return new GetProfileQueryResponse
            {
                ID = user.ID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
