using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;

namespace backend.Application.Repositories
{
    public interface IToDoRepository
    {
        Task<(IEnumerable<ToDo> Items, int TotalCount)> GetPagedAsync(Guid userId, int pageNumber, int pageSize, string sortBy, string filter, CancellationToken cancellationToken = default);
        Task<ToDo?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(ToDo todo, CancellationToken cancellationToken = default);
        void Update(ToDo todo);
        void Delete(ToDo todo);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
