using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Rattle.Core.Domain;

namespace Rattle.Infrastructure
{
    public class EventStore : IEventStore
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        
        public EventStore(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
        }

        public void CreateNewStream(string streamName, IEnumerable<DomainEvent> domainEvents)
        {
            var eventStreamTable = Table.LoadTable(_amazonDynamoDb, "EventStream");
            var eventStream = new EventStream(streamName);
            eventStreamTable.PutItem(eventStream.ToDocument());

            AppendEventsToStream(streamName, domainEvents);
        }

        public void AppendEventsToStream(string streamName, IEnumerable<DomainEvent> domainEvents, int? expectedVersion = null)
        {
            var eventStreamTable = Table.LoadTable(_amazonDynamoDb, "EventStream");
            EventStream stream = eventStreamTable.GetItem(streamName).ToEventStream();

            if (expectedVersion != null)
            {
                CheckForConcurrencyError(expectedVersion, stream);
            }

            var eventWrapperTable = Table.LoadTable(_amazonDynamoDb, "EventWrapper");
            foreach (var @event in domainEvents)
            {
                eventWrapperTable.PutItem(stream.RegisterEvent(@event).ToDocument());
            }

            eventStreamTable.PutItem(stream.ToDocument());
        }

        public IEnumerable<DomainEvent> GetStream(string streamName, int fromVersion, int toVersion)
        {
            var eventWrapperTable = Table.LoadTable(_amazonDynamoDb, "EventWrapper");

            var queryResult = eventWrapperTable.Scan(new ScanFilter()).AsEnumerable();

            // var queryResult = eventWrapperTable.Query(streamName, new Expression()).AsEnumerable();
            var eventWrappers = (from stream in queryResult
                where stream.EventNumber <= toVersion
                      && stream.EventNumber >= fromVersion
                orderby stream.EventNumber
                select stream).ToList();

            if (!eventWrappers.Any()) return null;

            var events = new List<DomainEvent>();

            foreach (var @event in eventWrappers)
            {
                events.Add(@event.Event);
            }

            return events;
        }

        public void AddSnapshot<T>(string streamName, T snapshot)
        {
            throw new NotImplementedException();
        }

        public T GetLatestSnapshot<T>(string streamName) where T : class
        {
            throw new NotImplementedException();
        }

        private static void CheckForConcurrencyError(int? expectedVersion, EventStream stream)
        {
            var lastUpdatedVersion = stream.Version;
            if (lastUpdatedVersion != expectedVersion)
            {
                var error = $"Expected: {expectedVersion}. Found: {lastUpdatedVersion}";
                throw new OptimsticConcurrencyException(error);
            }
        }
    }

    public static class QueryExt
    {
        public static IEnumerable<EventWrapper> AsEnumerable(this Search search)
        {
            var eventStreamCollection = new List<EventWrapper>();
            do
            {
                eventStreamCollection.AddRange(search.GetNextSet().Select(x => x.ToEventWrapper()));
            } while (!search.IsDone);
            return eventStreamCollection;
        }
    }

    public class OptimsticConcurrencyException : Exception
    {
        public OptimsticConcurrencyException(string message) : base(message) { }
    }

    public static class Extensions
    {
        public static Document ToDocument(this EventStream eventStream)
        {
            return new Document
            {
                ["Id"] = eventStream.Id,
                ["Version"] = eventStream.Version,
            };
        }

        public static Document ToDocument(this EventWrapper eventWrapper)
        {
            return new Document
            {
                ["Id"] = eventWrapper.Id,
                ["EventNumber"] = eventWrapper.EventNumber,
                ["EventStreamId"] = eventWrapper.EventStreamId,
                ["Event"] = Document.FromJson(JsonConvert.SerializeObject(eventWrapper.Event)),
                ["EventType"] = eventWrapper.Event.GetType().ToString()
            };
        }

        public static EventStream ToEventStream(this Document document)
        {
            string id = document["Id"];
            var version = (int)document["Version"];
            return new EventStream {Version = version, Id = id};
        }

        public static EventWrapper ToEventWrapper(this Document document)
        {
            var eventVersion = (int) document["EventNumber"];
            var eventStreamId = (string) document["EventStreamId"];
            var eventType = (string) document["EventType"];
            var eventJson = ((Document)document["Event"]).ToJson();
            var deserializeObject = JsonConvert.DeserializeObject(eventJson, Type.GetType($"{eventType}, Rattle.Domain"));
            return
                new EventWrapper(
                    (DomainEvent)
                        deserializeObject,
                    eventVersion, eventStreamId);
        }
    }
}
