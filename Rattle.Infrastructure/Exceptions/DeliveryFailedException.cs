using System;
using RabbitMQ.Client.Events;

namespace Rattle.Infrastructure.Exceptions
{
    public class DeliveryFailedException : Exception
    {
        public DeliveryFailedException(BasicDeliverEventArgs args)
        {
            Args = args;
        }

        public BasicDeliverEventArgs Args { get; }
    }
}
