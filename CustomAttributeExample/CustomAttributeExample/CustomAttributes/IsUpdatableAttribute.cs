using System;

namespace CustomAttributeExample.CustomAttributes
{
    /// <summary>
    /// Determines whether a property is updatable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IsUpdatableAttribute : Attribute
    {}
}
