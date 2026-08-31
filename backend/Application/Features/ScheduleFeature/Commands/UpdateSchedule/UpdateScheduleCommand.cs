using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;

namespace backend.Application.Features.ScheduleFeature.Commands.UpdateSchedule
{
    public class UpdateScheduleCommandRequest : IRequest<bool>
    {
        public Guid ToDoID { get; set; }
        public Guid ScheduleID { get; set; }
        public Guid UserID { get; set; }
        public DateTime ScheduledAt { get; set; }
    }

    public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommandRequest, bool>
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public UpdateScheduleCommandHandler(IToDoRepository toDoRepository, IScheduleRepository scheduleRepository)
        {
            _toDoRepository = toDoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<bool> Handle(UpdateScheduleCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ToDoID, request.UserID, cancellationToken);
            if (todo == null) return false;

            var matches = await _scheduleRepository.GetByIdsAsync(request.ToDoID, new[] { request.ScheduleID }, cancellationToken);
            var schedule = matches.Count > 0 ? matches[0] : null;
            if (schedule == null) return false;

            schedule.ScheduledAt = request.ScheduledAt;
            await _scheduleRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
