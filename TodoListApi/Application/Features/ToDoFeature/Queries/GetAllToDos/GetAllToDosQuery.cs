using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoListApi.Application.Repositories;

namespace TodoListApi.Application.Features.ToDoFeature.Queries.GetAllToDos
{
    public class GetAllToDosQueryRequest : MediatR.IRequest<GetAllToDosQueryResponse>
    {
        public Guid UserID { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        private static readonly int[] AllowedPageSizes = { 5, 10, 20 };

        public int NormalizedPageSize => Array.IndexOf(AllowedPageSizes, PageSize) >= 0 ? PageSize : 10;
        public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;
    }

    public class ToDoListItem
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetAllToDosQueryResponse
    {
        public IEnumerable<ToDoListItem> Items { get; set; } = Enumerable.Empty<ToDoListItem>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllToDosQueryHandler : MediatR.IRequestHandler<GetAllToDosQueryRequest, GetAllToDosQueryResponse>
    {
        private readonly IToDoRepository _toDoRepository;

        public GetAllToDosQueryHandler(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<GetAllToDosQueryResponse> Handle(GetAllToDosQueryRequest request, CancellationToken cancellationToken)
        {
            var pageNumber = request.NormalizedPageNumber;
            var pageSize = request.NormalizedPageSize;

            var (items, totalCount) = await _toDoRepository.GetPagedAsync(request.UserID, pageNumber, pageSize, cancellationToken);

            return new GetAllToDosQueryResponse
            {
                Items = items.Select(todo => new ToDoListItem
                {
                    ID = todo.ID,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt
                }),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
