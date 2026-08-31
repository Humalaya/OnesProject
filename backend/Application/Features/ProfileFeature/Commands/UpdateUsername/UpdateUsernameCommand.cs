using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ProfileFeature.Commands.UpdateUsername
{
    public class UpdateUsernameCommandRequest : IRequest<bool>
    {
        public Guid UserID { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommandRequest, bool>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUsernameCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UpdateUsernameCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null) return false;

            if (await _userRepository.UsernameExistsAsync(request.Username, request.UserID, cancellationToken))
                throw new InvalidOperationException("This username is already taken.");

            user.Username = request.Username;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
