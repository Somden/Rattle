using Rattle.Core.Messages;

namespace Rattle.Core.Events
{
    public interface IEventHandler<in T> where T : IEvent
    {
        void Handle(T @event);
    }
}