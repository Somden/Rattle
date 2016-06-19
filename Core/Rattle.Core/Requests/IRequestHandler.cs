using Rattle.Core.Messages;

namespace Rattle.Core.Requests
{
    public interface IRequestHandler<T> where T : IRequest
    {
        IMessage Handle(T request);
    }
}
