using System.ComponentModel.DataAnnotations;

namespace BlazorFrontend.Helpers
{
    public static class DisplayName
    {
        public static String GetDisplayName<T>(String propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            var displayAttribute = property?
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .FirstOrDefault();
            return displayAttribute?.Name ?? propertyName;
        }
    }
}
