using SkiStore.API.Models.API;
using static SkiStore.API.Controllers.BaseApiController;

namespace SkiStore.API.Extensions;

public static class HelperFunctions
{
    public static string BuildLog(APILoggerInfo loggerInfo, ControllerData controllerData)
    {
        List<string> logParts = new();
        string lastMessage;

        if (loggerInfo.StatusCode == 500)
        {
            logParts.Add("-EXCEPTION=>");
            lastMessage = loggerInfo.Message;
        }
        else if (loggerInfo.IsSuccessful == false)
        {
            logParts.Add("-FAILURE=>");
            lastMessage = loggerInfo.Message;
        }
        else
        {
            logParts.Add("SUCCESS=>");
            lastMessage = loggerInfo.SuccessMessage;
        }

        logParts.Add(controllerData.ControllerName);

        logParts.Add(controllerData.ActionName);

        logParts.Add($"STATUS:{loggerInfo.StatusCode}");

        logParts.Add(lastMessage);


        return string.Join(" ", logParts);

    }
}
