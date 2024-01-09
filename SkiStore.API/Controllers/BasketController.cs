using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.Models.API;
using SkiStore.API.Services.SkiStoreDB;

namespace SkiStore.API.Controllers;

public class BasketController : BaseApiController
{
    private readonly BasketService basketService;
    public BasketController(SkiStoreContext context, IMapper mapper)
    {
        basketService = new BasketService(context, mapper);
    }

    [HttpGet("basket/{buyerId}")]
    public async Task<ActionResult<APIResponse<ReturnBasketDTO>>> GetBasket(string buyerId)
    {
        CheckControllerData();

        try
        {
            APIResponse<ReturnBasketDTO> response = await basketService.GetBasket(buyerId);

            ProvideLog(response.GetResponseInfo());

            return DetermineResponseType(response);
        }
        catch (Exception ex)
        {
            APIResponse<string> Response = new()
            {
                IsSuccessful = false,
                StatusCode = 500,
                Data = null,
                ErrorMessage = ex.Message
            };

            ProvideLog(Response.GetResponseInfo());
            return StatusCode(500, Response);
        }
    }

    [HttpPost("basket")]
    public async Task<ActionResult<APIResponse<string>>> AddItemToBasket([FromQuery] int productId, [FromQuery] int quantity)
    {
        CheckControllerData();

        try
        {
            APIResponse<string> response = await basketService.AddItemToBasket(Request.Cookies["buyerId"]!,productId,quantity);

            if(response.IsSuccessful) 
            {
                CookieOptions cookieOptions =new CookieOptions { IsEssential = true , Expires=DateTime.Now.AddDays(30)};
                Response.Cookies.Append(StaticValues.StaticValues.BUYERID,response.Data!,cookieOptions);
            }
 
            ProvideLog(response.GetResponseInfo());

            return DetermineResponseType(response);
        }
        catch (Exception ex) 
        {
            APIResponse<string> Response = new()
            {
                IsSuccessful = false,
                StatusCode = 500,
                Data = null,
                ErrorMessage = ex.Message
            };

            ProvideLog(Response.GetResponseInfo());
            return StatusCode(500, Response);
        }
    }

    [HttpDelete("basket")]
    public async Task<ActionResult<APIResponse<bool>>> RemoveBasketItem([FromQuery] int productId, [FromQuery] int quantity)
    {
        CheckControllerData();

        try
        {
            APIResponse<bool> response = await basketService.RemoveBasketItem(Request.Cookies["buyerId"]!, productId, quantity);

            ProvideLog(response.GetResponseInfo());

            return DetermineResponseType(response);
        }
        catch (Exception ex)
        {
            APIResponse<string> Response = new()
            {
                IsSuccessful = false,
                StatusCode = 500,
                Data = null,
                ErrorMessage = ex.Message
            };

            ProvideLog(Response.GetResponseInfo());
            return StatusCode(500, Response);
        }
    }
}
