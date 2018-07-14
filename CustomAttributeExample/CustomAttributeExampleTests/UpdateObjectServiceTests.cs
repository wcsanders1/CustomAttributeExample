using CustomAttributeExample;
using CustomAttributeExample.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CustomAttributeExampleTests
{
    public class UpdateObjectServiceTests
    {
        [Fact]
        public void GetUpdateObjects_ReturnsDeltaObjects_OnlyForUpdatableCustomerProperties()
        {
            const string updatedFirstName = "updatedFirstName";
            const string updatedLastName = "updatedLastName";
            var testCustomer = new
            {
                Id = 444,
                FirstName = updatedFirstName,
                LastName = updatedLastName,
                CreatedAt = DateTime.Now,
                Deleted = true,
                SomeBadProperty = "badValue"
            };

            var testCustomerJObj = JObject.FromObject(testCustomer);

            var sut = new UpdateObjectService();
            var result = sut.GetUpdateObjects<Customer>(testCustomerJObj);

            Assert.NotNull(result);
            Assert.IsType<List<UpdateObject>>(result);
            Assert.True(result.Count == 2);
            Assert.Contains(result, d => d.Value == updatedFirstName);
            Assert.Contains(result, d => d.Value == updatedLastName);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsDeltaObjects_OnlyForUpdatableSportsmanProperties()
        {
            const string updatedFirstName = "updatedFirstName";
            const string updatedLastName = "updatedLastName";
            var testSportsman = new
            {
                Id = 444,
                FirstName = updatedFirstName,
                LastName = updatedLastName,
                CreatedAt = DateTime.Now,
                Deleted = true,
                SomeBadProperty = "badValue"
            };

            var testSportsmanJObj = JObject.FromObject(testSportsman);
            var sut = new UpdateObjectService();
            var result = sut.GetUpdateObjects<Sportsman>(testSportsmanJObj);

            Assert.NotNull(result);
            Assert.IsType<List<UpdateObject>>(result);
            Assert.True(result.Count == 1);
            Assert.Contains(result, d => d.Value == updatedFirstName);
        }
    }
}
