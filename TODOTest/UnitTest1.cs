using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TODOIT.ViewModel.User;

namespace TODOTest
{
    [TestClass]
    public class UnitTest1
    {
        public const string email = "admin@example.com", password = "Admin1!";


        [TestMethod]
        public async Task MainAsync()
        {
            var client = new HttpClient();

            //await CreateAccountAsync(client, email, password);

            var token = await GetTokenAsync(client, email, password);
            Console.WriteLine("Access token: {0}", token);
            Console.WriteLine();

            var resource = await GetResourceAsync(client, token);
            Console.WriteLine("API response: {0}", resource);

            if (string.IsNullOrEmpty(resource))
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public async Task graphQlTest()
        {
            var httpClient = new HttpClient();

            var queryObject = new
            {
                query = @"query { 
                opinions { 
                comment
                }
            }",
                variables = new { }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:58130/Api/GraphQL/graphql"),
                Content = new StringContent(JsonConvert.SerializeObject(queryObject), Encoding.UTF8, "application/json")
            };

            var token = await GetTokenAsync(httpClient, email, password);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            dynamic responseObj;

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<dynamic>(responseString);
            }
        }

        public static async Task CreateAccountAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:58130/Api/Authorization/Register")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new PasswordViewModel()
                {
                    Email = email, 
                    Name = "admin", 
                    Surrname = "Ja", 
                    Password  = password
                }), Encoding.UTF8, "application/json")
            };

            // Ignore 409 responses, as they indicate that the account already exists.
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        public static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:58130/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = email,
                ["password"] = password
            });

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload["error"] != null)
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return (string)payload["access_token"];
        }

        public static async Task<string> GetResourceAsync(HttpClient client, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:58130/api/user/test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
