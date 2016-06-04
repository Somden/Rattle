using Rattle.Core.Messages;

namespace Rattle.Core.Events
{
    public interface IEventHandler<in T> : IHandler<T> where T : IEvent
    { }
}