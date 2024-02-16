using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkiStore.API.Context;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Extensions;
using SkiStore.API.Models.API;
using SkiStore.API.Models.SkiStoreDB;
using SkiStore.API.RequestHelpers;

namespace SkiStore.API.Services.SkiStoreDB
{
    public class ProductService
    {
        private readonly SkiStoreContext _context;
        private readonly IMapper _mapper;

        public ProductService(SkiStoreContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<APIResponse<PagedList<GetProductDTO>>> GetAllProducts(ProductParams productParams)
        {
            try
            {

                IQueryable<Product>
                    query = _context.Products
                    .Sort(productParams.OrderBy)
                    .Search(productParams.SearchTerm)
                    .Filter(productParams.Brands, productParams.Types)
                    .AsQueryable();

                PagedList<GetProductDTO> raw_products = await PagedList<Product>.ToProductPagedList(query, productParams.PageNumber, productParams.PageSize, _mapper);


                APIResponse<PagedList<GetProductDTO>> APIResponse = new()
                {
                    StatusCode = 200,
                    Data = raw_products,
                    IsSuccessful = true,
                    SuccessMessage = $"{raw_products.Count} PRODUCTS INCOMING"
                };

                return APIResponse;

            }
            catch (Exception ex)
            {
                APIResponse<PagedList<GetProductDTO>> APIResponse = new()
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
                Product? raw_product = await _context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (raw_product == null)
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

                GetProductDTO product = _mapper.Map<GetProductDTO>(raw_product);

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

        public async Task<APIResponse<ProductFilters>> GetProductFilters()
        {
            try
            {
                List<string> brands = await _context.Products.Select(x => x.Brand).Distinct().ToListAsync();
                List<string> types = await _context.Products.Select(x => x.Type).Distinct().ToListAsync();

                ProductFilters productFilters = new()
                {
                    Brands = brands,
                    Types = types
                };


                APIResponse<ProductFilters> APIResponse = new()
                {
                    StatusCode = 200,
                    SuccessMessage = "PRODUCT FILTERS SUCCESSFULLY INCOMING",
                    IsSuccessful = true,
                    Data = productFilters
                };

                return APIResponse;

            }
            catch (Exception ex)
            {
                APIResponse<ProductFilters> APIResponse = new()
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
