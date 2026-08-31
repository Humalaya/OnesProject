using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ScheduleFeature.Commands.DeleteSchedules
{
    public class DeleteSchedulesCommandRequest : IRequest<bool>
    {
        public Guid ToDoID { get; set; }
        public Guid UserID { get; set; }
        public List<Guid> ScheduleIDs { get; set; } = new();
    }

    public class DeleteSchedulesCommandHandler : IRequestHandler<DeleteSchedulesCommandRequest, bool>
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public DeleteSchedulesCommandHandler(IToDoRepository toDoRepository, IScheduleRepository scheduleRepository)
        {
            _toDoRepository = toDoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<bool> Handle(DeleteSchedulesCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ToDoID, request.UserID, cancellationToken);
            if (todo == null) return false;

            if (request.ScheduleIDs.Count == 0) return true;

            var schedules = await _scheduleRepository.GetByIdsAsync(request.ToDoID, request.ScheduleIDs, cancellationToken);
            _scheduleRepository.RemoveRange(schedules);
            await _scheduleRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
