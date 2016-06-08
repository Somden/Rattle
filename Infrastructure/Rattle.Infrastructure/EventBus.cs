using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Rattle.Core.Events;

namespace Rattle.Infrastructure
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class EventBus : IEventPublisher
    {
        private const string BusName = "EventBus";

        private readonly string _connectionString;

        public EventBus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            var factory = new ConnectionFactory() { HostName = _connectionString };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var message = JsonConvert.SerializeObject(@event);
                    var bytes = Encoding.UTF8.GetBytes(message);
                    channel.ExchangeDeclare(BusName, "fanout");
                    channel.BasicPublish(BusName, string.Empty, null, bytes);
                }
            }
        }
    }
}
