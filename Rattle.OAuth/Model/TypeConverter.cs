using IdentityServer4.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Rattle.OAuth.Model
{
    public static class TypeConverter
    {
        public static InMemoryUser ToInMemoryUser(this User user)
        {
            if(user == null)
            {
                throw new ArgumentException("User cannot be null");
            }

            return new InMemoryUser
            {
                Username = user.Username,
                Password = user.Password,
                Claims = new List<Claim>(),
                Enabled = true,
                Subject = user.Username
            };
        } 
    }
}
