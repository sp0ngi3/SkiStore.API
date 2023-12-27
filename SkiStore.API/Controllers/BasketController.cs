using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.Models.API;
using SkiStore.API.Services.SkiStoreDB;

namespace SkiStore.API.Controllers;

public class BasketController:BaseApiController
{
    private readonly BasketService basketService;
    public BasketController(SkiStoreContext context , IMapper mapper)
    {
        basketService=new BasketService(context,mapper);
    }

    [HttpGet]
    public async Task<ActionResult<APIResponse<ReturnBasketDTO>>> GetBasket()
    {
        CheckControllerData();

        try
        {
            APIResponse<ReturnBasketDTO> response = await basketService.GetBasket(Request.Cookies["buyerId"]!);

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

    [HttpPost]
    public async Task<ActionResult<APIResponse<string>>> AddItemToBasket(int productId , int quantity)
    {
        CheckControllerData();

        try
        {
            APIResponse<string> response = await basketService.AddItemToBasket(productId,quantity);
 
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

    [HttpDelete]
    public async Task<ActionResult<APIResponse<string>>> RemoveBasketItem(int productId, int quantity)
    {
        CheckControllerData();

        try
        {
            APIResponse<string> response = await basketService.RemoveBasketItem(productId, quantity);

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
