using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VoyagesApi.Models;
using Xunit;

namespace VoyagesApi.IntegrationTests
{
    public class VoyagesControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_Returns_Ok_Response()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/Voyages/GetAll");

            //// Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAveragePrice_Returns_Ok_Response()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync($"/api/Voyages/GetAveragePrice(451S,{Currency.EUR})");

            //// Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAveragePrice_Returns_Unauthorized()
        {
            // Arrange
            //Do not authenticate first

            // Act
            var response = await TestClient.GetAsync($"/api/Voyages/GetAveragePrice(451S,{Currency.EUR})");

            //// Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAveragePrice_Returns_NotFound_Response()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync($"/api/Voyages/GetAveragePrice(Test,{Currency.EUR})");

            //// Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }     

        [Fact]
        public async Task UpdatePrice_Returns_Created_Response()
        {
            // Arrange
            await AuthenticateAsync();     
            var uri = TestClient.BaseAddress + "api/Voyages/UpdatePrice(test,123,USD,2022-05-22T14%3A14%3A34.7363227)";

            // Act
            var response = await TestClient.PostAsync(uri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            //// Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePrice_Returns_Unauthorized_Response()
        {
            // Arrange
            var uri = TestClient.BaseAddress + "api/Voyages/UpdatePrice(test,123,USD,2022-05-22T14%3A14%3A34.7363227)";

            // Act
            var response = await TestClient.PostAsync(uri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            //// Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePrice_Returns_Created_Response2()
        {
            // Arrange
            await AuthenticateAsync();
            var uri = TestClient.BaseAddress + "api/Voyages/UpdatePrice";

            var body = "{\"VoyageCode\":\"Test\",\"Price\":120.0,\"Currency\":0,\"Timestamp\":\"2022-05-25T01:48:05.5339157+02:00\"}";


            // Act
            var response = await TestClient.PostAsync(uri, new StringContent(body, Encoding.UTF8, "application/json"));

            //// Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePrice_Returns_Unauthorized_Response2()
        {
            // Arrange
            //Do not authenticate first
            var uri = TestClient.BaseAddress + "api/Voyages/UpdatePrice";

            var body = "{\"VoyageCode\":\"Test\",\"Price\":120.0,\"Currency\":0,\"Timestamp\":\"2022-05-25T01:48:05.5339157+02:00\"}";


            // Act
            var response = await TestClient.PostAsync(uri, new StringContent(body, Encoding.UTF8, "application/json"));

            //// Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
