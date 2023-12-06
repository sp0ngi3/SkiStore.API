using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Models.API;
using SkiStore.API.Services.SkiStoreDB;

namespace SkiStore.API.Controllers
{
    public class ProductsController:BaseApiController
    {
        private readonly ProductService productService;
        public ProductsController(SkiStoreContext context , IMapper mapper)
        {
            productService=new ProductService(context,mapper);
        }

        [HttpGet("products")]
        public async Task <ActionResult<APIResponse<List<GetProductDTO>>>> GetAllProducts()
        {
            CheckControllerData();
            try 
            {
                APIResponse<List<GetProductDTO>> response = await productService.GetAllProducts();

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

        [HttpGet("products/{id}")]
        public async Task<ActionResult<APIResponse<GetProductDTO>>> GetProductById(int id)
        {
            CheckControllerData();
            try 
            {
                APIResponse<GetProductDTO> response = await productService.GetProductById(id);

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
}
