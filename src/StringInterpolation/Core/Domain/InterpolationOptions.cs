using StringInterpolation.Core.Abstract;

namespace StringInterpolation.Core.Domain
{
    public class InterpolationOptions
    {
        public InterpolationOptions SetHeaderName(string keyName)
        {
            InterpolationConfig.KeyName = keyName;
            return this;
        }

        public InterpolationOptions SetProvider<T>() where T : IInterpolationProvider
        {
            InterpolationConfig.Provider = typeof(T);
            return this;
        }

        public InterpolationOptions SetDefaultKey(string defaultKey)
        {
            InterpolationConfig.DefaulKey = defaultKey;
            return this;
        }

        public InterpolationOptions SetSearchKey(SearchKey searchKey, string keyName)
        {
            InterpolationConfig.KeyName = keyName;
            InterpolationConfig.SearchKey = searchKey;

            return this;
        }
    }
}
