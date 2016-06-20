using System;

namespace Rattle.Domain.UserAccounts
{
    public class UserAccountSnapshot
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        public string UserName { get; set; }
    }
}