using Microsoft.EntityFrameworkCore;
using SkiStore.API.Models.SkiStoreDB.Product;

namespace SkiStore.API.Context
{
    public class SkiStoreContext:DbContext
    {
       public SkiStoreContext(DbContextOptions options):base(options) 
        {
           
        }

        public DbSet<Product> Products { get; set; }    
      
    }
}
