using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace aspnetapp.Auth
{
    public class LoggedInAuthorizationHandler : AuthorizationHandler<LoggedInAuthorizeAttribute>
    {
        private readonly ILogger<LoggedInAuthorizationHandler> _logger;

        public LoggedInAuthorizationHandler(ILogger<LoggedInAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        // Check whether a given MinimumAgeRequirement is satisfied or not for a particular
        // context.
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoggedInAuthorizeAttribute requirement)
        {
            // Log as a warning so that it's very clear in sample output which authorization
            // policies(and requirements/handlers) are in use.
            _logger.LogWarning("Evaluating authorization");

            Claim authenticationClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Authentication);
            if (authenticationClaim != null)
            {
                _logger.LogInformation("Authentication claim value: " + authenticationClaim.Value);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "No Authentication claim present"));
                _logger.LogInformation("No Authentication claim present");
            }

            return Task.CompletedTask;
        }
    }
}