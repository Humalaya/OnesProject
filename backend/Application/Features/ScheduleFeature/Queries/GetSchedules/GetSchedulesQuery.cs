using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ScheduleFeature.Queries.GetSchedules
{
    public class GetSchedulesQueryRequest : IRequest<List<ScheduleItem>?>
    {
        public Guid ToDoID { get; set; }
        public Guid UserID { get; set; }
    }

    public class ScheduleItem
    {
        public Guid ID { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int Order { get; set; }
    }

    public class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQueryRequest, List<ScheduleItem>?>
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public GetSchedulesQueryHandler(IToDoRepository toDoRepository, IScheduleRepository scheduleRepository)
        {
            _toDoRepository = toDoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<List<ScheduleItem>?> Handle(GetSchedulesQueryRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ToDoID, request.UserID, cancellationToken);
            if (todo == null) return null;

            var schedules = await _scheduleRepository.GetForToDoAsync(request.ToDoID, cancellationToken);
            return schedules.Select(s => new ScheduleItem { ID = s.ID, ScheduledAt = s.ScheduledAt, Order = s.Order }).ToList();
        }
    }
}
