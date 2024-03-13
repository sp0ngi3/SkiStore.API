using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiStore.API.DTOs.SkiStoreDB.Account;
using SkiStore.API.Models.API;
using SkiStore.API.Services.SkiStoreDB;

namespace SkiStore.API.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    /// <summary>
    /// Log User in
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/account/login
    ///     {
    ///     "Username":"bob",
    ///     "Password":"Pa$$w0rd"
    ///     }
    ///     
    /// </remarks>
    /// <returns>A model of User Return DTO</returns>
    /// <response code="200">User was logged in</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="500">General Error</response>
    [HttpPost("login")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(UserDTO), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<ActionResult<UserDTO>> LogInUser([FromBody] LoginDTO loginDTO
        )
    {
        try
        {
            APIResponse<UserDTO> response = await _accountService.LogInUser(loginDTO);
            if (response.IsSuccessful) return Ok(response.Data);
            else
            {
                _logger.LogError($"POST account/login " + response.ErrorMessage);
                return Problem(response.ErrorMessage, $"POST account/login", response.StatusCode, "Problem logging User in");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"POST account/login " + ex.Message);
            return Problem(ex.Message, $"POST account/login", 500, "Problem logging User in");
        }
    }

    /// <summary>
    /// Register User
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/account/register
    ///     {
    ///     "Email":"bob@test.com",
    ///     "Username":"bob",
    ///     "Password":"Pa$$w0rd"
    ///     }
    ///     
    /// </remarks>
    /// <returns>A message of the success </returns>
    /// <response code="201">The User was registered</response>
    /// <response code="400">Problem registering user</response>
    /// <response code="500">General Error</response>
    [HttpPost("register")]
    [Consumes("application/json")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<ActionResult<string>> RegisterUser([FromBody] RegisterDTO registerDto)
    {
        try
        {
            APIResponse<bool> response = await _accountService.RegisterUser(registerDto);
            if (response.IsSuccessful) return CreatedAtAction(nameof(RegisterUser), response.SuccessMessage);
            else
            {
                _logger.LogError($"POST account/register " + response.ErrorMessage);
                return Problem(response.ErrorMessage, $"POST account/register", response.StatusCode, "Problem registering User");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"POST account/register " + ex.Message);
            return Problem(ex.Message, $"POST account/register", 500, "Problem registering User");
        }
    }

    /// <summary>
    /// Get Current User
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/account/currentUser
    ///     
    /// </remarks>
    /// <returns>A message of the success </returns>
    /// <response code="200">The User was registered</response>
    /// <response code="400">Token not found</response>
    /// <response code="401">User not found</response>
    /// <response code="500">General Error</response>
    [Authorize]
    [HttpGet("currentUser")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserDTO), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<ActionResult<UserDTO>> GetCurrentUser()
    {
        try
        {
            APIResponse<UserDTO> response = await _accountService.GetCurrentUser(User.Identity.Name);
            if(response.IsSuccessful) return Ok(response.Data);
            else
            {
                _logger.LogError($"GET account/currentUser " + response.ErrorMessage);
                return Problem(response.ErrorMessage, $"GET account/currentUser", response.StatusCode, "Problem getting current User");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"GET account/currentUser " + ex.Message);
            return Problem(ex.Message, $"GET account/currentUser", 500, "Problem getting current User");
        }
    }
}
