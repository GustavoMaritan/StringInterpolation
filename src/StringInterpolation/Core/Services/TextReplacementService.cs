using StringInterpolation.Core.Abstract;
using StringInterpolation.Core.Domain;
using StringReplacement.Core.Domain;
using System.Reflection;
using System.Text.Json;

namespace StringInterpolation.Core.Services
{
    public class TextReplacementService : ITextReplacementService
    {
        public string ReplaceString(InputText toReplace, object parameters)
        {
            var dic = ToDictionary(parameters);

            return TextReplacement.WithSpan(toReplace, dic);
        }

        public string ReplaceString(InputText toReplace, string keyName)
        {
            var keys = StorageValues.GetValue(keyName);

            return TextReplacement.WithSpan(toReplace, keys);
        }

        public string ReplaceString(InputText toReplace, string keyName, object parameters)
        {
            var dic = ToDictionary(parameters);

            return ReplaceString(toReplace, keyName, dic);
        }

        public string ReplaceString(InputText toReplace, string keyName, Dictionary<string, string> parameters)
        {
            if (!string.IsNullOrEmpty(keyName))
            {
                var keys = StorageValues.GetValue(keyName);

                foreach (var item in keys)
                {
                    if (!parameters.ContainsKey(item.Key))
                    {
                        parameters.Add(item.Key, item.Value);
                    }
                }
            }

            return TextReplacement.WithSpan(toReplace, parameters);

        }

        public T Replace<T>(string keyName, T toReplace, object parameters)
        {
            var dic = ToDictionary(parameters);

            InputText stringBody = JsonSerializer.Serialize(toReplace);

            var result = ReplaceString(stringBody, keyName, dic);

            return result != null ? JsonSerializer.Deserialize<T>(result) : default;
        }

        public T Replace<T>(T toReplace, object parameters)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, string> ToDictionary(object obj)
        {
            Dictionary<string, string> result = new();

            Type type = obj.GetType();
            var props = new List<PropertyInfo>(type.GetProperties());

            foreach (var prop in props)
            {
                string propValue = prop.GetValue(obj, null)?.ToString() ?? string.Empty;
                result.Add(prop.Name, propValue);
            }

            return result;
        }
    }
}
