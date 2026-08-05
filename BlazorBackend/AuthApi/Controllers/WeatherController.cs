using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

/// <summary>
/// Provides weather forecast data.
/// This controller demonstrates a protected API endpoint
/// that requires authentication using JWT Bearer tokens.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    // Collection of possible weather descriptions
    // used for generating sample forecast data.
    private static readonly string[] Summaries =
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    };

    /// <summary>
    /// Returns a list of weather forecasts.
    ///
    /// This endpoint requires an authenticated user.
    /// A valid JWT token must be included in the request.
    /// </summary>
    /// <returns>
    /// A collection of randomly generated weather forecast data.
    /// </returns>
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        // Generate five sample weather forecast records.
        var forecasts = Enumerable.Range(1, 5)
            .Select(index => new WeatherForecast
            {
                // Generate a date starting from tomorrow.
                Date = DateOnly.FromDateTime(
                    DateTime.Now.AddDays(index)
                ),

                // Generate a random temperature between -20 and 55 Celsius.
                TemperatureC = Random.Shared.Next(-20, 55),

                // Select a random weather description.
                Summary = Summaries[
                    Random.Shared.Next(Summaries.Length)
                ]
            })
            .ToArray();

        // Return the generated forecasts as an HTTP 200 response.
        return Ok(forecasts);
    }
}

/// <summary>
/// Represents a weather forecast model.
/// This class contains sample weather information returned by the API.
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// Date of the weather forecast.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Temperature value in Celsius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Text description of the weather condition.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Calculated temperature value in Fahrenheit.
    /// Converts Celsius to Fahrenheit automatically.
    /// </summary>
    public int TemperatureF =>
        32 + (int)(TemperatureC / 0.5556);
}