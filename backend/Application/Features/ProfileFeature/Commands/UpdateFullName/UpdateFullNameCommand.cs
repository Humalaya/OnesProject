using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ProfileFeature.Commands.UpdateFullName
{
    public class UpdateFullNameCommandRequest : IRequest<bool>
    {
        public Guid UserID { get; set; }
        public string? FullName { get; set; }
    }

    public class UpdateFullNameCommandHandler : IRequestHandler<UpdateFullNameCommandRequest, bool>
    {
        private readonly IUserRepository _userRepository;

        public UpdateFullNameCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UpdateFullNameCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null) return false;

            user.FullName = request.FullName;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
