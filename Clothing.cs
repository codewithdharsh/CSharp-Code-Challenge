public class Clothing : Product
{
    public string Size { get; set; }
    public string Color { get; set; }

    public Clothing(int id, string name, string desc, double price, int qty, string size, string color)
        : base(id, name, desc, price, qty, "Clothing")
    {
        Size = size;
        Color = color;
    }
}
