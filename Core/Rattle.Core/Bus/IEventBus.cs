using Rattle.Core.Events;

namespace Rattle.Core.Bus
{
    public interface IEventBus
    {
        void SendEvent<T>(T @event) where T : IEvent;
    }
}
