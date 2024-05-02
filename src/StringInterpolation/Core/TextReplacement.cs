using StringInterpolation.Core.Domain;
using StringReplacement.Core.Domain;
using System.Text.RegularExpressions;

namespace StringInterpolation.Core
{
    internal static class TextReplacement
    {
        public static string WithSpan(InputText responseBody, Dictionary<string, string> replacements)
        {
            var result = WithSpanToChar(responseBody, replacements);
            return new string(result, 0, result.Length);
        }

        public static char[] WithSpanToChar(InputText responseBody, Dictionary<string, string> replacements)
        {
            ReadOnlySpan<char> original = responseBody.ToString();
            var keyPositions = KeyPositions(original, replacements);

            if (keyPositions.Count <= 0)
            {
                return original.ToArray();
            }

            var resultLength = CalculateResultLength(original.Length, keyPositions, replacements);
            var resultArray = ReplaceText(keyPositions, replacements, original, resultLength);

            return resultArray;

        }

        private static Dictionary<string, FormatValue> KeyPositions(
            ReadOnlySpan<char> original,
            Dictionary<string, string> replacements)
        {
            var keyPositions = new Dictionary<string, FormatValue>();
            foreach (var replacement in replacements.Keys)
            {
                FindAllOccurrences(original, replacement, keyPositions);
            }

            return keyPositions;
        }

        private static void FindAllOccurrences(
            ReadOnlySpan<char> span,
            string search,
            Dictionary<string, FormatValue> occurrences)
        {
            var regex = @"({{2})(" + search + ")(([ ]{1,})-[a-zA-Z0-9]{1,})?(}{2})";

            var matches = Regex.Matches(span.ToString(), regex);
            foreach (var item in matches.Cast<Match>())
            {
                if (!occurrences.TryGetValue(item.Value, out FormatValue value))
                {
                    var option = item.Groups[3].Value.Trim();
                    occurrences.Add(item.Value, FormatValue.GetInstance(option, search));
                    continue;

                }
                value.Count++;
            }
        }

        private static int CalculateResultLength(
            int originalLength,
            Dictionary<string, FormatValue> keyPositions,
            Dictionary<string, string> replacements)
        {
            int resultLength = originalLength;
            foreach (var item in keyPositions)
            {
                var ajustekey = item.Key.Length * item.Value.Count;
                var ajusteValue = replacements[item.Value.Search].Length * item.Value.Count;

                if (ajustekey > ajusteValue)
                    resultLength -= ajustekey - ajusteValue;
                else
                    resultLength += ajusteValue - ajustekey;
            }

            return resultLength;
        }

        private static char[] ReplaceText(
            Dictionary<string, FormatValue> keyPositions,
            Dictionary<string, string> replacements,
            ReadOnlySpan<char> original,
            int resultLength)
        {
            char[] resultArray = new char[resultLength];
            int resultIndex = 0;

            for (int i = 0; i < original.Length; i++)
            {
                var keyChanged = true;

                foreach (var item in keyPositions)
                {
                    string key = item.Key;

                    if (original[i..].StartsWith(key.AsSpan(), StringComparison.Ordinal))
                    {
                        string value = item.Value.GetValue(replacements);

                        value.AsSpan().CopyTo(resultArray.AsSpan()[resultIndex..]);
                        resultIndex += value.Length;

                        i += key.Length - 1;

                        keyChanged = true;

                        break;
                    }
                    else
                    {
                        keyChanged = false;
                    }
                }

                if (!keyChanged)
                {
                    resultArray[resultIndex++] = original[i];
                }
            }

            return resultArray;
        }
    }
}