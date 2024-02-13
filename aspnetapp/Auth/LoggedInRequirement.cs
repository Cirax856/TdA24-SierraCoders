using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspnetapp.Auth
{
    public class LoggedInRequirement : IAuthorizationRequirement
    {
    }
}