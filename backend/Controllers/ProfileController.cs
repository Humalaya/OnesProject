using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Application.Features.ProfileFeature.Commands.ChangePassword;
using backend.Application.Features.ProfileFeature.Commands.ResendVerification;
using backend.Application.Features.ProfileFeature.Commands.UpdateFullName;
using backend.Application.Features.ProfileFeature.Commands.UpdateProfilePicture;
using backend.Application.Features.ProfileFeature.Commands.UpdateUsername;
using backend.Application.Features.ProfileFeature.Queries.GetProfile;

namespace backend.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private static readonly (string Extension, string MimeType)[] AllowedTypes =
        {
            (".jpg", "image/jpeg"),
            (".jpeg", "image/jpeg"),
            (".png", "image/png"),
            (".gif", "image/gif"),
            (".webp", "image/webp")
        };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
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

        [HttpPut("fullname")]
        public async Task<IActionResult> UpdateFullName([FromBody] UpdateFullNameCommandRequest request)
        {
            request.UserID = this.GetUserId();
            var result = await _mediator.Send(request);
            if (!result) return NotFound();
            return NoContent();
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

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification()
        {
            var result = await _mediator.Send(new ResendVerificationCommandRequest { UserID = this.GetUserId() });
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("picture")]
        public async Task<IActionResult> UpdatePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "File is too large. Maximum size is 5MB." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedType = Array.Find(AllowedTypes, t => t.Extension == extension);
            if (allowedType.Extension == null)
                return BadRequest(new { message = "Unsupported file type. Use JPG, PNG, GIF or WEBP." });

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            var dataUri = $"data:{allowedType.MimeType};base64,{base64}";

            var userId = this.GetUserId();
            var savedUrl = await _mediator.Send(new UpdateProfilePictureCommandRequest { UserID = userId, ProfilePictureUrl = dataUri });
            if (savedUrl == null) return NotFound();

            return Ok(new { profilePictureUrl = savedUrl });
        }
    }
}
