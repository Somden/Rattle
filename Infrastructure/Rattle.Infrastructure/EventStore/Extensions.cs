using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Rattle.Core.Domain;

namespace Rattle.Infrastructure.EventStore
{
    public static class Extensions
    {
        public static string ToStreamName(this Type aggregateType, Guid id)
        {
            return $"{aggregateType.Name}-{id}";
        }

        public static string GetStreamName(this EventSourcedAggregate aggregate)
        {
            // Stream per-aggregate: {AggregateType}-{AggregateId}
            return $"{aggregate.GetType().Name}-{aggregate.Id}";
        }

        public static IEnumerable<T> AsEnumerable<T>(this Search search, Func<Document, T> map)
        {
            var eventStreamCollection = new List<T>();
            do
            {
                eventStreamCollection.AddRange(search.GetNextSet().Select(map));
            } while (!search.IsDone);
            return eventStreamCollection;
        }

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

        public static Document ToDocument<T>(this SnapshotWrapper<T> snapshotWrapper)
        {
            if (snapshotWrapper == null) throw new ArgumentNullException(nameof(snapshotWrapper));

            return new Document
            {
                ["StreamName"] = snapshotWrapper.StreamName,
                ["Snapshot"] = Document.FromJson(JsonConvert.SerializeObject(snapshotWrapper.Snapshot)),
                ["Created"] = snapshotWrapper.Created
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

        public static SnapshotWrapper<T> ToSnapshotWrapper<T>(this Document document)
        {
            return new SnapshotWrapper<T>
            {
                StreamName = document["StreamName"],
                Created = (DateTime) document["Created"],
                Snapshot = JsonConvert.DeserializeObject<T>(((Document) document["Snapshot"]).ToJson())
            };
        }
    }
}