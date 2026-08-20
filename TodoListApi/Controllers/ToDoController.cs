using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Application.Features.ToDoFeature.Commands.CreateToDo;
using TodoListApi.Application.Features.ToDoFeature.Commands.DeleteToDo;
using TodoListApi.Application.Features.ToDoFeature.Commands.UpdateToDo;
using TodoListApi.Application.Features.ToDoFeature.Queries.GetAllToDos;
using TodoListApi.Application.Features.ToDoFeature.Queries.GetByIdToDo;

namespace TodoListApi.Controllers
{
    [Route("api/todo")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ToDoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllToDosQueryRequest();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var query = new GetByIdToDoQueryRequest { ID = id };
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateToDoCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateToDoCommandRequest request)
        {
            request.ID = id;
            var result = await _mediator.Send(request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteToDoCommandRequest { ID = id };
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
