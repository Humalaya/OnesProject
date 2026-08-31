using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Application.Common;
using backend.Application.Repositories;

namespace backend.Application.Features.ToDoFeature.Queries.GetAllToDos
{
    public class GetAllToDosQueryRequest : MediatR.IRequest<GetAllToDosQueryResponse>
    {
        public Guid UserID { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "newest";
        public string Filter { get; set; } = "all";

        private static readonly int[] AllowedPageSizes = { 5, 10, 20 };
        private static readonly string[] AllowedSorts = { "newest", "oldest", "priority_asc", "priority_desc" };
        private static readonly string[] AllowedFilters = { "all", "active", "done" };

        public int NormalizedPageSize => Array.IndexOf(AllowedPageSizes, PageSize) >= 0 ? PageSize : 10;
        public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;
        public string NormalizedSortBy => Array.IndexOf(AllowedSorts, SortBy) >= 0 ? SortBy : "newest";
        public string NormalizedFilter => Array.IndexOf(AllowedFilters, Filter) >= 0 ? Filter : "all";
    }

    public class ToDoListItem
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public string Priority { get; set; } = "medium";
        public string[] Tags { get; set; } = Array.Empty<string>();
        public DateTime CreatedAt { get; set; }
        public int ScheduleCount { get; set; }
        public DateTime? NextScheduledAt { get; set; }
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
        private readonly IScheduleRepository _scheduleRepository;

        public GetAllToDosQueryHandler(IToDoRepository toDoRepository, IScheduleRepository scheduleRepository)
        {
            _toDoRepository = toDoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<GetAllToDosQueryResponse> Handle(GetAllToDosQueryRequest request, CancellationToken cancellationToken)
        {
            var pageNumber = request.NormalizedPageNumber;
            var pageSize = request.NormalizedPageSize;
            var sortBy = request.NormalizedSortBy;
            var filter = request.NormalizedFilter;

            var (items, totalCount) = await _toDoRepository.GetPagedAsync(request.UserID, pageNumber, pageSize, sortBy, filter, cancellationToken);
            var itemList = items.ToList();

            var summaries = await _scheduleRepository.GetSummariesAsync(itemList.Select(t => t.ID), cancellationToken);

            return new GetAllToDosQueryResponse
            {
                Items = itemList.Select(todo =>
                {
                    summaries.TryGetValue(todo.ID, out var summary);
                    return new ToDoListItem
                    {
                        ID = todo.ID,
                        Title = todo.Title,
                        Description = todo.Description,
                        IsCompleted = todo.IsCompleted,
                        Priority = ToDoFieldMapper.PriorityToString(todo.Priority),
                        Tags = ToDoFieldMapper.TagsToArray(todo.Tags),
                        CreatedAt = todo.CreatedAt,
                        ScheduleCount = summary.Count,
                        NextScheduledAt = summary.NextScheduledAt
                    };
                }),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
