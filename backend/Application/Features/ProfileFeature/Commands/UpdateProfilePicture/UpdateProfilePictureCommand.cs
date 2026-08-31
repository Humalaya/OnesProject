using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ProfileFeature.Commands.UpdateProfilePicture
{
    public class UpdateProfilePictureCommandRequest : IRequest<string?>
    {
        public Guid UserID { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }

    public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommandRequest, string?>
    {
        private readonly IUserRepository _userRepository;

        public UpdateProfilePictureCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string?> Handle(UpdateProfilePictureCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null) return null;

            user.ProfilePictureUrl = request.ProfilePictureUrl;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return user.ProfilePictureUrl;
        }
    }
}
