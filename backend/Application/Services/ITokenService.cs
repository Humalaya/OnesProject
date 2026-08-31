using backend.Domain.Entities;

namespace backend.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
