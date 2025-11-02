using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Attributes
{
    
        // Attributes/AuthorizeRolesAttribute.cs
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
        public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
        {
            private readonly string[] _allowedRoles;

            public AuthorizeRolesAttribute(params string[] roles)
            {
                _allowedRoles = roles;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                // Skip authorization if action is decorated with [AllowAnonymous]
                if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
                    return;

                var user = context.HttpContext.User;
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return;
                }

                // Check if user has any of the required roles
                var hasRequiredRole = _allowedRoles.Any(role => user.IsInRole(role));
                if (!hasRequiredRole)
                {
                    context.Result = new JsonResult(new { message = "Forbidden - Insufficient permissions" })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }
            }
        }

        // Attributes/AllowAnonymousAttribute.cs
        [AttributeUsage(AttributeTargets.Method)]
        public class AllowAnonymousAttribute : Attribute { }

        // Attributes/ClearanceLevelAttribute.cs
        [AttributeUsage(AttributeTargets.Method)]
        public class ClearanceLevelAttribute : Attribute, IAuthorizationFilter
        {
            private readonly string _requiredLevel;

            public ClearanceLevelAttribute(string requiredLevel)
            {
                _requiredLevel = requiredLevel;
            }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var userClearance = user.FindFirst("ClearanceLevel")?.Value;

        
            Console.WriteLine($"User clearance from token: '{userClearance}', Required: '{_requiredLevel}'");

            if (!HasSufficientClearance(userClearance, _requiredLevel))
            {
                context.Result = new JsonResult(new { message = "Forbidden - Insufficient clearance level" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }

        private bool HasSufficientClearance(string userLevel, string requiredLevel)
        {
            if (string.IsNullOrEmpty(userLevel) || string.IsNullOrEmpty(requiredLevel))
                return false; 

            var levels = new Dictionary<string, int>
            {
                ["low"] = 1,
                ["medium"] = 2,
                ["high"] = 3,
                ["critical"] = 4
            };

            if (!levels.TryGetValue(userLevel.ToLower(), out int userValue))
                return false;

            if (!levels.TryGetValue(requiredLevel.ToLower(), out int requiredValue))
                return false;

            return userValue >= requiredValue;
        }

    }
}

