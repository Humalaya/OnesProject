using System;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Application.Repositories;

namespace TodoListApi.Application.Features.ToDoFeature.Queries.GetByIdToDo
{
    public class GetByIdToDoQueryRequest : MediatR.IRequest<GetByIdToDoQueryResponse>
    {
        public Guid ID { get; set; }
    }

    public class GetByIdToDoQueryResponse
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetByIdToDoQueryHandler : MediatR.IRequestHandler<GetByIdToDoQueryRequest, GetByIdToDoQueryResponse?>
    {
        private readonly IToDoRepository _toDoRepository;

        public GetByIdToDoQueryHandler(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<GetByIdToDoQueryResponse?> Handle(GetByIdToDoQueryRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ID, cancellationToken);
            if (todo == null) return null;

            return new GetByIdToDoQueryResponse
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
