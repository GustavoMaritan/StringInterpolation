using StringInterpolation.Core;
using StringReplacement.Core.Domain;

namespace StringInterpolation.Extensions
{
    public static class Extensions
    {
        public static string ReplaceWithSpan(this InputText toReplace, Dictionary<string, string> parameters)
        {
            return TextReplacement.WithSpan(toReplace, parameters);
        }
    }
}
