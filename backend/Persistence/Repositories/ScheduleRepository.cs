using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Application.Repositories;
using backend.Domain.Entities;
using backend.Persistence.Contexts;

namespace backend.Persistence.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly TodoDbContext _context;

        public ScheduleRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule>> GetForToDoAsync(Guid toDoId, CancellationToken cancellationToken = default)
        {
            return await _context.Schedules
                .AsNoTracking()
                .Where(s => s.ToDoID == toDoId)
                .OrderBy(s => s.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetMaxOrderAsync(Guid toDoId, CancellationToken cancellationToken = default)
        {
            var any = await _context.Schedules.AnyAsync(s => s.ToDoID == toDoId, cancellationToken);
            if (!any) return -1;
            return await _context.Schedules.Where(s => s.ToDoID == toDoId).MaxAsync(s => s.Order, cancellationToken);
        }

        public async Task<int> GetCountAsync(Guid toDoId, CancellationToken cancellationToken = default)
        {
            return await _context.Schedules.CountAsync(s => s.ToDoID == toDoId, cancellationToken);
        }

        public async Task<DateTime?> GetNextScheduledAtAsync(Guid toDoId, CancellationToken cancellationToken = default)
        {
            return await _context.Schedules
                .Where(s => s.ToDoID == toDoId)
                .OrderBy(s => s.ScheduledAt)
                .Select(s => (DateTime?)s.ScheduledAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Dictionary<Guid, (int Count, DateTime? NextScheduledAt)>> GetSummariesAsync(IEnumerable<Guid> toDoIds, CancellationToken cancellationToken = default)
        {
            var ids = toDoIds.ToList();
            var rows = await _context.Schedules
                .AsNoTracking()
                .Where(s => ids.Contains(s.ToDoID))
                .GroupBy(s => s.ToDoID)
                .Select(g => new { ToDoID = g.Key, Count = g.Count(), NextScheduledAt = g.Min(s => s.ScheduledAt) })
                .ToListAsync(cancellationToken);

            return rows.ToDictionary(r => r.ToDoID, r => (r.Count, (DateTime?)r.NextScheduledAt));
        }

        public async Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default)
        {
            await _context.Schedules.AddAsync(schedule, cancellationToken);
        }

        public async Task<List<Schedule>> GetByIdsAsync(Guid toDoId, IEnumerable<Guid> scheduleIds, CancellationToken cancellationToken = default)
        {
            var ids = scheduleIds.ToList();
            return await _context.Schedules
                .Where(s => s.ToDoID == toDoId && ids.Contains(s.ID))
                .ToListAsync(cancellationToken);
        }

        public void RemoveRange(IEnumerable<Schedule> schedules)
        {
            _context.Schedules.RemoveRange(schedules);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
