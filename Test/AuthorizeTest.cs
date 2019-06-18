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


                //var token = await GetTokenAsync(client, "eyJhbGciOiJSUzI1NiIsImtpZCI6IjY4NjQyODlmZmE1MWU0ZTE3ZjE0ZWRmYWFmNTEzMGRmNDBkODllN2QiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI4MTk2MDg2Mzk0OTEtbjM1azMyYjFyM3M0bW5mcGE2bnByYjhjNTIyNTd1aGwuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI4MTk2MDg2Mzk0OTEtbjM1azMyYjFyM3M0bW5mcGE2bnByYjhjNTIyNTd1aGwuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDAwNzIwNzY1NDI1MzM1MDk5NjMiLCJlbWFpbCI6Iml0dXJpZWwuZ29sQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdF9oYXNoIjoiNjJHLUU4aGV0QjNDUmJpVDN1Y25idyIsIm5hbWUiOiJCYXJ0b3N6IEdvxYLEmWJpb3dza2kiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDYuZ29vZ2xldXNlcmNvbnRlbnQuY29tLy1kRWpyM0xuNXlqSS9BQUFBQUFBQUFBSS9BQUFBQUFBQUFHcy9xMFVwYVREbHl1NC9zOTYtYy9waG90by5qcGciLCJnaXZlbl9uYW1lIjoiQmFydG9zeiIsImZhbWlseV9uYW1lIjoiR2_FgsSZYmlvd3NraSIsImxvY2FsZSI6InBsIiwiaWF0IjoxNTYwODgxNzY1LCJleHAiOjE1NjA4ODUzNjV9.ApKIeKhigtp4N38k7htK5eT3MNOg0vAb4BOSNLSedZXiPWQJ7P5CjWuVin8UFRPA4Tmy6HQV398hWlO7IdfXBzjz2qwn25W2QT99yt-hplynh8c8JMXyP4kDtll1CN8WAZiFlNj3XxeFjBVXycAdUls_olQx4OrfMcreztgP6_weuSAeov2GqqVRxWAGrTwpVSdAls08TZ3R1yBSrDrvuTdDx5oZobxHltdu6NgUqqOR-_xdNBvtUTdi3AUMtVimvnji612tmI0ysJ-l1fUigm7hQbB-CRbaZ1v4TxlZ9iQLH1EDmCQ-5kzE9Cz_j0a67AYlkATEgsWWqlNEMCANVQ");
                //Console.WriteLine("Access token: {0}", token);
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
