using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using VoyagesApi.Controllers;
using VoyagesApi.Data;
using VoyagesApi.Models;
using Xunit;

namespace VoyagesApi.Tests
{
    public class VoyagesControllerTests
    {
        [Fact]
        public void GetAll_Returns_Correct_Number_Of_Items()
        {
            //Arrange
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            var logger = Mock.Of<ILogger<VoyagesController>>();

            var fakeVoyages = A.CollectionOfDummy<Voyage>(5).AsEnumerable();
            var dataStore = A.Fake<IVoyageDataStore>();
            A.CallTo(() => dataStore.GetAll()).Returns(fakeVoyages);

            var controller = new VoyagesController(memoryCache, logger, dataStore);

            //Act
            var actionResult = controller.GetAll();

            //Assert
            var result = actionResult as OkObjectResult;
            var returnedVoyages = result.Value as IEnumerable<Voyage>;
            Assert.Equal(5, returnedVoyages.Count());
        }
    }
}
