using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsyncAmnin()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync("admin", "admin_PW"));
        }

        protected async Task AuthenticateAsyncNormal()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync("normal", "normal_PW"));
        }

        private async Task<string> GetJwtAsync(string username, string password)
        {
            var response = await TestClient.PostAsJsonAsync("/api/Login", new UserLogin 
            {
                Username = username,
                Password = password
            });

            var registrationResponse = await response.Content.ReadAsStringAsync();
            return registrationResponse;
        }
    }
}
