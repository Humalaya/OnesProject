using TodoListApi.Domain.Entities;

namespace TodoListApi.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
