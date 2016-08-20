using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Rattle.Core.Aggregates;
using Rattle.UserManagement.Contracts.Events;

namespace Rattle.UserManagement.Domain.Entities
{
    public class User : Aggregate
    {
        private User()
        {
        }

        public string Username { get; private set; }

        public string PasswordHash { get; private set; }

        public string Salt { get; private set; }



        public static User Create(string userName, string password)
        {
            var user = new User();

            user.ApplyEvent(new UserCreatedEvent(user.Id, 1, userName, password));

            return user;
        }


        public void ChangeUsername(string userName)
        {
            ApplyEvent(new UsernameChangedEvent(Id, this.Version + 1, userName));
        }




        #region Event Handlers

        public void Apply(UserCreatedEvent @event)
        {
            this.Username = @event.Username;
            this.Salt = this.CreateSalt();
            this.PasswordHash = this.HashPassword(@event.Password, this.Salt);
        }

        public void Apply(UsernameChangedEvent @event)
        {
            this.Username = @event.Username;
        }

        #endregion




        private string CreateSalt()
        {
            using (var randGenerator = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[32];
                randGenerator.GetBytes(buffer);

                return Convert.ToBase64String(buffer);
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256Managed = new SHA256Managed())
            {
                var saltBytes = Convert.FromBase64String(salt);
                var passwordBytes = Encoding.UTF8.GetBytes(password);

                var plainTextWithSalt = passwordBytes.Concat(saltBytes).ToArray();

                var hash = sha256Managed.ComputeHash(plainTextWithSalt);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
