using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Models.API;

namespace SkiStore.API.Services.SkiStoreDB;

public class BasketService
{
    private readonly SkiStoreContext context;
    private readonly IMapper mapper;

    public BasketService(SkiStoreContext context , IMapper mapper )
    {
        this.context = context;
        this.mapper = mapper;
    }


    
    public async Task<APIResponse<ReturnBasketDTO>> GetBasket(string buyerID)
    {
        try
        {
            APIResponse<ReturnBasketDTO> APIResponse = new();

            if (buyerID == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"- BUYER ID MISSING";

                return APIResponse;
            }

            Models.SkiStoreDB.Basket.Basket? basket = await context.Baskets.Where(x => x.BuyerId == buyerID)
                .Include(x=>x.Items)
                .ThenInclude(x=>x.Product)
                .FirstOrDefaultAsync();

            if(basket == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 404;
                APIResponse.ErrorMessage = $"- BASKET NOT FOUND";

                return APIResponse;
            }

            ReturnBasketDTO basketDTO =mapper.Map<ReturnBasketDTO>(basket);

            APIResponse.IsSuccessful = true;
            APIResponse.StatusCode = 200;
            APIResponse.SuccessMessage = $"BASKET FOUND";
            APIResponse.Data = basketDTO;


            return APIResponse;
        }
        catch (Exception ex) 
        {
            APIResponse<ReturnBasketDTO> APIResponse = new()
            {
                StatusCode = 500,
                ErrorMessage = "-" + ex.Message,
                Data = null,
                IsSuccessful = true
            };

            return APIResponse;
        }
    }

    

    public async Task<APIResponse<string>> AddItemToBasket(int productId , int quantity)
    {
        APIResponse<string> APIResponse = new();
        return APIResponse;
       
    }

    public async Task<APIResponse<string>> RemoveBasketItem(int productId ,int quantity)
    {
        APIResponse<string> APIResponse = new();
        return APIResponse;
    }
}
