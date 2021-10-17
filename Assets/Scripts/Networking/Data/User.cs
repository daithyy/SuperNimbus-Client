public class User
{
    public string Email { get => email; private set => email = value; }
    public string Username { get => username; private set => username = value; }
    public string Password { get => password; private set => password = value; }

    private string email;

    private string username;

    private string password;

    public User(string email, string username, string password)
    {
        this.email = email;
        this.username = username;
        this.password = password;
    }
}
