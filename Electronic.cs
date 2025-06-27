public class Electronics : Product
{
    public string Brand { get; set; }
    public int WarrantyPeriod { get; set; }

    public Electronics(int id, string name, string desc, double price, int qty, string brand, int warranty)
        : base(id, name, desc, price, qty, "Electronics")
    {
        Brand = brand;
        WarrantyPeriod = warranty;
    }
}
