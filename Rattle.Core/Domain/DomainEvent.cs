using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rattle.Core.Domain
{
    public abstract class DomainEvent
    {
        protected DomainEvent(Guid aggregateId)
        {
            Id = aggregateId;
        }

        public Guid Id { get; set; }
    }
}
