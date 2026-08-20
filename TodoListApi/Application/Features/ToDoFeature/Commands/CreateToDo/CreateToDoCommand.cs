using System;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Application.Repositories;
using TodoListApi.Domain.Entities;

namespace TodoListApi.Application.Features.ToDoFeature.Commands.CreateToDo
{
    public class CreateToDoCommandRequest : MediatR.IRequest<CreateToDoCommandResponse>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CreateToDoCommandResponse
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateToDoCommandHandler : MediatR.IRequestHandler<CreateToDoCommandRequest, CreateToDoCommandResponse>
    {
        private readonly IToDoRepository _toDoRepository;

        public CreateToDoCommandHandler(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<CreateToDoCommandResponse> Handle(CreateToDoCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = new ToDo
            {
                ID = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            await _toDoRepository.AddAsync(todo, cancellationToken);
            await _toDoRepository.SaveChangesAsync(cancellationToken);

            return new CreateToDoCommandResponse
            {
                ID = todo.ID,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            };
        }
    }
}
