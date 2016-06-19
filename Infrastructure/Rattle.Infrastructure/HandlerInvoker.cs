using Rattle.Core.Messages;
using System;
using Rattle.Core.Commands;
using Rattle.Core.Events;
using System.Collections.Generic;
using Rattle.Core.Requests;

namespace Rattle.Infrastructure
{
    public class HandlerInvoker : IHandlerInvoker
    {
        private readonly Dictionary<Type, Func<ICommand, IMessage>> m_commandHandlers = new Dictionary<Type, Func<ICommand, IMessage>>();
        private readonly Dictionary<Type, Func<IRequest, IMessage>> m_requestHandlers = new Dictionary<Type, Func<IRequest, IMessage>>();
        private readonly Dictionary<Type, Action<IEvent>> m_eventHandlers = new Dictionary<Type, Action<IEvent>>();




        public void RegisterHandler<T>(ICommandHandler<T> handler) where T : ICommand
        {
            var commandType = typeof(T);
            if (!m_commandHandlers.ContainsKey(commandType))
            {
                m_commandHandlers.Add(commandType, this.CreateHandler(handler));
            }
        }

        public void RegisterHandler<T>(IRequestHandler<T> handler) where T : IRequest
        {
            var requestType = typeof(T);
            if (!m_requestHandlers.ContainsKey(requestType))
            {
                m_requestHandlers.Add(requestType, this.CreateHandler(handler));
            }
        }

        public void RegisterHandler<T>(IEventHandler<T> handler) where T : IEvent
        {
            var eventType = typeof(T);
            if (!m_eventHandlers.ContainsKey(eventType))
            {
                m_eventHandlers.Add(eventType, this.CreateHandler(handler));
            }
        }




        public IMessage Handle(ICommand command)
        {
            var commandType = command.GetType();
            Func<ICommand, IMessage> handler;
            if (m_commandHandlers.TryGetValue(commandType, out handler))
            {
                return handler(command);
            }

            throw new InvalidOperationException("Correspondong handler not found.");
        }

        public IMessage Handle(IRequest request)
        {
            var requestType = request.GetType();
            Func<IRequest, IMessage> handler;
            if (m_requestHandlers.TryGetValue(requestType, out handler))
            {
                return handler(request);
            }

            throw new InvalidOperationException("Correspondong handler not found.");
        }

        public void Handle(IEvent @event)
        {
            var eventType = @event.GetType();
            Action<IEvent> handler;
            if (m_eventHandlers.TryGetValue(eventType, out handler))
            {
                handler(@event);
                return;
            }

            throw new InvalidOperationException("Correspondong handler not found.");
        }





        private Func<ICommand, IMessage> CreateHandler<T>(ICommandHandler<T> commandHandler) 
            where T : ICommand
        {
            return command => commandHandler.Handle((T)command);
        }


        private Func<IRequest, IMessage> CreateHandler<T>(IRequestHandler<T> requestHandler)
            where T : IRequest
        {
            return request => requestHandler.Handle((T)request);
        }


        private Action<IEvent> CreateHandler<T>(IEventHandler<T> eventHandler)
            where T : IEvent
        {
            return @event => eventHandler.Handle((T)@event);
        }
    }
}
