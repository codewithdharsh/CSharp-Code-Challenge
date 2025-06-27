public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public User(int id, string uname, string pwd, string role)
    {
        UserId = id;
        Username = uname;
        Password = pwd;
        Role = role;
    }
}
