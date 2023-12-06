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
                    SuccessMessage = $"{products.Count} PRODUCTS INCOMING"
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
                    IsSuccessful = true
                };

                return APIResponse;
            }

        }

        public async Task<APIResponse<GetProductDTO>> GetProductById(int id)
        {

            try
            {
                Models.SkiStoreDB.Product.Product? raw_product = await context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();

                if(raw_product == null) 
                {
                    APIResponse<GetProductDTO> APIResponse = new()
                    {
                        StatusCode = 404,
                        ErrorMessage = $"- PRODUCT ID {id} NOT FOUND",
                        Data = null,
                        IsSuccessful = false
                    };

                    return APIResponse;
                }

                GetProductDTO product =mapper.Map<GetProductDTO>(raw_product);

                APIResponse<GetProductDTO> Final_APIResponse = new()
                {
                    StatusCode = 200,
                    SuccessMessage = $"GETTING PRODUCT ID {id}",
                    Data = product,
                    IsSuccessful = true
                };

                return Final_APIResponse;

            }
            catch (Exception ex) 
            {
                APIResponse<GetProductDTO> APIResponse = new()
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
