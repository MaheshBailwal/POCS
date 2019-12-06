

using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;

namespace SVM.Enterprise.Users.Infrastructure.IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            context.Result = new GrantValidationResult(context.UserName, OidcConstants.AuthenticationMethods.Password);
            return;
        }
    }
}