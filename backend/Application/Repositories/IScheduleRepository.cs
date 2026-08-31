using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;

namespace backend.Application.Repositories
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetForToDoAsync(Guid toDoId, CancellationToken cancellationToken = default);
        Task<int> GetMaxOrderAsync(Guid toDoId, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(Guid toDoId, CancellationToken cancellationToken = default);
        Task<DateTime?> GetNextScheduledAtAsync(Guid toDoId, CancellationToken cancellationToken = default);
        Task<Dictionary<Guid, (int Count, DateTime? NextScheduledAt)>> GetSummariesAsync(IEnumerable<Guid> toDoIds, CancellationToken cancellationToken = default);
        Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default);
        Task<List<Schedule>> GetByIdsAsync(Guid toDoId, IEnumerable<Guid> scheduleIds, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<Schedule> schedules);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
