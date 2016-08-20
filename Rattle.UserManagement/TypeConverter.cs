using Rattle.UserManagement.Contracts.DTO;
using Rattle.UserManagement.Domain.Entities;

namespace Rattle.UserManagement
{
    internal static class TypeConverter
    {
        public static UserDTO ToDTO(this User user)
        {
            return new UserDTO(user.Username);
        }
    }
}
