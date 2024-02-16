namespace SkiStore.API.DTOs.SkiStoreDB.Basket;

public class ReturnBasketDTO
{
    public int Id { get; set; }

    public string BuyerId { get; set; }

    public List<BasketItemDTO> Items { get; set; } = new();
}
