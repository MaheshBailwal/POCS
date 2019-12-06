

using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Linq;

namespace SVM.Enterprise.Users.Infrastructure.IdentityServer
{
    public class ClientStore : IClientStore
    {
        public async Task<Client> FindClientByIdAsync(string  clientId)
        {
          var clients =  GetClients();
          return  clients.FirstOrDefault(x => x.ClientId == clientId);
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Hybrid Flow = OpenId Connect + OAuth
                // To use both Identity and Access Tokens
                new Client
                {
                    ClientId = "fiver_auth_client",
                    ClientName = "Fiver.Security.AuthServer.Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowOfflineAccess = true,
                    RequireConsent = false,

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "fiver_auth_api"
                    },
                },
                // Resource Owner Password Flow
                new Client
                {
                    ClientId = "fiver_auth_client_ro",
                    ClientName = "Fiver.Security.AuthServer.Client.RO",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    AllowedScopes =
                    {
                        "fiver_auth_api",
                        /// "role",
                         "roleid"
                          
                    },
                }
            };
        }
    }
}