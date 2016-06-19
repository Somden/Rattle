using Rattle.Core.Messages;
using System;
using Rattle.Core.Commands;
using Rattle.Core.Events;
using System.Collections.Generic;

namespace Rattle.Infrastructure
{
    public class HandlerInvoker : IHandlerInvoker
    {
        private readonly Dictionary<Type, object> m_handlers = new Dictionary<Type, object>();



        public void RegisterHandler<T>(ICommandHandler<T> handler) where T : ICommand
        {
            var commandType = typeof(T);
            if (!m_handlers.ContainsKey(commandType))
            {
                m_handlers.Add(commandType, handler);
            }
        }

        public void RegisterHandler<T>(IEventHandler<T> handler) where T : IEvent
        {
            var eventType = typeof(T);
            if (!m_handlers.ContainsKey(eventType))
            {
                m_handlers.Add(eventType, handler);
            }
        }


        public IMessage Handle(ICommand command)
        {
            var commandType = command.GetType();
            object handler;
            if (m_handlers.TryGetValue(commandType, out handler))
            {
                var responseObject = typeof(ICommandHandler<>).MakeGenericType(commandType)
                                                              .GetMethod("Handle")
                                                              .Invoke(handler, new[] { command });
                return responseObject as IMessage;
            }

            throw new InvalidOperationException("Correspondong handler not found.");
        }

        public void Handle(IEvent @event)
        {
            var eventType = @event.GetType();
            object handler;
            if (m_handlers.TryGetValue(eventType, out handler))
            {
                typeof(IEventHandler<>).MakeGenericType(eventType)
                                       .GetMethod("Handle")
                                       .Invoke(handler, new[] { @event });
                return;
            }

            throw new InvalidOperationException("Correspondong handler not found.");
        }
    }
}
