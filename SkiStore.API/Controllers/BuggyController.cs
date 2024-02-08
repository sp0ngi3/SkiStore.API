using Microsoft.AspNetCore.Mvc;
using SkiStore.API.Models.API;

namespace SkiStore.API.Controllers;

[ApiController]
[Route("buggy")]
public class BuggyController : ControllerBase
{
    [HttpGet("bad-request")]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public ActionResult<APIResponse<string>> GetBadRequest()
    {
        return Problem("Bad Request", "buggy/bad-request", 400, "Buggy Controller");
    }

    [HttpGet("not-found")]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public ActionResult<APIResponse<string>> GetNotFound()
    {
        return Problem("Not Found", "buggy/not-found", 404, "Buggy Controller");
    }

    [HttpGet("unauthorized")]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public ActionResult<APIResponse<string>> GetUnauthorized()
    {
        return Problem("Unauthorized", "buggy/unauthorized", 401, "Buggy Controller");
    }

    [HttpGet("server-error")]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public ActionResult GetServerError()
    {
        return Problem("Exception", "buggy/server-error", 500, "Buggy Controller");
    }

    [HttpGet("custom-server-error")]
    [ProducesErrorResponseType(typeof(AppException))]
    public ActionResult<APIResponse<string>> GetCustomServerError()
    {
        throw new Exception("This is a server error !");
    }
}
