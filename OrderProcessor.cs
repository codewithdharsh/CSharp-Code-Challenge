using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class OrderProcessor : IOrderManagementRepository
{
    private const string ConnectionName = "OrderDB";

    public void CreateUser(User user)
    {
        using (SqlConnection conn = DBConnUtil.GetConnection(ConnectionName))
        {
            conn.Open();
            string query = "INSERT INTO Users (UserId, Username, Password, Role) VALUES (@id, @username, @password, @role)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", user.UserId);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@role", user.Role);
            cmd.ExecuteNonQuery();

            Console.WriteLine("User created successfully.");
        }
    }

    public void CreateProduct(User user, Product product)
    {
        if (!user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Only admin users can create products.");
            return;
        }

        using (SqlConnection conn = DBConnUtil.GetConnection(ConnectionName))
        {
            conn.Open();
            string query = @"INSERT INTO Products 
                            (ProductId, ProductName, Description, Price, QuantityInStock, Type, Brand, WarrantyPeriod, Size, Color) 
                            VALUES 
                            (@id, @name, @desc, @price, @qty, @type, @brand, @warranty, @size, @color)";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", product.ProductId);
            cmd.Parameters.AddWithValue("@name", product.ProductName);
            cmd.Parameters.AddWithValue("@desc", product.Description);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@qty", product.QuantityInStock);
            cmd.Parameters.AddWithValue("@type", product.Type);

            cmd.Parameters.AddWithValue("@brand", GetProperty(product, "Brand") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@warranty", GetProperty(product, "WarrantyPeriod") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@size", GetProperty(product, "Size") ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@color", GetProperty(product, "Color") ?? DBNull.Value);

            cmd.ExecuteNonQuery();
            Console.WriteLine("Product created successfully.");
        }
    }

    public void CreateOrder(User user, List<Product> products)
    {
        using (SqlConnection conn = DBConnUtil.GetConnection(ConnectionName))
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserId = @uid", conn, transaction);
                checkUser.Parameters.AddWithValue("@uid", user.UserId);
                int count = (int)checkUser.ExecuteScalar();

                if (count == 0)
                {
                    SqlCommand insertUser = new SqlCommand("INSERT INTO Users (UserId, Username, Password, Role) VALUES (@id, @username, @password, @role)", conn, transaction);
                    insertUser.Parameters.AddWithValue("@id", user.UserId);
                    insertUser.Parameters.AddWithValue("@username", user.Username);
                    insertUser.Parameters.AddWithValue("@password", user.Password);
                    insertUser.Parameters.AddWithValue("@role", user.Role);
                    insertUser.ExecuteNonQuery();
                }

                SqlCommand insertOrder = new SqlCommand("INSERT INTO Orders (UserId) OUTPUT INSERTED.OrderId VALUES (@uid)", conn, transaction);
                insertOrder.Parameters.AddWithValue("@uid", user.UserId);
                int orderId = (int)insertOrder.ExecuteScalar();

                foreach (Product product in products)
                {
                    SqlCommand insertDetail = new SqlCommand("INSERT INTO OrderDetails (OrderId, ProductId, Quantity) VALUES (@oid, @pid, 1)", conn, transaction);
                    insertDetail.Parameters.AddWithValue("@oid", orderId);
                    insertDetail.Parameters.AddWithValue("@pid", product.ProductId);
                    insertDetail.ExecuteNonQuery();
                }

                transaction.Commit();
                Console.WriteLine("Order placed successfully. Order ID: " + orderId);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Failed to place order: " + ex.Message);
            }
        }
    }

    public void CancelOrder(int userId, int orderId)
    {
        using (SqlConnection conn = DBConnUtil.GetConnection(ConnectionName))
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserId = @uid", conn, transaction);
                checkUser.Parameters.AddWithValue("@uid", userId);
                if ((int)checkUser.ExecuteScalar() == 0) throw new UserNotFoundException("User not found.");

                SqlCommand checkOrder = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE OrderId = @oid AND UserId = @uid", conn, transaction);
                checkOrder.Parameters.AddWithValue("@oid", orderId);
                checkOrder.Parameters.AddWithValue("@uid", userId);
                if ((int)checkOrder.ExecuteScalar() == 0) throw new OrderNotFoundException("Order not found.");

                SqlCommand deleteDetails = new SqlCommand("DELETE FROM OrderDetails WHERE OrderId = @oid", conn, transaction);
                deleteDetails.Parameters.AddWithValue("@oid", orderId);
                deleteDetails.ExecuteNonQuery();

                SqlCommand deleteOrder = new SqlCommand("DELETE FROM Orders WHERE OrderId = @oid", conn, transaction);
                deleteOrder.Parameters.AddWithValue("@oid", orderId);
                deleteOrder.ExecuteNonQuery();

                transaction.Commit();
                Console.WriteLine("Order cancelled successfully.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Cancellation failed: " + ex.Message);
            }
        }
    }

    public List<Product> GetAllProducts()
    {
        return FetchProducts("SELECT * FROM Products", null);
    }

    public List<Product> GetOrderByUser(User user)
    {
        string query = @"SELECT P.* FROM Orders O
                          JOIN OrderDetails OD ON O.OrderId = OD.OrderId
                          JOIN Products P ON OD.ProductId = P.ProductId
                          WHERE O.UserId = @uid";

        SqlParameter[] parameters = { new SqlParameter("@uid", user.UserId) };
        return FetchProducts(query, parameters);
    }

    private List<Product> FetchProducts(string query, SqlParameter[] parameters)
    {
        List<Product> products = new List<Product>();
        using (SqlConnection conn = DBConnUtil.GetConnection(ConnectionName))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(MapReaderToProduct(reader));
            }
        }
        return products;
    }

    private Product MapReaderToProduct(SqlDataReader reader)
    {
        string type = reader["Type"].ToString().ToLower();
        if (type == "electronics")
        {
            return new Electronics(
                Convert.ToInt32(reader["ProductId"]),
                reader["ProductName"].ToString(),
                reader["Description"].ToString(),
                Convert.ToDouble(reader["Price"]),
                Convert.ToInt32(reader["QuantityInStock"]),
                reader["Brand"].ToString(),
                Convert.ToInt32(reader["WarrantyPeriod"])
            );
        }
        else if (type == "clothing")
        {
            return new Clothing(
                Convert.ToInt32(reader["ProductId"]),
                reader["ProductName"].ToString(),
                reader["Description"].ToString(),
                Convert.ToDouble(reader["Price"]),
                Convert.ToInt32(reader["QuantityInStock"]),
                reader["Size"].ToString(),
                reader["Color"].ToString()
            );
        }
        else
        {
            return new Product(
                Convert.ToInt32(reader["ProductId"]),
                reader["ProductName"].ToString(),
                reader["Description"].ToString(),
                Convert.ToDouble(reader["Price"]),
                Convert.ToInt32(reader["QuantityInStock"]),
                type
            );
        }
    }

    private object GetProperty(object obj, string propertyName)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        return prop?.GetValue(obj);
    }
}
