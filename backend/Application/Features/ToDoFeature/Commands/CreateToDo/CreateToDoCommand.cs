using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Application.Common;
using backend.Application.Repositories;
using backend.Domain.Entities;

namespace backend.Application.Features.ToDoFeature.Commands.CreateToDo
{
    public class CreateToDoCommandRequest : MediatR.IRequest<CreateToDoCommandResponse>
    {
        public Guid UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public string Priority { get; set; } = "medium";
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    public class CreateToDoCommandResponse
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public string Priority { get; set; } = "medium";
        public string[] Tags { get; set; } = Array.Empty<string>();
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
                UserID = request.UserID,
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                Priority = ToDoFieldMapper.PriorityToInt(request.Priority),
                Tags = ToDoFieldMapper.TagsToString(request.Tags),
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
                Priority = ToDoFieldMapper.PriorityToString(todo.Priority),
                Tags = ToDoFieldMapper.TagsToArray(todo.Tags),
                CreatedAt = todo.CreatedAt
            };
        }
    }
}
