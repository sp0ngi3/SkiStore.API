namespace SkiStore.API.Models.SkiStoreDB;

public class Basket
{
    public int Id { get; set; }

    public string BuyerId { get; set; }

    public List<BasketItem> Items { get; set; } = new();

    public void AddItem(Product product, int quantity)
    {
        if (!Items.All(item => item.ProductId != product.Id))
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null) existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new BasketItem { Product = product, Quantity = quantity });
            return;
        }
    }

    public void RemoveItem(int productId, int quantity = 1)
    {
        BasketItem? item = Items.Where(basketItem => basketItem.ProductId == productId).FirstOrDefault();
        if (item == null) return;
        item.Quantity -= quantity;
        if (item.Quantity == 0) Items.Remove(item);
    }
}
