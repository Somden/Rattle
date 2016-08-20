using Rattle.Core.Messages;

namespace Rattle.UserManagement.Contracts.DTO
{
    public class UserDTO : IMessage
    {
        public UserDTO(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }
}
