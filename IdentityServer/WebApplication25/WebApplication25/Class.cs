using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication25
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _someFilterParameter;

        public CustomAuthorizeAttribute(string someFilterParameter)
        {
            _someFilterParameter = someFilterParameter;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                // it isn't needed to set unauthorized result 
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
               // return;
            }

            var jwt = context.HttpContext.Request.Headers["Authorization"].ToString();
            jwt = jwt.Replace("bearer ", "", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return;
            }

          //  jwt = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjI4NzAzZTkwZWIyM2Q2ZGRlMGE1YmQ5NzJlZDIwYjNkIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1NzU2NzE0ODQsImV4cCI6MTU3NTY3NTA4NCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1NzAyMiIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjU3MDIyL3Jlc291cmNlcyIsImZpdmVyX2F1dGhfYXBpIl0sImNsaWVudF9pZCI6ImZpdmVyX2F1dGhfY2xpZW50X3JvIiwic3ViIjoiamFtZXMiLCJhdXRoX3RpbWUiOjE1NzU2NzE0ODQsImlkcCI6ImxvY2FsIiwicm9sZSI6ImFkbWluUm9sZSIsInJvbGVpZCI6IjllYmUyOTdlLTAzZDUtNDFjZS04OTNkLWJmMmVkZjU0N2MzMyIsInNjb3BlIjpbImZpdmVyX2F1dGhfYXBpIl0sImFtciI6WyJwd2QiXX0.uI7qnlPFDLOSvnRy7N44mrMCPEYrFl7ozX39Q-JdaTWRvzFVdAXiB4BfyeNtjuOsuwMxxpnq59H2Fz2IHpHWxeIpSKAKSs4AHl_fBFrxk827W9f_f7WeWsoCSBfV23pDFVCXtM5_eA5DBCib1EoAMY7YGrf7GBN7q-PIEuTzo4sDgzXtU0q8IxnihrmFL0Zz9bEWFNkUKW07hOFdpXdNDK3AAjXI_xiRjMS0ya0Pd7a-F_xP9-1o2L-pJ8nzsrOkRQD6rIB4hk1ZKH5DQztDQn9H0jrspImjmFbzQzleGpH96ZcuXYUGZbfAnCvUkgWBM06E4-_b4XTpWOQ_-ARG_w";
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);


            var tt = "dfsdfd";
           context.Result =  new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            return;

            // you can also use registered services
            //var someService = context.HttpContext.RequestServices.GetService<ISomeService>();

            //var isAuthorized = someService.IsUserAuthorized(user.Identity.Name, _someFilterParameter);
            //if (!isAuthorized)
            //{
            //    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            //    return;
            //}
        }
    }
}
