using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Application.Features.AuthFeature.Commands.Login;
using backend.Application.Features.AuthFeature.Commands.Register;
using backend.Application.Features.AuthFeature.Commands.VerifyEmail;

namespace backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private static readonly Regex EmailPattern = new(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || !EmailPattern.IsMatch(request.Email))
                return BadRequest(new { message = "Please provide a valid email address." });

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

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest(new { message = "Missing verification token." });

            var result = await _mediator.Send(new VerifyEmailCommandRequest { Token = token });

            return result switch
            {
                VerifyEmailResult.Success => Ok(new { message = "Email verified." }),
                VerifyEmailResult.Expired => BadRequest(new { message = "This verification link has expired. Request a new one from your profile." }),
                _ => BadRequest(new { message = "Invalid verification link." })
            };
        }
    }
}
