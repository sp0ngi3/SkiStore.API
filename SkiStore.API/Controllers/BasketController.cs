using Microsoft.AspNetCore.Mvc;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.Models.API;
using SkiStore.API.Services.SkiStoreDB;

namespace SkiStore.API.Controllers;

[ApiController]
[Route("basket")]
public class BasketController : ControllerBase
{
    private readonly BasketService _basketService;
    private readonly ILogger<BasketController> _logger;
    public BasketController(BasketService basketService, ILogger<BasketController> logger)
    {
        _basketService = basketService;
        _logger = logger;
    }


    /// <summary>
    /// Get a Basket by Id
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/basket/a6a072b0-d676-4a7d-b56a-fb16ed85a889
    ///     
    /// </remarks>
    /// <param name="buyerId">ID of the Basket we want to get</param>
    /// <returns>Action Result with an model of an Basket</returns>
    /// <response code="200">The Basket was found and returned</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">The Basket was not found</response>
    /// <response code="500">General Error</response>
    [HttpGet("{buyerId}")]
    [ProducesResponseType(typeof(ActionResult<ReturnBasketDTO>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]

    public async Task<ActionResult<ReturnBasketDTO>> GetBasket(string buyerId)
    {
        try
        {
            APIResponse<ReturnBasketDTO> response = await _basketService.GetBasket(buyerId);
            if (response.IsSuccessful) return Ok(response.Data);
            else
            {
                _logger.LogError($"GET - basket/{buyerId} " + response.ErrorMessage);
                return Problem(response.ErrorMessage, $"GET - basket/{buyerId}", response.StatusCode, "Problem Getting Product By Id");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"GET - basket/{buyerId} " + ex.Message);
            return Problem(ex.Message, $"basket/{buyerId}", 500, "Problem Getting Basket By Id");
        }
    }

    [HttpPost("{buyerId}")]
    [ProducesResponseType(typeof(ActionResult<string>), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]

    public async Task<ActionResult<string>> AddItemToBasket([FromQuery] int productId, [FromQuery] int quantity, string? buyerId = "Undefined")
    {
        try
        {
            APIResponse<string> response = await _basketService.AddItemToBasket(buyerId, productId, quantity);

            if (response.IsSuccessful)
            {
                CookieOptions cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append(Settings.BUYERID, response.Data!, cookieOptions);
                return CreatedAtAction(nameof(AddItemToBasket), response.Data);
            }
            else
            {
                _logger.LogError($"POST - basket/{buyerId} " + response.ErrorMessage);
                return Problem(response.ErrorMessage, $"POST - basket/{buyerId}", response.StatusCode, "Problem Getting a Basket");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"POST - basket/{buyerId} " + ex.Message);
            return Problem(ex.Message, $"basket/{buyerId}", 500, "Problem Creating a Basket");
        }
    }

    /// <summary>
    /// Remove products from the basket
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/basket//a6a072b0-d676-4a7d-b56a-fb16ed85a889?productId=1&quantity=1
    ///     
    /// </remarks>
    /// <param name="buyerId">ID of the Basket we want to get</param>
    /// <param name="productId">ID of the Basket we want to get</param>
    /// <param name="quantity">ID of the Basket we want to get</param>
    /// <returns>Action Result with an model of an Basket</returns>
    /// <response code="200">The Basket was found and returned</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">The Basket was not found</response>
    /// <response code="500">General Error</response>
    [HttpDelete("{buyerId}")]
    [ProducesResponseType(typeof(ActionResult<string>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]

    public async Task<ActionResult<string>> RemoveBasketItem([FromQuery] int productId, [FromQuery] int quantity, string? buyerId = "Undefined")
    {

        try
        {
            APIResponse<string> response = await _basketService.RemoveBasketItem(buyerId, productId, quantity);

            if (response.IsSuccessful) return Ok(response.Data);
            else
            {
                _logger.LogError($"DELETE - basket/{buyerId} " + response.ErrorMessage);
                return Problem(response.ErrorMessage, $"POST - basket/{buyerId}", response.StatusCode, "Problem Deleting a Basket");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"DELETE - basket/{buyerId} " + ex.Message);
            return Problem(ex.Message, $"basket/{buyerId}", 500, "Problem Deleting a Basket");
        }
    }
}
