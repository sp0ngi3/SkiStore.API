using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Models.API;

namespace SkiStore.API.Services.SkiStoreDB
{
    public class ProductService
    {
        private readonly SkiStoreContext context;
        private readonly IMapper mapper;

        public ProductService(SkiStoreContext context , IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<APIResponse<List<GetProductDTO>>> GetAllProducts()
        {
            try 
            {
                List<Models.SkiStoreDB.Product.Product> raw_Products = await context.Products.ToListAsync();

                List<GetProductDTO> products =mapper.Map<List<GetProductDTO>>(raw_Products);


                APIResponse<List<GetProductDTO>> APIResponse = new()
                {
                    StatusCode = 200,
                    Data = products,
                    IsSuccessful = true,
                    SuccessMessage = $"{products.Count} STANDORTE GELIEFERT"
                };

                return APIResponse;

            }
            catch (Exception ex)
            {
                APIResponse<List<GetProductDTO>> APIResponse = new()
                {
                    StatusCode = 500,
                    ErrorMessage = "-" + ex.Message,
                    Data = null,
                    IsSuccessful = false
                };

                return APIResponse;
            }

        }
    }
}
