using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Application.Repositories;
using TodoListApi.Domain.Entities;
using TodoListApi.Persistence.Contexts;

namespace TodoListApi.Persistence.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly TodoDbContext _context;

        public ToDoRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<ToDo> Items, int TotalCount)> GetPagedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.ToDos.AsNoTracking().Where(t => t.UserID == userId).OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<ToDo?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var todo = await _context.ToDos.FindAsync(new object[] { id }, cancellationToken);
            return todo != null && todo.UserID == userId ? todo : null;
        }

        public async Task AddAsync(ToDo todo, CancellationToken cancellationToken = default)
        {
            await _context.ToDos.AddAsync(todo, cancellationToken);
        }

        public void Update(ToDo todo)
        {
            _context.ToDos.Update(todo);
        }

        public void Delete(ToDo todo)
        {
            _context.ToDos.Remove(todo);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
