using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore;
using SkiStore.API.DTOs.SkiStoreDB.Product;
using SkiStore.API.Models.SkiStoreDB;

namespace SkiStore.API.Models.API;

public class PagedList<T>:List<T>
{
    public PagedList(List<T> items , int count , int pageNumber , int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count ,
            PageSize=pageSize,
            CurentPage = pageNumber ,
            TotalPages=(int)Math.Ceiling(count/(double)pageSize) ,
        };
        AddRange(items);
    }

    public MetaData MetaData { get; set; }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query , int pageNumber , int pageSize)
    {
        int count = await query.CountAsync();

        List<T> items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items , count , pageNumber , pageSize);
    }

    public static async Task<PagedList<GetProductDTO>> ToProductPagedList(IQueryable<Product> query , int pageNumber , int pageSize, IMapper mapper)
    {
        int count = await query.CountAsync();

        List<Product> items= await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        List<GetProductDTO> list_of_products = mapper.Map<List<GetProductDTO>>(items);

        return new PagedList<GetProductDTO>(list_of_products, count , pageNumber , pageSize);   
    }
}
