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

        public (List<UpdateObject>, Error) GetUpdateObjects<T>(JObject rawUpdateObj)
        {
            var updateObjects = new List<UpdateObject>();
            if (rawUpdateObj == null || !rawUpdateObj.HasValues)
            {
                var error = new Error
                {
                    Message = "No object provided to update"
                };

                return (null, error);
            }

            var updatablePropertyNames = GetUpdatablePropertyNames<T>();
            foreach (var obj in rawUpdateObj)
            {
                var rawPropertyName = obj.Key;
                if (!updatablePropertyNames.Contains(rawPropertyName))
                {
                    var error = new Error
                    {
                        Message = $"Cannot update property {rawPropertyName} on a {typeof(T).Name}."
                    };

                    return (null, error);
                }

                var value = rawUpdateObj.Value<string>(rawPropertyName);
                var updateObject = new UpdateObject
                {
                    Property = rawPropertyName,
                    Value = value
                };

                updateObjects.Add(updateObject);
            }

            return (updateObjects, null);
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

        //private bool ValueIsValidType<T>(string value)
        //{
            
        //}
    }
}
