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

namespace Test
{
    [TestClass]
    public class AuthorizeTest
    {
        [TestMethod]
        public void CallServer()
        {
            async Task method()
            {
                var client = new HttpClient();

                const string email = "bob@le-magnifique.com", password = "}s>EWG@f4g;_v7nB";


                //var token = await GetTokenAsync(client, //Console.WriteLine("Access token: {0}", token);

                    //Console.WriteLine();

                var token = await GetTokenAsync(client, email, password);

                var resource = await GetResourceAsync(client, token);
                Console.WriteLine("API response: {0}", resource);
            }
            
            method().GetAwaiter().GetResult();
        }

        public static async Task<string> GetTokenAsync(HttpClient client, string userName, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/ManyForMany/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "mobile_app",
                ["username"] = userName,
                ["password"] = password,
                ["scope"] = "openid"
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

        public static async Task<string> GetTokenAsync(HttpClient client, string googleToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/ManyForMany/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "Google",
                ["client_id"] = "mobile_app",
                ["token"] = googleToken,
                ["scope"] = "openid"
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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/ManyForMany/UserInfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
