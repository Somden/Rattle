using System;

namespace Rattle.Infrastructure.EventStore
{
    public class SnapshotWrapper<T>
    {
        public string StreamName { get; set; }

        public T Snapshot { get; set; }

        public DateTime Created { get; set; }
    }
}