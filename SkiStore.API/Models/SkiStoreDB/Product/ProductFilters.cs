namespace SkiStore.API.Models.SkiStoreDB.Product;

public class ProductFilters
{
    public List<string> Brands { get; set; } = new List<string>();

    public List<string> Types { get; set; } = new List<string>();
}
