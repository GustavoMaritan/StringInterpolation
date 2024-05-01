using System.Collections.Concurrent;

namespace StringReplacement.Core.Domain
{
    internal static class DataStaticProvider
    {
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> concurrentDictionary = new()
        {
            ["default"] = new()
            {
                { "chave", "Valor Default" },
                { "chave1", "Valor Default 1" },
                { "chave2", "Valor Default 2" }
            },
            ["item1"] = new()
            {
                { "chave", "Valor item" },
                { "chave1", "Valor item1" },
                { "chave2", "Valor item2" },
            }
        };

        public static ConcurrentDictionary<string, Dictionary<string, string>> KeyValues => concurrentDictionary;
    }
}
