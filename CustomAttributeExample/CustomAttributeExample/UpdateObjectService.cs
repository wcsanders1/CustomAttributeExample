using CustomAttributeExample.CustomAttributes;
using CustomAttributeExample.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomAttributeExample
{
    public class UpdateObjectService
    {
        private Dictionary<string, List<PropertyInfo>> PropertyInfoByClassName
        { get; set;} = new Dictionary<string, List<PropertyInfo>>();

        private Dictionary<string, List<string>> UpdatablePropertyNamesByClassName
        { get; set; } = new Dictionary<string, List<string>>();

        public List<UpdateObject> GetUpdateObjects<T>(JObject rawUpdateObj)
        {
            var updateObjects = new List<UpdateObject>();
            if (rawUpdateObj == null || !rawUpdateObj.HasValues)
            {
                return updateObjects;
            }

            var updatablePropertyNames = GetUpdatablePropertyNames<T>();
            foreach (var obj in rawUpdateObj)
            {
                var rawPropertyName = obj.Key;

                if (!updatablePropertyNames.Contains(rawPropertyName))
                {
                    continue;
                }

                var value = rawUpdateObj.Value<string>(rawPropertyName);
                var deltaObject = new UpdateObject
                {
                    Property = rawPropertyName,
                    Value = value
                };

                updateObjects.Add(deltaObject);
            }

            return updateObjects;
        }

        private List<string> GetUpdatablePropertyNames<T>()
        {
            var classType = typeof(T);
            var className = classType.Name;
            if (UpdatablePropertyNamesByClassName.TryGetValue(className, out var updatablePropertyNames))
            {
                return updatablePropertyNames;
            }

            updatablePropertyNames = classType
                .GetProperties()
                .Where(pi => Attribute.IsDefined(pi, typeof(IsUpdatableAttribute)))
                .Select(pi => pi.Name)
                .ToList();

            UpdatablePropertyNamesByClassName.Add(className, updatablePropertyNames);

            return updatablePropertyNames;
        }
    }
}
