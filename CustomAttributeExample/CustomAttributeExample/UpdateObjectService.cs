using CustomAttributeExample.CustomAttributes;
using CustomAttributeExample.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            where T : class
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
                var propertyName = obj.Key;
                if (!updatablePropertyNames.Contains(propertyName))
                {
                    var error = new Error
                    {
                        Message = $"Cannot update property '{propertyName}' on a {typeof(T).Name}."
                    };

                    return (null, error);
                }

                var value = rawUpdateObj.Value<string>(propertyName);
                var typeError = ValidatePropertyType<T>(propertyName, value);
                if (typeError != null)
                {
                    return (null, typeError);
                }

                var updateObject = new UpdateObject
                {
                    Property = propertyName,
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

        private Error ValidatePropertyType<T>(string property, string value)
        {
            var classType = typeof(T);
            var className = classType.Name;
            if (!PropertyInfoByClassName.TryGetValue(className, out var propertiesInfo))
            {
                propertiesInfo = classType.GetProperties().ToList();
                PropertyInfoByClassName.Add(className, propertiesInfo);
            }

            var propertyType = propertiesInfo
                .FirstOrDefault(p => p.Name == property)
                .PropertyType;

            if (propertyType == null)
            {
                return new Error
                {
                    Message = $"'{property}' does not exist on {className}"
                };
            }

            var converter = TypeDescriptor.GetConverter(propertyType);
            try
            {
                converter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                return new Error
                {
                    Exception = ex,
                    Message = $"Cannot convert '{value}' into {propertyType.Name}"
                };
            }

            return null;
        }
    }
}
