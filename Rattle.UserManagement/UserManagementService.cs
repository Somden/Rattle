using System;
using System.Reflection;
using Autofac;
using Rattle.Infrastructure.Services;
using Rattle.UserManagement.Contracts.DTO;
using Rattle.UserManagement.Handlers.Commands;

namespace Rattle.UserManagement
{
    public class UserManagementService : Service
    {
        public UserManagementService(string name) : base(name)
        {
        }



        protected override Assembly[] ContractsAssemblies => new[] {typeof(UserDTO).Assembly};
        protected override Assembly[] HandlersAssemblies => new[] { typeof(RegisterUserCommandHandler).Assembly };


        protected override void RegisterDependencies(ContainerBuilder containerBuilder)
        {
        }
    }
}
