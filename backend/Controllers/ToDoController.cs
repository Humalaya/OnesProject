using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Application.Features.ToDoFeature.Commands.CreateToDo;
using backend.Application.Features.ToDoFeature.Commands.DeleteToDo;
using backend.Application.Features.ToDoFeature.Commands.UpdateToDo;
using backend.Application.Features.ToDoFeature.Queries.GetAllToDos;
using backend.Application.Features.ToDoFeature.Queries.GetByIdToDo;

namespace backend.Controllers
{
    [Route("api/todo")]
    [ApiController]
    [Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ToDoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "newest",
            [FromQuery] string filter = "all")
        {
            var query = new GetAllToDosQueryRequest
            {
                UserID = this.GetUserId(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                Filter = filter
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var query = new GetByIdToDoQueryRequest { ID = id, UserID = this.GetUserId() };
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateToDoCommandRequest request)
        {
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateToDoCommandRequest request)
        {
            request.ID = id;
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteToDoCommandRequest { ID = id, UserID = this.GetUserId() };
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
