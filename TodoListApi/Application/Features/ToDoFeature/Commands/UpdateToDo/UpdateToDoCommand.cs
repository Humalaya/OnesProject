using System;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Application.Repositories;

namespace TodoListApi.Application.Features.ToDoFeature.Commands.UpdateToDo
{
    public class UpdateToDoCommandRequest : MediatR.IRequest<bool>
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class UpdateToDoCommandHandler : MediatR.IRequestHandler<UpdateToDoCommandRequest, bool>
    {
        private readonly IToDoRepository _toDoRepository;

        public UpdateToDoCommandHandler(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<bool> Handle(UpdateToDoCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ID, request.UserID, cancellationToken);
            if (todo == null) return false;

            todo.Title = request.Title;
            todo.Description = request.Description;
            todo.IsCompleted = request.IsCompleted;

            _toDoRepository.Update(todo);
            await _toDoRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
