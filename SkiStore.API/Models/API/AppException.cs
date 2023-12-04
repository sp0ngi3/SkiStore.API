namespace SkiStore.API.Models.API;

public class AppException
{
    public int StatusCode { get; set; }

    public string ErrorMessage { get; set; }

    public string? Details { get; set; }

    public AppException(int statusCode, string message, string details = null)
    {
        StatusCode = statusCode;
        ErrorMessage = message;
        Details = details;
    }
}
