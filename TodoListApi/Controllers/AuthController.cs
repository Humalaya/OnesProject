using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Application.Features.AuthFeature.Commands.Login;
using TodoListApi.Application.Features.AuthFeature.Commands.Register;

namespace TodoListApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommandRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null) return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }
    }
}
