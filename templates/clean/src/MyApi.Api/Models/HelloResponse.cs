namespace MyApi.Api.Models;

/// <summary>
/// Response model for the hello endpoint.
/// </summary>
public record HelloResponse(string Message, DateTime Timestamp);
