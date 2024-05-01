using StringInterpolation.Core.Abstract;
using System.Collections.Concurrent;

namespace ApiTests
{
    public class MeuProvider : IInterpolationProvider
    {
        public Task<ConcurrentDictionary<string, Dictionary<string, string>>> LoadAsync()
        {
            var result = new ConcurrentDictionary<string, Dictionary<string, string>>()
            {
                ["default"] = new()
            {
                { "chave", "Valor Default" },
                { "chave1", "Valor Default 1" },
                { "chave2", "Valor Default 2" }
            },
                ["gustavo"] = new()
            {
                { "chave", "Valor item" },
                { "chave1", "Valor item1" },
                { "chave2", "Valor item2" },
            },
                ["gm"] = new()
            {
                { "chave", "Valor gm item" },
                { "chave1", "Valor gm item1" },
                { "chave2", "Valor gm item2" },
            }
            };

            return Task.FromResult(result);
        }
    }
}
