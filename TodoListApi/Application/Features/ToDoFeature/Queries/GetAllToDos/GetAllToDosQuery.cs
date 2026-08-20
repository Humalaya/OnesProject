using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Application.Repositories;
using TodoListApi.Domain.Entities;

namespace TodoListApi.Application.Features.ToDoFeature.Queries.GetAllToDos
{
    public class GetAllToDosQueryRequest : MediatR.IRequest<IEnumerable<GetAllToDosQueryResponse>>
    {
    }

    public class GetAllToDosQueryResponse
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetAllToDosQueryHandler : MediatR.IRequestHandler<GetAllToDosQueryRequest, IEnumerable<GetAllToDosQueryResponse>>
    {
        private readonly IToDoRepository _toDoRepository;

        public GetAllToDosQueryHandler(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<IEnumerable<GetAllToDosQueryResponse>> Handle(GetAllToDosQueryRequest request, CancellationToken cancellationToken)
        {
            var todos = await _toDoRepository.GetAllAsync(cancellationToken);
            var response = new List<GetAllToDosQueryResponse>();
            foreach (var todo in todos)
            {
                response.Add(new GetAllToDosQueryResponse
                {
                    ID = todo.ID,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt
                });
            }
            return response;
        }
    }
}
