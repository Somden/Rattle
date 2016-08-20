using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Infrastructure.Services.TopologyStrategies
{
    public interface ITopology
    {
        ITopology Initialize();
        ITopology ListenForCommands(Action<BasicDeliverEventArgs> onCommand);
        ITopology ListenForEvents(Action<BasicDeliverEventArgs> onEvent);
    }
}
