using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;

namespace Fiver.Security.AuthServer.Client.RO
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread.Sleep(4000);
                // discover endpoints from metadata
               // http://localhost:57022/api/values
                var disco = DiscoveryClient.GetAsync("http://localhost:57022").Result;

                // request token
                var tokenClient = new TokenClient(disco.TokenEndpoint, "fiver_auth_client_ro", "secret");
                var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("james", "password", "fiver_auth_api").Result;

                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }

                //Console.WriteLine(tokenResponse.Json);

                // call api
                var client = new HttpClient();
                client.SetBearerToken(tokenResponse.AccessToken);

                var response = client.GetAsync("http://localhost:64797/home").Result;
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}