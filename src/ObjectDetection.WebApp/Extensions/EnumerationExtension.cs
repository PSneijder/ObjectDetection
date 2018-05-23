using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ObjectDetection.WebApp.Extensions
{
    internal static class EnumerationExtension
    {
        public static string Name(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DisplayAttribute attributes = field.GetCustomAttribute<DisplayAttribute>();

            return attributes != null ? attributes.Name : value.ToString();
        }

        public static string Description(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DisplayAttribute attributes = field.GetCustomAttribute<DisplayAttribute>();

            return attributes != null ? attributes.Description : value.ToString();
        }
    }
}