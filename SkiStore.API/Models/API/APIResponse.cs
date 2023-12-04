using System.Text.Json.Serialization;

namespace SkiStore.API.Models.API;

public class APIResponse<T>
{
    public int StatusCode { get; set; }

    public bool IsSuccessful { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    [JsonIgnore]
    public string SuccessMessage { get; set; } = string.Empty;

    public T? Data { get; set; }

    public APILoggerInfo GetResponseInfo()
    {
        APILoggerInfo loggerInfo = new()
        {
            IsSuccessful = IsSuccessful,
            Message = ErrorMessage,
            StatusCode = StatusCode,
            SuccessMessage = SuccessMessage

        };
        return loggerInfo;
    }
}

public class APILoggerInfo
{
    public bool IsSuccessful { set; get; }

    public string Message { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string SuccessMessage { get; set; } = string.Empty;

}

