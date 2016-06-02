namespace Rattle.OAuth.Model
{
    public class User
    {
        public User(string userName, string password)
        {
            this.Username = userName;
            this.Password = password;
        }

        public string Username { get; private set; }
                                       
        public string Password { get; private set; }
    }
}
