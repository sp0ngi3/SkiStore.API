using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using SkiStore.API.Extensions;
using SkiStore.API.Models.API;

namespace SkiStore.API.Controllers;

[ApiController]
[Route("/api/")]
public class BaseApiController:ControllerBase
{
    public static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    public string? _controllerName;
    public string? _actionName;


    public BaseApiController()
    {
        CheckAccessToEnv();
    }
    private static void CheckAccessToEnv()
    {
        if (!StaticValues.StaticValues.IsEnvValid())
        {
            logger.Error("ERROR => Problem loading the variables!");
            throw new Exception("Problem loading the variables!");
        }
    }

    public ActionResult DetermineResponseType<T>(APIResponse<T> response)
    {

        return response.StatusCode switch
        {
            200 => Ok(response),
            201 => StatusCode(201, response),
            204 => StatusCode(204, response),
            400 => BadRequest(response),
            401 => Unauthorized(response),
            404 => NotFound(response),
            500 => StatusCode(500, response),
            _ => BadRequest(response),
        };
    }


    public void ProvideLog(APILoggerInfo loggerInf)
    {
        ControllerData controllerData = GetControllerData();

        string log = HelperFunctions.BuildLog(loggerInf, controllerData);

        if (log[0] == '-')
        {
            logger.Error(log);
        }
        else
        {
            logger.Info(log);
        }
    }



    private void InitializeControllerContext()
    {

        ControllerContext controllerContext = ControllerContext;


        if (controllerContext != null && controllerContext.ActionDescriptor != null)
        {

            ActionDescriptor actionDescriptor = controllerContext.ActionDescriptor;


            _controllerName = actionDescriptor.RouteValues["controller"];
            _actionName = actionDescriptor.RouteValues["action"];
        }
    }

    public void CheckControllerData()
    {
        if (string.IsNullOrEmpty(_controllerName) || string.IsNullOrEmpty(_actionName))
        {
            InitializeControllerContext();
        }
    }

    private ControllerData GetControllerData()
    {
        if (_actionName != null && _controllerName != null)
        {
            return new ControllerData { ActionName = _actionName, ControllerName = _controllerName };
        }
        else return null!;
    }

    public class ControllerData
    {
        public string ControllerName { get; set; } = string.Empty;

        public string ActionName { get; set; } = string.Empty;

    }
}
