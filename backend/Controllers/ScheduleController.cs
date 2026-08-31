using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Application.Features.ScheduleFeature.Commands.CreateSchedule;
using backend.Application.Features.ScheduleFeature.Commands.DeleteSchedules;
using backend.Application.Features.ScheduleFeature.Commands.ReorderSchedules;
using backend.Application.Features.ScheduleFeature.Commands.UpdateSchedule;
using backend.Application.Features.ScheduleFeature.Queries.GetSchedules;

namespace backend.Controllers
{
    [Route("api/todo/{todoId}/schedules")]
    [ApiController]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute] Guid todoId)
        {
            var result = await _mediator.Send(new GetSchedulesQueryRequest { ToDoID = todoId, UserID = this.GetUserId() });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] Guid todoId, [FromBody] CreateScheduleCommandRequest request)
        {
            request.ToDoID = todoId;
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> Update([FromRoute] Guid todoId, [FromRoute] Guid scheduleId, [FromBody] UpdateScheduleCommandRequest request)
        {
            request.ToDoID = todoId;
            request.ScheduleID = scheduleId;
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromRoute] Guid todoId, [FromBody] DeleteSchedulesCommandRequest request)
        {
            request.ToDoID = todoId;
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> Reorder([FromRoute] Guid todoId, [FromBody] ReorderSchedulesCommandRequest request)
        {
            request.ToDoID = todoId;
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
