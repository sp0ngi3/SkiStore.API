using Microsoft.AspNetCore.Mvc;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Extensions;
using SkiStore.API.Models.API;
using SkiStore.API.Models.SkiStoreDB.Product;
using SkiStore.API.RequestHelpers;
using SkiStore.API.Services.SkiStoreDB;
using System.Text.Json;

namespace SkiStore.API.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get an Products
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/products
        ///     
        /// </remarks>
        /// <param name="OrderBy">Order By Descending or Ascending Price</param>
        /// <param name="SearchTerm">Search for the Name of the Product</param>
        /// <param name="Types">Get products with specific Types</param>
        /// <param name="Brands">Get products with specific Brands</param>
        /// <param name="PageNumber">The Number of the page from all available pages</param>
        /// <param name="PageSize">The size of the page</param>
        /// <returns>Action Result with an list of Products</returns>
        /// <response code="200">The Products were found and returned</response>
        /// <response code="500">General Error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<PagedList<GetProductDTO>>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<ActionResult<PagedList<GetProductDTO>>> GetAllProducts([FromQuery] ProductParams productParams)
        {
            try
            {
                APIResponse<PagedList<GetProductDTO>> response = await _productService.GetAllProducts(productParams);
                if (response.IsSuccessful)
                {
                    Response.AddPaginationHeader(response.Data!.MetaData);
                    return Ok(response.Data);
                }
                else
                {
                    _logger.LogError($"GET - products/ " + response.ErrorMessage);
                    return Problem(response.ErrorMessage, $"GET - products/", response.StatusCode, "Problem Getting Product By Id");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GET - products " + ex.Message);
                return Problem(ex.Message, "products", 500, "Problem Getting Products");
            }
        }

        /// <summary>
        /// Get an Product by Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/products/1
        ///     
        /// </remarks>
        /// <param name="productID">ID of the Product we want to get</param>
        /// <returns>Action Result with an model of an Product</returns>
        /// <response code="200">The Product was found and returned</response>
        /// <response code="404">The Product was not found</response>
        /// <response code="500">General Error</response>

        [HttpGet("{productID}")]
        [ProducesResponseType(typeof(ActionResult<GetProductDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<ActionResult<GetProductDTO>> GetProductById(int productID)
        {
            try
            {
                APIResponse<GetProductDTO> response = await _productService.GetProductById(productID);
                if (response.IsSuccessful) return Ok(response.Data);
                else
                {
                    _logger.LogError($"GET - products/{productID} " + response.ErrorMessage);
                    return Problem(response.ErrorMessage, $"GET - products/{productID}", response.StatusCode, "Problem Getting Product By Id");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET - products/{productID} " + ex.Message);
                return Problem(ex.Message, $"products/{productID}", 500, "Problem Getting Product By Id");
            }
        }

        /// <summary>
        /// Get an Product Filters
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/products/filters
        ///     
        /// </remarks>
        /// <returns>Action Result with an model of an Product Filters</returns>
        /// <response code="200">The Product Filters were found and returned</response>
        /// <response code="500">General Error</response>
        [HttpGet("filters")]
        [ProducesResponseType(typeof(ActionResult<ProductFilters>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<ActionResult<ProductFilters>> GetProductFilters()
        {
            try
            {
                APIResponse<ProductFilters> response = await _productService.GetProductFilters();
                if (response.IsSuccessful) return Ok(response.Data);
                else
                {
                    _logger.LogError("products/filters " + response.ErrorMessage);
                    return Problem(response.ErrorMessage, "products/filters", response.StatusCode, "Problem Getting Product Filters");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("products/filters " + ex.Message);
                return Problem(ex.Message, $"products/filters", 500, "Problem Getting Product Filters");
            }
        }
    }
}
