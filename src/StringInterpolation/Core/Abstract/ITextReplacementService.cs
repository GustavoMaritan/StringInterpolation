using StringReplacement.Core.Domain;

namespace StringInterpolation.Core.Abstract
{
    public interface ITextReplacementService
    {
        string ReplaceString(InputText toReplace, object parameters);
        string ReplaceString(InputText toReplace, string keyName);
        string ReplaceString(InputText toReplace, string keyName, object parameters);
        string ReplaceString(InputText toReplace, string keyName, Dictionary<string, string> parameters);
        T Replace<T>(T toReplace, object parameters);
        T Replace<T>(string keyName, T toReplace, object parameters);
    }
}
