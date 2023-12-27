using Microsoft.AspNetCore.Mvc;
using SkiStore.API.Models.API;

namespace SkiStore.API.Controllers;

public class BuggyController:BaseApiController
{


    [HttpGet("bad-request")]
    public ActionResult<APIResponse<string>> GetBadRequest()
    {
        
            CheckControllerData();

            APIResponse<string> APIResponse = new()
            {
                Data = string.Empty,
                StatusCode = 400,
                IsSuccessful = false,
                ErrorMessage = "This is an example of a bad request"
            };

            ProvideLog(APIResponse.GetResponseInfo());

            return DetermineResponseType(APIResponse);
        
    }

    [HttpGet("not-found")]
    public ActionResult<APIResponse<string>> GetNotFound()
    {

        CheckControllerData();

        APIResponse<string> APIResponse = new()
        {
            Data = string.Empty,
            StatusCode = 404,
            IsSuccessful = false,
            ErrorMessage = "This is an example of a not found"
        };

        ProvideLog(APIResponse.GetResponseInfo());

        return DetermineResponseType(APIResponse);

    }

    [HttpGet("unauthorized")]
    public ActionResult<APIResponse<string>> GetUnauthorized()
    {

        CheckControllerData();

        APIResponse<string> APIResponse = new()
        {
            Data = string.Empty,
            StatusCode = 401,
            IsSuccessful = false,
            ErrorMessage = "This is an example of unauthorized"
        };

        ProvideLog(APIResponse.GetResponseInfo());

        return DetermineResponseType(APIResponse);

    }

    [HttpGet("server-error")]
    public ActionResult GetServerError()
    {
        throw new Exception("This is a server error !");
    }
}
