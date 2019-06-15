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

namespace ClientApp
{
    [TestClass]
    public class Program
    {

        

        [TestMethod]
        public async Task MainAsync()
        {
            var client = new HttpClient();

            var token =
                "eyJhbGciOiJSUzI1NiIsImtpZCI6IlI1T09WTTJaSE9QVkJCNU1URURHS0JKRTNRWlo4TERSTjFOSlo3TU4iLCJ0eXAiOiJKV1QifQ.eyJzdWIiOiIxMDAwNzIwNzY1NDI1MzM1MDk5NjMiLCJuYW1lIjoiQmFydG9zeiBHb8WCxJliaW93c2tpIiwiZW1haWwiOiJpdHVyaWVsLmdvbEBnbWFpbC5jb20iLCJ0b2tlbl91c2FnZSI6ImFjY2Vzc190b2tlbiIsImp0aSI6ImNmMTlhMGMyLTI3OTctNDhkNi1hMjc1LWNiYmYzZTA4NWY1OCIsInNjb3BlIjpbIm9wZW5pZCIsIm9mZmxpbmVfYWNjZXNzIl0sImF6cCI6Im1vYmlsZV9hcHAiLCJuYmYiOjE1NjAzNzYwMDMsImV4cCI6MTU2MDM3OTYwMywiaWF0IjoxNTYwMzc2MDAzLCJpc3MiOiJodHRwOi8vMTkyLjE2OC4xLjEwNC9NYW55Rm9yTWFueSJ9.pWdSFz8ZlbtZYG0hHrUdSzyVfkMUNdJNAqqPMxfjgDplhBSTsYwasyaR_l1TBjGCzrVKjyg0bfR6QVdRWW9D0OBFfI8bGehRZwp_UgjyV6-dMX3EbqIabc1TyO1vGjDjhh9Zy7_scngyUavziAKdM0axBfYeVuu-rnIVW6bX7fWlvHejlG6DzQgcdCFpR8AOdqXqpgSNkrfmpY_W5oAEQy4Kden5YUrL1EKdhmURuniDWMUGYfqIKL32HuTJg0fnhNMdH2QvFUdnbhCepQKOg0bvlltJH_lRkGVz45AsbPMG_ZK1NT25gBhkfdTpfS7oWuwkI1j7TcDAAzNTgENjGA";
            Console.WriteLine("Access token: {0}", token);
            Console.WriteLine();

            var resource = await GetResourceAsync(client, token);
            Console.WriteLine("API response: {0}", resource);

            Console.ReadLine();
        }

        public static async Task CreateAccountAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/ManyForMany/Account/UserInfo")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { email, password }), Encoding.UTF8, "application/json")
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
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:58795/connect/token");
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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/ManyForMany/Account/UserInfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}