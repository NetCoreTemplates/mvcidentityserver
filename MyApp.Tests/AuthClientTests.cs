using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Text;

namespace MyApp.Tests
{
    public class AuthClientTests
    {
        [Test]
        public async Task Client_can_call_protected_services_with_ClientId_and_ClientSecret()
        {
            var client = new HttpClient();

            // discover endpoints from metadata
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine($"TokenResponse:\n{tokenResponse.Json}\n\n");

            await CallApi(tokenResponse.AccessToken);
        }
        
        [Test]
        public async Task Client_can_call_protected_services_with_Username_and_Password()
        {
            // Resource Owner Client Example
            var client = new HttpClient();

            // discover endpoints from metadata
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ro.client",
                ClientSecret = "secret",

                UserName = "alice",
                Password = "password",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine($"TokenResponse:\n{tokenResponse.Json}\n\n");

            await CallApi(tokenResponse.AccessToken);
        }
        
        private static async Task CallApi(string accessToken)
        {
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(accessToken);
            
            // Web API (JSON)
            var response = await apiClient.GetAsync("https://localhost:5001/webapi-identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Web API (JSON):\n{json}");
                Console.WriteLine(JArray.Parse(json));
            }

            // ServiceStack API (JSON)
            apiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(MimeTypes.Json)); // or call /servicestack-identity.json
            response = await apiClient.GetAsync("https://localhost:5001/servicestack-identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var dto = json.FromJson<GetIdentityResponse>();
                Console.WriteLine($"ServiceStack (JSON):\n{dto.Dump()}");
            }

            // ServiceStack Service Client (Typed DTOs)
            var serviceClient = new JsonServiceClient("https://localhost:5001/") {
                BearerToken = accessToken
            };

            var res = await serviceClient.GetAsync(new GetIdentity());
            Console.WriteLine($"ServiceStack Service Client:\n{res.Dump()}");
        }
    }
}