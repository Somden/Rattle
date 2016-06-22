using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Rattle.Core.Domain;
using Rattle.Core.Events;

namespace Rattle.Infrastructure.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly Table _eventStreamTable;
        private readonly Table _eventWrapperTable;
        private readonly Table _snapshotWrapperTable;

        public EventStore(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;

            _eventStreamTable = Table.LoadTable(_amazonDynamoDb, "EventStream");
            _eventWrapperTable = Table.LoadTable(_amazonDynamoDb, "EventWrapper");
            _snapshotWrapperTable = Table.LoadTable(_amazonDynamoDb, "SnapshotWrapper");
        }

        //TODO: Merge with AppendEventsToStream
        public void CreateNewStream(string streamName, IEnumerable<IAggregateEvent> domainEvents)
        {
            var eventStream = new EventStream(streamName);
            _eventStreamTable.PutItem(eventStream.ToDocument());

            AppendEventsToStream(streamName, domainEvents);
        }

        public void AppendEventsToStream(string streamName, IEnumerable<IAggregateEvent> domainEvents)
        {
            EventStream stream = _eventStreamTable.GetItem(streamName).ToEventStream();

            var minEventsVersion = domainEvents.Min(e => e.Version);
            if (minEventsVersion <= stream.Version)
            {
                var error = $"Expected: {stream.Version + 1}. Found: {minEventsVersion}";
                throw new OptimsticConcurrencyException(error);
            }

            foreach (var @event in domainEvents)
            {
                _eventWrapperTable.PutItem(stream.RegisterEvent(@event).ToDocument());
            }

            _eventStreamTable.PutItem(stream.ToDocument());
        }

        public IEnumerable<IAggregateEvent> GetStream(string streamName, int fromVersion, int toVersion)
        {
            var filter = new QueryFilter("EventStreamId", QueryOperator.Equal, streamName);
            filter.AddCondition("EventNumber", QueryOperator.Between, fromVersion, toVersion);
            var queryResult = _eventWrapperTable.Query(filter).AsEnumerable(x => x.ToEventWrapper());

            var eventWrappers = (from stream in queryResult
                orderby stream.EventNumber
                select stream).ToList();

            if (!eventWrappers.Any()) return null;

            var events = new List<IAggregateEvent>();

            foreach (var @event in eventWrappers)
            {
                events.Add(@event.Event);
            }

            return events;
        }

        public void AddSnapshot<T>(string streamName, T snapshot)
        {
            var wrapper = new SnapshotWrapper<T>
            {
                StreamName = streamName,
                Snapshot = snapshot,
                Created = DateTime.Now
            };

            _snapshotWrapperTable.PutItem(wrapper.ToDocument());
        }

        public T GetLatestSnapshot<T>(string streamName) where T : class
        {
            var filter = new QueryFilter("StreamName", QueryOperator.Equal, streamName);
            var queryResult = _snapshotWrapperTable.Query(filter).AsEnumerable(x => x.ToSnapshotWrapper<T>());

            var latestSnapshot = queryResult
                .Where(x => x.StreamName == streamName)
                .OrderByDescending(x => x.Created)
                .FirstOrDefault();

            return (T) latestSnapshot?.Snapshot;
        }
    }

    public class OptimsticConcurrencyException : Exception
    {
        public OptimsticConcurrencyException(string message) : base(message) { }
    }
}
