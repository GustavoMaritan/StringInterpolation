namespace StringInterpolation.Core.Domain
{
    internal static class InterpolationConfig
    {
        public static string DefaulKey = "default";
        public static Type Provider = null;

        public static string KeyName = "partner";
        public static SearchKey SearchKey = SearchKey.Header;
    }

    public enum SearchKey
    {
        Null,
        Header,
        Route,
        Query
    }
}
