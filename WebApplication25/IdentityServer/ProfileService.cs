
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace SVM.Enterprise.Users.Infrastructure.IdentityServer
{
    public class ProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var principal = await CreateAsync("user1");

            if (principal == null)
            {
                throw new Exception("ClaimsFactory failed to create a principal");
            }

            context.AddRequestedClaims(principal.Claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }

        public async Task<ClaimsPrincipal> CreateAsync(string user)
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(JwtClaimTypes.Subject, "userId".ToString()));
            identity.AddClaim(new Claim("role", "adminRole"));
            identity.AddClaim(new Claim("roleid", Guid.NewGuid().ToString()));


            //if (!identity.HasClaim(x => x.Type == JwtClaimTypes.Subject))
            //{
            //    var subjectId = await _userService.GetUserIdAsync(user);
            //    identity.AddClaim(new Claim(JwtClaimTypes.Subject, subjectId.ToString()));
            //}

            //var userName = await _userService.GetUserNameAsync(user);
            //var userNameClaim = identity.FindFirst(claim => claim.Type == ClaimTypes.Name && claim.Value == userName);
            //if (userNameClaim != null)
            //{
            //    identity.RemoveClaim(userNameClaim);
            //    identity.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, userName));
            //}

            //if (!identity.HasClaim(x => x.Type == JwtClaimTypes.Name))
            //{
            //    identity.AddClaim(new Claim(JwtClaimTypes.Name, userName));
            //}

            //if (!identity.HasClaim(x => x.Type == "UserId"))
            //{
            //    identity.AddClaim(new Claim("UserId", user.UserId.ToString()));
            //}


            //if (!identity.HasClaim(x => x.Type == "Platform"))
            //{
            //    identity.AddClaim(new Claim("Platform", user.Platform));
            //}


            //if (!identity.HasClaim(x => x.Type == "RoleName"))
            //{
            //    var roleDetail = await _userService.GetUserRoleAsync(user.Username);
            //    if (roleDetail != null)
            //    {
            //        var roleNames = Join(",", roleDetail.Roles.Select(g => g.RoleName).Distinct().ToArray());
            //        var roleids = Join(",", roleDetail.Roles.Select(g => g.RoleId.ToString()).Distinct().ToArray());
            //        identity.AddClaim(new Claim("RoleName", roleNames));
            //        identity.AddClaim(new Claim("RoleId", roleids));
            //    }
            //}
            //var email = await _userService.GetEmailAsync(user);
            //if (!IsNullOrWhiteSpace(email))
            //{
            //    identity.AddClaims(new[]
            //    {
            //        new Claim(JwtClaimTypes.Email, email),
            //        new Claim(JwtClaimTypes.EmailVerified,
            //            await _userService.IsEmailConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
            //    });
            //}
            return new ClaimsPrincipal(identity);
        }
    }
}