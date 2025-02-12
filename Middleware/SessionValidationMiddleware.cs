using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Services;

namespace WebApplication1.Middleware
{
    // SessionValidationMiddleware.cs
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, SessionService sessionService)
        {
            var userId = context.Session.GetString("UserId");
            var sessionId = context.Session.GetString("SessionId");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sessionId))
            {
                if (!sessionService.ValidateSession(userId, sessionId))
                {
                    // Session is invalid - force logout
                    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                    context.Session.Clear();
                    context.Response.Redirect("/Login");
                    return;
                }
            }

            await _next(context);
        }
}

    }
