using System;
using System.Collections.Generic;

public class MainModule
{
    public static void Main(string[] args)
    {
        IOrderManagementRepository orderRepo = new OrderProcessor();

        while (true)
        {
            Console.WriteLine("\n=== Order Management System ===");
            Console.WriteLine("1. Create User");
            Console.WriteLine("2. Create Product");
            Console.WriteLine("3. Create Order");
            Console.WriteLine("4. Cancel Order");
            Console.WriteLine("5. Get All Products");
            Console.WriteLine("6. Get Orders by User");
            Console.WriteLine("7. Exit");
            Console.Write("Enter your choice: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("User ID: ");
                        int userId = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        Console.Write("Role (Admin/User): ");
                        string role = Console.ReadLine();

                        User newUser = new User(userId, username, password, role);
                        orderRepo.CreateUser(newUser);
                        Console.WriteLine(" User created successfully.");
                        break;

                    case 2:
                        Console.Write("User ID (Admin): ");
                        int adminId = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Username: ");
                        string adminName = Console.ReadLine();
                        Console.Write("Password: ");
                        string adminPwd = Console.ReadLine();

                        User admin = new User(adminId, adminName, adminPwd, "Admin");

                        Console.Write("Product ID: ");
                        int pid = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Product Name: ");
                        string pname = Console.ReadLine();
                        Console.Write("Description: ");
                        string desc = Console.ReadLine();
                        Console.Write("Price: ");
                        double price = Convert.ToDouble(Console.ReadLine());
                        Console.Write("Quantity: ");
                        int qty = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Type (Electronics/Clothing): ");
                        string type = Console.ReadLine();

                        Product product = null;

                        if (type.ToLower() == "electronics")
                        {
                            Console.Write("Brand: ");
                            string brand = Console.ReadLine();
                            Console.Write("Warranty Period (in months): ");
                            int warranty = Convert.ToInt32(Console.ReadLine());
                            product = new Electronics(pid, pname, desc, price, qty, brand, warranty);
                        }
                        else if (type.ToLower() == "clothing")
                        {
                            Console.Write("Size: ");
                            string size = Console.ReadLine();
                            Console.Write("Color: ");
                            string color = Console.ReadLine();
                            product = new Clothing(pid, pname, desc, price, qty, size, color);
                        }
                        else
                        {
                            product = new Product(pid, pname, desc, price, qty, type);
                        }

                        orderRepo.CreateProduct(admin, product);
                        
                        break;

                    case 3:
                        Console.Write("User ID: ");
                        int uid = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Username: ");
                        string uname = Console.ReadLine();
                        Console.Write("Password: ");
                        string pwd = Console.ReadLine();

                        User orderUser = new User(uid, uname, pwd, "User");

                        List<Product> productList = new List<Product>();

                        Console.Write("How many products to order? ");
                        int count = Convert.ToInt32(Console.ReadLine());

                        for (int i = 0; i < count; i++)
                        {
                            Console.Write("Enter Product ID for item " + (i + 1) + ": ");
                            int prId = Convert.ToInt32(Console.ReadLine());

                      
                            productList.Add(new Product { ProductId = prId });
                        }

                        orderRepo.CreateOrder(orderUser, productList);
                        Console.WriteLine("Order created successfully.");
                        break;

                    case 4:
                        Console.Write("Enter User ID: ");
                        int cancelUserId = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Enter Order ID to cancel: ");
                        int cancelOrderId = Convert.ToInt32(Console.ReadLine());

                        orderRepo.CancelOrder(cancelUserId, cancelOrderId);
                        Console.WriteLine("Order cancelled successfully.");
                        break;

                    case 5:
                        List<Product> allProducts = orderRepo.GetAllProducts();
                        Console.WriteLine("\n All Products:");
                        foreach (Product prod in allProducts)
                        {
                            Console.WriteLine($"ID: {prod.ProductId}, Name: {prod.ProductName}, Price: {prod.Price}, Type: {prod.Type}");
                        }
                        break;

                    case 6:
                        Console.Write("Enter User ID to fetch orders: ");
                        int findUserId = Convert.ToInt32(Console.ReadLine());
                        User u = new User(findUserId, "", "", "User");

                        List<Product> orderedProducts = orderRepo.GetOrderByUser(u);
                        Console.WriteLine("\nOrders by User ID " + findUserId + ":");
                        foreach (Product p in orderedProducts)
                        {
                            Console.WriteLine($"Product ID: {p.ProductId}, Name: {p.ProductName}, Type: {p.Type}");
                        }
                        break;

                    case 7:
                        Console.WriteLine("Exiting Order Management System.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error: " + ex.Message);
            }
        }
    }
}
