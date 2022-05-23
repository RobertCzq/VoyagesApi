using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task GetAverage_Returns_Ok_Response()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync($"/api/Voyages/GetAverage(451S,{Currency.EUR})");

            //// Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAverage_Returns_Unauthorized()
        {
            // Arrange
            //await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync($"/api/Voyages/GetAverage(451S,{Currency.EUR})");

            //// Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
