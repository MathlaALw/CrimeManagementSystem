using Crime_Management_System.Servises;
using Microsoft.Extensions.Primitives;

namespace Crime_Management_System.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider; // inject root provider

        public JwtMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            // create a scoped provider for this request
            using var scope = _serviceProvider.CreateScope();
            var jwtService = scope.ServiceProvider.GetRequiredService<JwtService>();

            // extract token
            var token = ExtractTokenFromHeader(context);
            if (!string.IsNullOrEmpty(token))
            {
                var principal = jwtService.ValidateToken(token);
                if (principal != null)
                    context.User = principal;
            }

            await _next(context);
        }

        private string ExtractTokenFromHeader(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            {
                var headerValue = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(headerValue) && headerValue.StartsWith("Bearer "))
                    return headerValue.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
    }
}
