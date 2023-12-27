namespace SkiStore.API.Models.SkiStoreDB.Basket;

public class BasketItem
{
    public int Id { get; set; } 

    public int Quantity { get; set; }

    public int ProductId { get; set; }

    public Models.SkiStoreDB.Product.Product Product { get; set; }
}
