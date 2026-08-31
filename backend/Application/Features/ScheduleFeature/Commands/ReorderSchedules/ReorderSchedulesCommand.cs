using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ScheduleFeature.Commands.ReorderSchedules
{
    public class ReorderSchedulesCommandRequest : IRequest<bool>
    {
        public Guid ToDoID { get; set; }
        public Guid UserID { get; set; }
        public List<Guid> OrderedScheduleIDs { get; set; } = new();
    }

    public class ReorderSchedulesCommandHandler : IRequestHandler<ReorderSchedulesCommandRequest, bool>
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public ReorderSchedulesCommandHandler(IToDoRepository toDoRepository, IScheduleRepository scheduleRepository)
        {
            _toDoRepository = toDoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<bool> Handle(ReorderSchedulesCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ToDoID, request.UserID, cancellationToken);
            if (todo == null) return false;

            var schedules = await _scheduleRepository.GetByIdsAsync(request.ToDoID, request.OrderedScheduleIDs, cancellationToken);
            var byId = schedules.ToDictionary(s => s.ID);

            for (var i = 0; i < request.OrderedScheduleIDs.Count; i++)
            {
                if (byId.TryGetValue(request.OrderedScheduleIDs[i], out var schedule))
                {
                    schedule.Order = i;
                }
            }

            await _scheduleRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
