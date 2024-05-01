using System.Collections.Concurrent;

namespace StringInterpolation.Core.Abstract
{
    public interface IInterpolationProvider
    {
        Task<ConcurrentDictionary<string, Dictionary<string, string>>> LoadAsync();
    }
}
