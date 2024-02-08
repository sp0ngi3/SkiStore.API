namespace SkiStore.API.RequestHelpers;

public class ProductParams : PaginationParams
{
    public string? OrderBy { get; set; } = null;

    public string? SearchTerm { get; set; } = null;

    public string? Types { get; set; } = null;

    public string? Brands { get; set; } = null;
}
