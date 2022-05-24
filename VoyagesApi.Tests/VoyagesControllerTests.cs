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
        (IMemoryCache, ILogger<VoyagesController>, IVoyageDataStore) GetSetup()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            var logger = Mock.Of<ILogger<VoyagesController>>();

            var dataStore = A.Fake<IVoyageDataStore>();

            return (memoryCache, logger, dataStore);
        }

        [Fact]
        public async void GetAll_Returns_Correct_Number_Of_Items()
        {
            //Arrange
            var (memoryCache, logger, dataStore) = GetSetup();
            var fakeVoyages = A.CollectionOfDummy<Voyage>(5).AsEnumerable();
            A.CallTo(() => dataStore.GetAll()).Returns(fakeVoyages);
            var controller = new VoyagesController(memoryCache, logger, dataStore);

            //Act
            var actionResult = await controller.GetAll();

            //Assert
            var result = actionResult as OkObjectResult;
            var returnedVoyages = result.Value as IEnumerable<Voyage>;
            Assert.Equal(5, returnedVoyages.Count());
        }

        [Fact]
        public async void GetAverage_Returns_Correct_Average()
        {
            //Arrange
            var (memoryCache, logger, dataStore) = GetSetup();
            var fakeVoyages = A.CollectionOfDummy<Voyage>(10).AsEnumerable();
            A.CallTo(() => dataStore.GetAll()).Returns(fakeVoyages);
            var controller = new VoyagesController(memoryCache, logger, dataStore);

            //Update values
            var code = "123d";
            var price = 5;
            foreach (var voyage in fakeVoyages)
            {
                voyage.VoyageCode = code;
                voyage.Price = price;
            }
           
            //Act
            var actionResult = await controller.GetAveragePrice(code, Currency.USD);

            //Assert
            var result = actionResult as OkObjectResult;
            var returnedAverage = (decimal)result.Value;
            Assert.Equal(price, returnedAverage);
        }


        [Fact]
        public async void UpdatePrice_Returns_Created_Item()
        {
            //Arrange
            var (memoryCache, logger, dataStore) = GetSetup();

            var voyage = Mock.Of<Voyage>();
            voyage.VoyageCode = "Test";
            voyage.Price = 120;
            voyage.Currency = Currency.USD;
            voyage.Timestamp = DateTimeOffset.Now;

            A.CallTo(() => dataStore.SaveVoyage(voyage)).Returns((true, new List<Voyage>() { voyage }));
            
            var controller = new VoyagesController(memoryCache, logger, dataStore);

            //Act
            var actionResult = await controller.UpdatePrice(voyage);

            //Assert
            var result = actionResult as CreatedResult;
            Assert.Equal(201, result.StatusCode);
        }
    }
}
