using Rattle.Core.Commands;
using Rattle.Core.Messages;
using Rattle.UserManagement.Contracts.Commands;
using Rattle.UserManagement.Domain.Entities;

namespace Rattle.UserManagement.Handlers.Commands
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
    {
        public IMessage Handle(RegisterUserCommand command)
        {
            var user = User.Create(command.Username, command.Password);
            return user.ToDTO();
        }
    }
}
