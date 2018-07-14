using CustomAttributeExample;
using CustomAttributeExample.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CustomAttributeExampleTests
{
    public class CustomerServiceTests
    {
        [Fact]
        public void CustomerService_ReturnsDeltaObjects_OnlyForUpdatableProperties()
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

            var sut = new CustomerService();
            var result = sut.UpdateCustomer(testCustomerJObj);

            Assert.NotNull(result);
            Assert.IsType<List<DeltaObject>>(result);
            Assert.True(result.Count == 2);
            Assert.Contains(result, d => d.Value == updatedFirstName);
            Assert.Contains(result, d => d.Value == updatedLastName);
        }
    }
}
