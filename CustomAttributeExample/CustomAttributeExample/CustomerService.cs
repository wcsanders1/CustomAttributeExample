using CustomAttributeExample.CustomAttributes;
using CustomAttributeExample.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomAttributeExample
{
    public class CustomerService
    {
        public List<DeltaObject> UpdateCustomer(JObject rawCustomer)
        {
            var deltaObjects = new List<DeltaObject>();
            if (rawCustomer == null || !rawCustomer.HasValues)
            {
                return deltaObjects;
            }

            var customerProperties = typeof(Customer)
                .GetProperties();

            var customerPropertyNames = customerProperties
                .Select(p => p.Name)
                .ToList();

            var updatableCustomerPropertyNames = customerProperties
                .Where(pi => Attribute.IsDefined(pi, typeof(IsUpdatableAttribute)))
                .Select(pi => pi.Name)
                .ToList();
            
            foreach (var customer in rawCustomer)
            {
                var rawPropertyName = customer.Key;

                if (!customerPropertyNames.Contains(rawPropertyName))
                {
                    // Here, you could log that the property doesn't exist on the model at all.
                    continue;
                }

                if (!updatableCustomerPropertyNames.Contains(rawPropertyName))
                {
                    // Here, you could log that while the property exists on the model,
                    // it is not a property that may be updated.
                    continue;
                }

                var value = rawCustomer.Value<string>(rawPropertyName);
                var deltaObject = new DeltaObject
                {
                    Property = rawPropertyName,
                    Value = value
                };

                deltaObjects.Add(deltaObject);
            }

            return deltaObjects;
        }
    }
}
