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

        public async Task<IEnumerable<ToDo>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ToDos.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ToDos.FindAsync(new object[] { id }, cancellationToken);
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
