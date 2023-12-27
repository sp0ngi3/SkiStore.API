using AutoMapper;
using SkiStore.API.DTOs.SkiStoreDB.Basket;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Models.SkiStoreDB.Basket;
using SkiStore.API.Models.SkiStoreDB.Product;

namespace SkiStore.API.Extensions
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            // Product
            CreateMap<Product, GetProductDTO>().ReverseMap();

            // Basket

            CreateMap<Basket,ReturnBasketDTO>().ReverseMap();
            CreateMap<BasketItem,BasketItemDTO>().ReverseMap(); 
        }
    }
}
