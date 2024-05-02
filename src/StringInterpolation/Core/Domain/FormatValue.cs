namespace StringInterpolation.Core.Domain
{
    internal class FormatValue
    {
        public string Search { get; set; }
        public int Count { get; set; } = 1;

        public virtual string GetValue(Dictionary<string, string> replacements)
        {
            return replacements[Search];
        }

        public static FormatValue GetInstance(string option, string search)
        {
            return option switch
            {
                "-lower" or "-l" => new FormatToLower { Search = search },
                "-upper" or "-u" => new FormatToUpper { Search = search },
                "-password" or "-pw" => new FormatToPassword { Search = search },
                "-pascalCase" or "-pc" => new FormatToPascalCase { Search = search },
                _ => new FormatValue { Search = search },
            };
        }
    }

    internal class FormatToLower : FormatValue
    {
        public override string GetValue(Dictionary<string, string> replacements)
        {
            return replacements[Search].ToLower();
        }
    }

    internal class FormatToUpper : FormatValue
    {
        public override string GetValue(Dictionary<string, string> replacements)
        {
            return replacements[Search].ToUpper();
        }
    }

    internal class FormatToPassword : FormatValue
    {
        public override string GetValue(Dictionary<string, string> replacements)
        {
            var length = replacements[Search].Length;

            return "".PadLeft(length, '*');
        }
    }

    internal class FormatToPascalCase : FormatValue
    {
        public override string GetValue(Dictionary<string, string> replacements)
        {
            var value = replacements[Search].ToLower();

            return value[..1].ToUpper() + value[1..];
        }
    }
}
