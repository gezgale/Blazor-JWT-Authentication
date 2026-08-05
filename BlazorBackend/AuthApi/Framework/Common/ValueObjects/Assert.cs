using System.Collections;

namespace AuthApi.Framework.Common.ValueObjects;

/// <summary>
/// Provides helper methods for validating method arguments.
/// Throws appropriate exceptions when validation rules fail.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Ensures that a reference type argument is not null.
    /// </summary>
    /// <typeparam name="T">Reference type to validate.</typeparam>
    /// <param name="obj">The object to validate.</param>
    /// <param name="name">The argument name.</param>
    /// <param name="message">Optional custom exception message.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="obj"/> is null.
    /// </exception>
    public static void NotNull<T>(T obj, string name, string message = null!)
        where T : class
    {
        if (obj is null)
            throw new ArgumentNullException($"{name} : {typeof(T)}", message);
    }

    /// <summary>
    /// Ensures that a nullable value type contains a value.
    /// </summary>
    /// <typeparam name="T">Value type to validate.</typeparam>
    /// <param name="obj">The nullable value.</param>
    /// <param name="name">The argument name.</param>
    /// <param name="message">Optional custom exception message.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the nullable value has no value.
    /// </exception>
    public static void NotNull<T>(T? obj, string name, string message = null!)
        where T : struct
    {
        if (!obj.HasValue)
            throw new ArgumentNullException($"{name} : {typeof(T)}", message);
    }

    /// <summary>
    /// Ensures that a reference type is not null or empty.
    /// Supports strings and collections.
    /// </summary>
    /// <typeparam name="T">Reference type to validate.</typeparam>
    /// <param name="obj">The object to validate.</param>
    /// <param name="name">The argument name.</param>
    /// <param name="message">Optional custom exception message.</param>
    /// <param name="defaultValue">
    /// A custom value considered equivalent to "empty".
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is null, an empty/whitespace string,
    /// an empty collection, or equals the specified default value.
    /// </exception>
    public static void NotEmpty<T>(T obj, string name, string message = null!, T defaultValue = null!)
        where T : class
    {
        // Check if the object equals the specified default value.
        if (obj == defaultValue

            // Check if the object is an empty or whitespace string.
            || (obj is string str && string.IsNullOrWhiteSpace(str))

            // Check if the object is an empty collection.
            || (obj is IEnumerable list && !list.Cast<object>().Any()))
        {
            throw new ArgumentException(
                @"Argument is empty : " + message,
                $"{name} : {typeof(T)}");
        }
    }
}