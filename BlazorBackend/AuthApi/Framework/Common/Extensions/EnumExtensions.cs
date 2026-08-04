using AuthApi.Framework.Common.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AuthApi.Framework.Common.Extensions
{
    /// <summary>
    /// Extension methods for System.Enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets all enum values
        /// </summary>
        public static IEnumerable<T> GetEnumValues<T>(this T input) where T : struct
        {
            return !typeof(T).IsEnum ? throw new NotSupportedException() : Enum.GetValues(input.GetType()).Cast<T>();
        }

        /// <summary>
        /// Gets enum flags that are set on the current value
        /// </summary>
        public static IEnumerable<T> GetEnumFlags<T>(this T input) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException();

            foreach (var value in Enum.GetValues(input.GetType()))
                if ((input as Enum)!.HasFlag((value as Enum)!))
                    yield return (T)value;
        }

        /// <summary>
        /// Gets the display name of an enum value based on DisplayAttribute
        /// </summary>
        public static string ToDisplay(this Enum value, DisplayProperty property = DisplayProperty.Name)
        {
            Assert.NotNull(value, nameof(value));

            var attribute = value.GetType().GetField(value.ToString())!
                .GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

            if (attribute == null)
                return value.ToString();

            var propValue = attribute.GetType().GetProperty(property.ToString())!.GetValue(attribute, null);
            return propValue?.ToString()!;
        }

        /// <summary>
        /// Converts enum to dictionary with int key and display name value
        /// </summary>
        public static Dictionary<int, string> ToDictionary(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(Convert.ToInt32, q => ToDisplay(q));
        }
    }

    /// <summary>
    /// Display properties for enum values
    /// </summary>
    public enum DisplayProperty
    {
        Description,
        GroupName,
        Name,
        Prompt,
        ShortName,
        Order
    }
}