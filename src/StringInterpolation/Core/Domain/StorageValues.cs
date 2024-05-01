using System.Collections.Concurrent;

namespace StringInterpolation.Core.Domain
{
    public static class StorageValues
    {
        private static ConcurrentDictionary<string, Dictionary<string, string>> _replacementKeys = new();

        public static void Load(Lazy<Task<ConcurrentDictionary<string, Dictionary<string, string>>>> lazy)
        {
            var keys = lazy.Value?.GetAwaiter().GetResult();
            _replacementKeys = keys ?? new();
        }

        public static void Load(ConcurrentDictionary<string, Dictionary<string, string>> keys)
        {
            _replacementKeys = keys ?? new();
        }

        public static bool Insert(string key, Dictionary<string, string> keys)
        {
            return _replacementKeys.TryAdd(key, keys);
        }

        public static Dictionary<string, string> GetValue(string key)
        {
            key = key?.ToLower();

            if (string.IsNullOrEmpty(key) || !_replacementKeys.TryGetValue(key, out var result))
            {
                if (!_replacementKeys.TryGetValue(InterpolationConfig.DefaulKey, out result))
                {
                    result = _replacementKeys.FirstOrDefault().Value;
                }
            }

            return result ?? new();
        }
    }
}
