using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.Models.API;
using SkiStore.API.Models.SkiStoreDB;

namespace SkiStore.API.Services.SkiStoreDB;

public class BasketService
{
    private readonly SkiStoreContext _context;
    private readonly IMapper _mapper;

    public BasketService(SkiStoreContext context, IMapper mapper)
    {
        this._context = context;
        this._mapper = mapper;
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

            Basket? basket = await FetchBasket(buyerID);

            if (basket == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 404;
                APIResponse.ErrorMessage = $"- BASKET NOT FOUND";

                return APIResponse;
            }

            ReturnBasketDTO basketDTO = _mapper.Map<ReturnBasketDTO>(basket);

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

    public async Task<APIResponse<string>> AddItemToBasket(string buyerID, int productId, int quantity)
    {
        try
        {
            APIResponse<string> APIResponse = new();

            Basket? basket = await FetchBasket(buyerID) ?? CreateBasket();

            Product? product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 404;
                APIResponse.ErrorMessage = $"- PRODUCT ID {productId} NOT FOUND";

                return APIResponse;
            }

            basket.AddItem(product, quantity);

            bool changesMade = await _context.SaveChangesAsync() > 0;

            if (changesMade)
            {
                APIResponse.IsSuccessful = true;
                APIResponse.StatusCode = 201;
                APIResponse.SuccessMessage = $"PRODUCT ID :      {productId} ADDED TO BASKET";
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

    public async Task<APIResponse<string>> RemoveBasketItem(string buyerID, int productId, int quantity)
    {
        try
        {
            APIResponse<string> APIResponse = new();

            if (buyerID == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 400;
                APIResponse.ErrorMessage = $"- BUYER ID MISSING";

                return APIResponse;
            }

            Basket basket = await FetchBasket(buyerID);

            if (basket == null)
            {
                APIResponse.IsSuccessful = false;
                APIResponse.StatusCode = 404;
                APIResponse.ErrorMessage = $"- BASKET NOT FOUND";
            }

            basket!.RemoveItem(productId, quantity);

            bool changesMade = await _context.SaveChangesAsync() > 0;

            if (changesMade)
            {
                APIResponse.IsSuccessful = true;
                APIResponse.StatusCode = 200;
                APIResponse.SuccessMessage = $"PRODUCT ID:  {productId} REMOVED FROM THE BASKET";
                APIResponse.Data = $"PRODUCT ID :  {productId} REMOVED FROM THE BASKET";
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

    private async Task<Basket> FetchBasket(string BuyerID)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(BuyerID) || BuyerID == null || BuyerID.ToLower() == "undefined")
            {
                return null;
            }

            return await _context.Baskets
           .Include(i => i.Items)
           .ThenInclude(p => p.Product)
           .FirstOrDefaultAsync(basket => basket.BuyerId == BuyerID);

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private Basket CreateBasket()
    {
        string buyerId = Guid.NewGuid().ToString();

        //CookieOptions cookieOptions =new CookieOptions {IsEssential=true,Expires=DateTime.Now.AddDays(30)};
        //Reponse.Cookies.Append(StaticValues.StaticValues.BUYERID,buyerId,cookieOptions)

        Basket basket = new() { BuyerId = buyerId };

        _context.Baskets.Add(basket);

        return basket;
    }
}
