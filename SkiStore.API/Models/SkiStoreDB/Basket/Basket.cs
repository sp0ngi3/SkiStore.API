namespace SkiStore.API.Models.SkiStoreDB.Basket;

public class Basket
{
    public int Id { get; set; }

    public string BuyerId { get; set; }

    public List<BasketItem> Items = new();

    public void AddItem(Product.Product product , int quantity)
    {
        if (Items.All(item=>item.Id!=product.Id))
        {
            Items.Add(new BasketItem { Product = product, Quantity = quantity });
        }

        BasketItem? existingItem =Items.FirstOrDefault(x=>x.Product.Id==product.Id);

        if (existingItem!=null) 
        {
            existingItem.Quantity += quantity;
        }
    }

    public void RemoveItem(int productId, int quantity)
    {
        BasketItem? item = Items.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
        {
            return;
        }
        item.Quantity -= quantity;

        if (item.Quantity == 0) { Items.Remove(item); }
    }
}
