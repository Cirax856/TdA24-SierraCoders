using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspnetapp.Auth
{
    // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iard?view=aspnetcore-8.0
    public class LoggedInAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement/*, IAuthorizationRequirementData*/
    {
        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}