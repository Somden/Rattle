using Rattle.Core.Events;
using Rattle.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rattle.Tests
{
    public class Program
    {
        private const string RabbitMqHost = "localhost";

        public static void Main(string[] args)
        {
            try
            {
                TestEventBus();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

        private static void TestEventBus()
        {
            IEventPublisher eventBus = new EventBus(RabbitMqHost);
            eventBus.Publish(new TestEvent {Id = Guid.NewGuid(), TimeStamp = DateTimeOffset.Now, Version = -1});
        }
    }

    public class TestEvent : IEvent
    {
        public Guid Id { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public int Version { get; set; }
    }
}
