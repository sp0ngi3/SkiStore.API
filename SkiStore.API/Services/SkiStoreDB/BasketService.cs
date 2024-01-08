using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog.Web.LayoutRenderers;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Models.API;
using SkiStore.API.Models.SkiStoreDB.Basket;

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

            Models.SkiStoreDB.Basket.Basket? basket =  await FetchBasket(buyerID);

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

    

    public async Task<APIResponse<string>> AddItemToBasket(string buyerID ,int productId , int quantity)
    {
        try
        {
            APIResponse<string> APIResponse = new();

            Models.SkiStoreDB.Basket.Basket? basket = await FetchBasket(buyerID) ?? CreateBasket();

            Models.SkiStoreDB.Product.Product? product =await context.Products.FindAsync(productId);

            if(product == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 404;
                APIResponse.ErrorMessage = $"- PRODUCT ID {productId} NOT FOUND";

                return APIResponse;
            }

            basket.AddItem(product,quantity);

            bool changesMade = await context.SaveChangesAsync() > 0;

            if(changesMade) 
            {
                APIResponse.IsSuccessful = true;
                APIResponse.StatusCode = 201;
                APIResponse.SuccessMessage = $"ITEM {productId} ADDED TO BASKET";
                APIResponse.Data = basket.BuyerId.ToString();  

                return APIResponse;
            }
            else 
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"- No changes have been made in the DB";

                return APIResponse;
            }

        }
        catch (Exception ex)
        {
            APIResponse<string> APIResponse = new()
            {
                StatusCode = 500,
                ErrorMessage = "-" + ex.Message,
                Data = null,
                IsSuccessful = true
            };

            return APIResponse;
        }
    }

    public async Task<APIResponse<bool>> RemoveBasketItem(string buyerID , int productId ,int quantity)
    {
        try
        {
            APIResponse<bool> APIResponse = new();

            if (buyerID == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"- BUYER ID MISSING";

                return APIResponse;
            }

            Basket basket = await FetchBasket(buyerID);

            if (basket==null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 404;
                APIResponse.ErrorMessage = $"- BASKET NOT FOUND";
            }

            basket.RemoveItem(productId,quantity);

            bool changesMade = await context.SaveChangesAsync() > 0;

            if(changesMade) 
            {
                APIResponse.IsSuccessful = true;
                APIResponse.StatusCode = 200;
                APIResponse.SuccessMessage = $"ITEM {productId} REMOVED FROM THE BASKET";
                APIResponse.Data = true;
                return APIResponse;
            }
            else
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"- NO CHANGES HAVE BEEN MADE";
                return APIResponse;
            }

            
        }
        catch (Exception ex) 
        {
            APIResponse<bool> APIResponse = new()
            {
                StatusCode = 500,
                ErrorMessage = "-" + ex.Message,
                Data = false,
                IsSuccessful = true
            };

            return APIResponse;
        }
    }

   private async Task<Basket> FetchBasket(string BuyerID)
    {
        if(string.IsNullOrWhiteSpace(BuyerID))
        {
            return null;
        }
        
            return await context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .Where(x => x.BuyerId == BuyerID)
                .FirstOrDefaultAsync();
       
    }

    private Basket CreateBasket()
    {
        string buyerId = Guid.NewGuid().ToString();

        //CookieOptions cookieOptions =new CookieOptions {IsEssential=true,Expires=DateTime.Now.AddDays(30)};
        //Reponse.Cookies.Append(StaticValues.StaticValues.BUYERID,buyerId,cookieOptions)

        Basket basket = new Basket { BuyerId = buyerId };

        context.Baskets.Add(basket);

        return basket;
    }
}
