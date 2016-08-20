using Rattle.Core.Events;
using System.Collections.Generic;

namespace Rattle.Core.Domain
{
    public interface IEventStore
    {
        void CreateNewStream(string streamName, IEnumerable<IAggregateEvent> domainEvents);

        void AppendEventsToStream(string streamName, IEnumerable<IAggregateEvent> domainEvents);

        IEnumerable<IAggregateEvent> GetStream(string streamName, int fromVersion, int toVersion);

        void AddSnapshot<T>(string streamName, T snapshot);

        T GetLatestSnapshot<T>(string streamName) where T : class;
    }
}