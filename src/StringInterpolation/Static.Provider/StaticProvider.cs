using StringInterpolation.Core.Abstract;
using StringReplacement.Core.Domain;
using System.Collections.Concurrent;

namespace StringInterpolation.Static.Provider
{
    public class StaticProvider : IInterpolationProvider
    {
        public Task<ConcurrentDictionary<string, Dictionary<string, string>>> LoadAsync()
        {
            return Task.FromResult(DataStaticProvider.KeyValues);
        }
    }
}
