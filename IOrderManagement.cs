using System.Collections.Generic;

public interface IOrderManagementRepository
{
    void CreateUser(User user);
    void CreateProduct(User user, Product product);
    void CreateOrder(User user, List<Product> products);
    void CancelOrder(int userId, int orderId);
    List<Product> GetAllProducts();
    List<Product> GetOrderByUser(User user);
}
