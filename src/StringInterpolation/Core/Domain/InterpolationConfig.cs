namespace StringInterpolation.Core.Domain
{
    internal static class InterpolationConfig
    {
        public static string DefaulKey = "default";
        public static Type Provider = null;

        public static string KeyName = "partner";
        public static SearchKey SearchKey = SearchKey.Header;

        private static InterpolationPattern _keyPattern = new("{{", "}}");

        public static InterpolationPattern KeyPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return _keyPattern;
            }

            return new InterpolationPattern(pattern);
        }

        public static void SetPattern(string pattern)
        {
            _keyPattern = new(pattern);
        }
    }

    public enum SearchKey
    {
        Null,
        Header,
        Route,
        Query
    }

    internal class InterpolationPattern
    {
        public string Left { get; internal set; }
        public string Right { get; internal set; }

        private string _leftWithEscape;

        public InterpolationPattern(string pattern)
        {
            var length = pattern.Length / 2;

            Left = pattern[..length];
            Right = pattern[length..];

            LeftWithEscape();
        }

        public InterpolationPattern(string left, string right)
        {
            Left = left;
            Right = right;

            LeftWithEscape();
        }

        public string Get(string value)
        {
            return Left + value + Right;
        }

        public int Length(string value)
        {
            return Get(value).Length;
        }

        private void LeftWithEscape()
        {
            var addEscape = "";

            foreach (var item in Left.AsSpan())
            {
                addEscape += @"\" + item;
            }

            _leftWithEscape = addEscape;
        }

        public string Regex(string key)
        {
            return @"(" + _leftWithEscape + ")(" + key + ")(([ ]{1,})-[a-zA-Z0-9]{1,})?(" + Right + ")";
        }
    }
}
