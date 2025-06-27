public static class DBPropertyUtil
{
    public static string GetConnectionString(string name)
    {
        return "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=OrderManagementSystem;Integrated Security=True";
    }
}
