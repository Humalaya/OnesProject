using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using backend.Application.Repositories;
using backend.Domain.Entities;

namespace backend.Application.Features.ScheduleFeature.Commands.CreateSchedule
{
    public class CreateScheduleCommandRequest : IRequest<CreateScheduleCommandResponse?>
    {
        public Guid ToDoID { get; set; }
        public Guid UserID { get; set; }
        public DateTime ScheduledAt { get; set; }
    }

    public class CreateScheduleCommandResponse
    {
        public Guid ID { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int Order { get; set; }
    }

    public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommandRequest, CreateScheduleCommandResponse?>
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public CreateScheduleCommandHandler(IToDoRepository toDoRepository, IScheduleRepository scheduleRepository)
        {
            _toDoRepository = toDoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<CreateScheduleCommandResponse?> Handle(CreateScheduleCommandRequest request, CancellationToken cancellationToken)
        {
            var todo = await _toDoRepository.GetByIdAsync(request.ToDoID, request.UserID, cancellationToken);
            if (todo == null) return null;

            var maxOrder = await _scheduleRepository.GetMaxOrderAsync(request.ToDoID, cancellationToken);

            var schedule = new Schedule
            {
                ID = Guid.NewGuid(),
                ToDoID = request.ToDoID,
                ScheduledAt = request.ScheduledAt,
                Order = maxOrder + 1
            };

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            await _scheduleRepository.SaveChangesAsync(cancellationToken);

            return new CreateScheduleCommandResponse
            {
                ID = schedule.ID,
                ScheduledAt = schedule.ScheduledAt,
                Order = schedule.Order
            };
        }
    }
}
