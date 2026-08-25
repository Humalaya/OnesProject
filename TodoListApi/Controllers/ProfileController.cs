using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Application.Features.ProfileFeature.Commands.ChangePassword;
using TodoListApi.Application.Features.ProfileFeature.Commands.UpdateProfilePicture;
using TodoListApi.Application.Features.ProfileFeature.Commands.UpdateUsername;
using TodoListApi.Application.Features.ProfileFeature.Queries.GetProfile;

namespace TodoListApi.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediator.Send(new GetProfileQueryRequest { UserID = this.GetUserId() });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameCommandRequest request)
        {
            request.UserID = this.GetUserId();
            try
            {
                var result = await _mediator.Send(request);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommandRequest request)
        {
            request.UserID = this.GetUserId();
            try
            {
                var result = await _mediator.Send(request);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("picture")]
        public async Task<IActionResult> UpdatePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "File is too large. Maximum size is 5MB." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (Array.IndexOf(AllowedExtensions, extension) < 0)
                return BadRequest(new { message = "Unsupported file type. Use JPG, PNG, GIF or WEBP." });

            var userId = this.GetUserId();
            var uploadsRoot = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads");
            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{userId}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/{fileName}";
            var savedUrl = await _mediator.Send(new UpdateProfilePictureCommandRequest { UserID = userId, ProfilePictureUrl = url });
            if (savedUrl == null) return NotFound();

            return Ok(new { profilePictureUrl = savedUrl });
        }
    }
}
