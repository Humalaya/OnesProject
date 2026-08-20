using System;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Application.Repositories;

namespace TodoListApi.Application.Features.ToDoFeature.Commands.DeleteToDo
{
    public class DeleteToDoCommandRequest : MediatR.IRequest<bool>
    {
        public Guid ID { get; set; }
    }

    public class DeleteToDoCommandHandler : MediatR.IRequestHandler<DeleteToDoCommandRequest, bool>
    {
        private readonly IToDoRepository _toDoRepository;

        public DeleteToDoCommandHandler(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<bool> Handle(DeleteToDoCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ID, cancellationToken);
            if (todo == null) return false;

            _toDoRepository.Delete(todo);
            await _toDoRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
