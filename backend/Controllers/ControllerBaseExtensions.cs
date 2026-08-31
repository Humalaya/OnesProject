using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public static class ControllerBaseExtensions
    {
        public static Guid GetUserId(this ControllerBase controller)
        {
            var value = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(value!);
        }
    }
}
