public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public string Type { get; set; }

    public Product() { }

    public Product(int id, string name, string desc, double price, int qty, string type)
    {
        ProductId = id;
        ProductName = name;
        Description = desc;
        Price = price;
        QuantityInStock = qty;
        Type = type;
    }
}
