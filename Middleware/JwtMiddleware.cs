using Crime_Management_System.Servises;
using Microsoft.Extensions.Primitives;

namespace Crime_Management_System.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, JwtService jwtService)
        {
            // Extract token from Authorization header
            var token = ExtractTokenFromHeader(context);

            if (!string.IsNullOrEmpty(token))
            {
                // Validate token and set user context
                var principal = jwtService.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                }
            }

            await _next(context);
        }

        private string ExtractTokenFromHeader(HttpContext context)
        {
            // Check Authorization header
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            {
                var headerValue = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(headerValue) && headerValue.StartsWith("Bearer "))
                {
                    return headerValue.Substring("Bearer ".Length).Trim();
                }
            }

            return null;
        }
    }
}