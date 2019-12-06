
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace SVM.Enterprise.Users.Infrastructure.IdentityServer
{
    public class DbResourceStore : IResourceStore
    {
        private readonly IList<IdentityResource> _identityResources = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };


        private readonly IList<ApiResource> _apiResources = new List<ApiResource>
            {
                new ApiResource("fiver_auth_api", "Fiver.Security.AuthServer.Api")
                 //   new[] { JwtClaimTypes.Subject, JwtClaimTypes.Email,"role" , "roleid"})
                {
                    UserClaims = {"role", "roleid"}
                }
            };

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return _apiResources;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            //need to check
            return _identityResources;

        }

        public async Task<Resources> GetAllResourcesAsync()
        {
          

            return new Resources(_identityResources, _apiResources);
        }
    }
}