using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rattle.Core.Domain;

namespace Rattle.Domain.Drinks
{
    public class Drink : EventSourcedAggregate
    {
        public override void Apply(DomainEvent @event)
        {
            When((dynamic)@event);
            Version = Version++;
        }

        private void Causes(DomainEvent @event)
        {
            Changes.Add(@event);
            Apply(@event);
        }

        private void When(DomainEvent @event)
        {
            
        }
    }
}
