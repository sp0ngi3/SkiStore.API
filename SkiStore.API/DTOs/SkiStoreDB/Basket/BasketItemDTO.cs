namespace SkiStore.API.DTOs.SkiStoreDB.Basket;

public class BasketItemDTO
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int ProductId { get; set; }

    public DTOs.SkiStoreDB.Product.GetProductDTO Product { get; set; }
}
