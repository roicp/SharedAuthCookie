namespace MvcCoreAuthentication.Models
{
    public class User
    {
        public User(string username)
        {
            Username = username;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; private set; }
    }
}
