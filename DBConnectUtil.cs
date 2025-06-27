using System.Data.SqlClient;

public static class DBConnUtil
{
    public static SqlConnection GetConnection(string name)
    {
        string connString = DBPropertyUtil.GetConnectionString(name);
        return new SqlConnection(connString);
    }
}
