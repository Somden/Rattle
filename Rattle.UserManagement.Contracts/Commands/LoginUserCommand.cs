using Rattle.Core.Commands;

namespace Rattle.UserManagement.Contracts.Commands
{
    public class LoginUserCommand : ICommand
    {
        public LoginUserCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; private set; }

        public string Password { get; private set; }
    }
}
