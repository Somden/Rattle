using Rattle.OAuth.Model;
using System.Collections.Generic;

namespace Rattle.OAuth.Data
{
    public interface IRepository<T>
    {
        IEnumerable<T> Get();
    }
}
