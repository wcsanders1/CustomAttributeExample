using CustomAttributeExample;
using CustomAttributeExample.CustomAttributes;
using CustomAttributeExample.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CustomAttributeExampleTests
{
    public class UpdateObjectServiceTests
    {
        private class TestCustomer
        {
            public long Id { get; set; }

            [IsUpdatable]
            public string FirstName { get; set; }

            [IsUpdatable]
            public string LastName { get; set; }

            [IsUpdatable]
            public string Address { get; set; }

            public DateTime CreatedAt { get; set; }

            public bool Deleted { get; set; }
        }

        private class TestCompany
        {
            [IsUpdatable]
            public string Name { get; set; }

            public int Assets { get; set; }

            public int Debts { get; set; }

            [IsUpdatable]
            public int Sales { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        private class TestAccount
        {
            [IsUpdatable]
            public decimal Amount { get; set; }

            [IsUpdatable]
            public uint TripsTaken { get; set; }

            [IsUpdatable]
            public DateTime DueDate { get; set; }
        }

        [Fact]
        public void GetUpdateObjects_ReturnsError_WhenProvidedNonUpdatableCustomerProperty()
        {
            const string updatedFirstName = "updatedFirstName";
            const string updatedLastName = "updatedLastName";

            var testCustomer = new
            {
                Id = 444,
                FirstName = updatedFirstName,
                LastName = updatedLastName,
                CreatedAt = DateTime.Now,
                Deleted = true
            };

            var testCustomerJObj = JObject.FromObject(testCustomer);

            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestCustomer>(testCustomerJObj);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<Error>(error);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsError_ReturnsErrorWhenProvidedNonUpdatableCompanyProperty()
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

            var testCompanyJObj = JObject.FromObject(testSportsman);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestCompany>(testCompanyJObj);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<Error>(error);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsUpdateObjects_WhenProvidedValidCustomerProperties()
        {
            const string updatedFirstName = "updatedFirstName";
            const string updatedLastName = "updatedLastName";

            var testCustomer = new
            {
                FirstName = updatedFirstName,
                LastName = updatedLastName,
            };

            var testCustomerJObj = JObject.FromObject(testCustomer);

            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestCustomer>(testCustomerJObj);

            Assert.Null(error);
            Assert.NotNull(result);
            Assert.IsType<List<UpdateObject>>(result);
            Assert.True(result.Count == 2);
            Assert.Contains(result, d => d.Value == updatedFirstName);
            Assert.Contains(result, d => d.Value == updatedLastName);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsUpdateObjects_WhenProvidedValidCompanyProperties()
        {
            const string updatedName = "testCompanyName";
            const string updatedSales = "60";

            var testCompany = new
            {
                Name = updatedName,
                Sales = updatedSales
            };

            var testCompanyObj = JObject.FromObject(testCompany);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestCompany>(testCompanyObj);

            Assert.Null(error);
            Assert.NotNull(result);
            Assert.IsType<List<UpdateObject>>(result);
            Assert.True(result.Count == 2);
            Assert.Contains(result, d => d.Value == updatedName);
            Assert.Contains(result, d => d.Value == updatedSales);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsError_WhenPropertyHasInvalidDecimalValue()
        {
            var testAccount = new
            {
                Amount = "notADecimal"
            };

            var testAccountObj = JObject.FromObject(testAccount);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestAccount>(testAccountObj);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<Error>(error);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsError_WhenPropertryHasInvalidUIntValue()
        {
            var testAccount = new
            {
                TripsTaken = -5
            };

            var testAccountObj = JObject.FromObject(testAccount);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestAccount>(testAccountObj);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<Error>(error);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsError_WhenPropertyHasInvalidDateValue()
        {
            var testAccount = new
            {
                DueDate = "not a date"
            };

            var testAccountObj = JObject.FromObject(testAccount);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestAccount>(testAccountObj);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<Error>(error);
        }

        [Fact]
        public void GetUpdateObjects_ReturnsUpdateObjects_WhenPropertyHasValidValues()
        {
            const decimal testAmount = 45.66M;
            const uint testTripsTaken = 456;
            var testDueDate = DateTime.Now;

            var testAccount = new
            {
                Amount = testAmount,
                TripsTaken = testTripsTaken,
                DueDate = testDueDate
            };

            var testAccountObj = JObject.FromObject(testAccount);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestAccount>(testAccountObj);

            Assert.Null(error);
            Assert.NotNull(result);
            Assert.IsType<List<UpdateObject>>(result);
            Assert.Contains(result, d => d.Value == testAmount.ToString());
            Assert.Contains(result, d => d.Value == testTripsTaken.ToString());
        }

        [Fact]
        public void GetUpdateObjects_ReturnsUpdateObjects_WhenDatePropertyHasConvertableStringValue()
        {
            const decimal testAmount = 45.66M;
            const uint testTripsTaken = 456;
            var testDueDate = "june 5, 2004";

            var testAccount = new
            {
                Amount = testAmount,
                TripsTaken = testTripsTaken,
                DueDate = testDueDate
            };

            var testAccountObj = JObject.FromObject(testAccount);
            var sut = new UpdateObjectService();
            var (result, error) = sut.GetUpdateObjects<TestAccount>(testAccountObj);

            Assert.Null(error);
            Assert.NotNull(result);
            Assert.IsType<List<UpdateObject>>(result);
            Assert.Contains(result, d => d.Value == testAmount.ToString());
            Assert.Contains(result, d => d.Value == testTripsTaken.ToString());
            Assert.Contains(result, d => d.Value == testDueDate.ToString());
        }
    }
}
