using System;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Domain.Entities;

namespace TodoListApi.Application.Repositories
{
    public interface IToDoRepository
    {
        Task<IEnumerable<ToDo>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(ToDo todo, CancellationToken cancellationToken = default);
        void Update(ToDo todo);
        void Delete(ToDo todo);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
