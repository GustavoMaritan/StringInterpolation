using StringInterpolation.Core.Domain;
using StringInterpolation.Core.Services;
using System.Collections.Concurrent;
using Xunit;

namespace StringReplacement.UnitTests
{
    public class ServiceTests
    {
        class DataTest
        {
            public DataTest()
            {

            }
            public DataTest(string? title, string? description, string? name)
            {
                Title = title;
                Description = description;
                Name = name;
            }

            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Name { get; set; }
        }

        private TextReplacementService _service = new();

        private ConcurrentDictionary<string, Dictionary<string, string>> _dictionary => new()
        {
            ["default"] = new()
            {
                { "partner", "xp" },
                { "name", "Xp Investimentos" },
                { "valor", "$ 10.000,51" },
                { "baseUrl", "xp-investimentos.com.br"},
            },
            ["whg"] = new()
            {
                { "partner", "whg" },
                { "name", "Whg Brasil" },
                { "valor", "R$ 90.000,52" },
                { "baseUrl", "whg-brasil.com.br" },
            }
        };

        private ConcurrentDictionary<string, Dictionary<string, string>> _dictionary2 => new()
        {
            ["default"] = new()
            {
                { "name", "XPInc" },
                { "partner", "xp" }
            },
            ["whg"] = new()
            {
                { "name", "Whg" },
                { "partner", "whg" },

            }
        };

        [Fact]
        public void ReplaceString_Without_Dictionary()
        {
            StorageValues.Load(new ConcurrentDictionary<string, Dictionary<string, string>>());

            var result = _service.ReplaceString("Bem vindo à {{ name }}", "whg");
            Assert.Equal("Bem vindo à {{ name }}", result);
        }

        [Fact]
        public void ReplaceString_Without_Keys()
        {
            StorageValues.Load(_dictionary ?? new());

            var result = _service.ReplaceString("Bem vindo à XP", "whg");

            Assert.Equal($"Bem vindo à XP", result);
        }

        [Fact]
        public void ReplaceString_Not_Exists_Keys()
        {
            StorageValues.Load(_dictionary ?? new());
            var result = _service.ReplaceString("Bem vindo à {{chave-nao-existe}}", "whg");
            Assert.Equal("Bem vindo à {{chave-nao-existe}}", result);
        }

        [Theory]
        [InlineData("XP")]
        [InlineData("Whg")]
        public void ReplaceString_With_Object(string name)
        {
            var result = _service.ReplaceString("Bem vindo à {{ name }}", new { name }, "{{  }}");
            Assert.Equal($"Bem vindo à {name}", result);
        }

        [Theory]
        [InlineData("xp")]
        [InlineData("WHG")]
        [InlineData("taler")]
        public void ReplaceString_With_Provider_Keys(string keyName)
        {
            StorageValues.Load(_dictionary2);

            var result = _service.ReplaceString("[[name]] Bem vindo à", keyName, "[[]]");

            if (keyName.Equals("xp") || keyName.Equals("taler"))
                Assert.Equal($"XPInc Bem vindo à", result);
            else
                Assert.Equal("Whg Bem vindo à", result);
        }

        [Theory]
        [InlineData("xp")]
        [InlineData("WHG")]
        [InlineData("taler")]
        public void ReplaceString_With_Provider_Keys22(string keyName)
        {
            StorageValues.Load(_dictionary2);

            var result = _service.ReplaceString("[[ name -l ]] Bem vindo à", keyName, "[[  ]]");

            if (keyName.Equals("xp") || keyName.Equals("taler"))
                Assert.Equal($"xpinc Bem vindo à", result);
            else
                Assert.Equal("whg Bem vindo à", result);
        }

        [Fact]
        public void ReplaceString_With_Provider_Keys2()
        {
            StorageValues.Load(_dictionary2);

            var result = _service.ReplaceString(
                "{{name -lower}} Teste a {{name}} utilizando opcoes Lower: {{name -l}}, Upper: {{name -u}} e Normal: {{name}}," +
                " e encontrando {{partner -upper}} varios casos {{name -l}} {{name}} {{name -upper}}", "xp");

            Assert.Equal($"xpinc Teste a XPInc utilizando opcoes Lower: xpinc, Upper: XPINC e Normal: XPInc, e encontrando XP varios casos xpinc XPInc XPINC", result);
        }

        [Fact]
        public void ReplaceString_With_Provider_Keys3()
        {
            StorageValues.Load(new ConcurrentDictionary<string, Dictionary<string, string>>
            {
                ["xp"] = new()
                {
                    { "college", "College" },
                    { "version", "1.10.32" },
                    { "city", "Virginia" },
                    { "year", "1914" },
                    { "rackham", "RACKHAM" },
                    { "teste-com", "going" },
                    { "teste-com-isso", "English" },
                    { "teste-com-upper", "translation" },
                    { "cpf", "311-851-707-51" },
                    { "cartao", "4444 5555 6666 7777" }
                }
            });

            var result = _service.ReplaceString(@"
                Where does it come from?
                Contrary to popular belief, Lorem Ipsum is not simply random text.
                It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, 
                a Latin professor at Hampden-Sydney {{college}} in {{city -u}}, looked up one of the more obscure Latin words, consectetur, 
                from a Lorem Ipsum passage, and {{teste-com}} through the cites of the word in classical literature, 
                discovered the undoubtable source. Lorem Ipsum comes from sections {{version}} and 1.10.33 of 
                ""de Finibus Bonorum et Malorum"" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a 
                treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, 
                ""Lorem ipsum dolor sit amet.."", comes from a line in section {{version}}.

                The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. 
                Sections 1.10.32 and 1.10.33 from ""de Finibus Bonorum et Malorum"" by Cicero are also reproduced in their exact original form, 
                accompanied by {{teste-com-isso}} versions from the {{year}} {{teste-com-upper -u}} by H. {{rackham -l}} {{cpf}} {{cpf -pw}}.", "xp");

            Assert.Equal(@"
                Where does it come from?
                Contrary to popular belief, Lorem Ipsum is not simply random text.
                It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, 
                a Latin professor at Hampden-Sydney College in VIRGINIA, looked up one of the more obscure Latin words, consectetur, 
                from a Lorem Ipsum passage, and going through the cites of the word in classical literature, 
                discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of 
                ""de Finibus Bonorum et Malorum"" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a 
                treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, 
                ""Lorem ipsum dolor sit amet.."", comes from a line in section 1.10.32.

                The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. 
                Sections 1.10.32 and 1.10.33 from ""de Finibus Bonorum et Malorum"" by Cicero are also reproduced in their exact original form, 
                accompanied by English versions from the 1914 TRANSLATION by H. rackham 311-851-707-51 **************.", result);
        }

        [Theory]
        [InlineData("xp")]
        [InlineData("whg")]
        [InlineData("taler")]
        public void ReplaceString_With_Provider_Keys_And_Object(string keyName)
        {
            StorageValues.Load(_dictionary ?? new());
            var result = _service.ReplaceString("Bem vindo à {{name}} - {{teste}}", keyName, new { teste = "valor para teste" });

            if (keyName.Equals("xp") || keyName.Equals("taler"))
                Assert.Equal($"Bem vindo à Xp Investimentos - valor para teste", result);

            else
                Assert.Equal($"Bem vindo à Whg Brasil - valor para teste", result);
        }

        //[Fact]
        public void ReplaceObject_With_Object()
        {
            var toReplace = new DataTest("{{title}}", "{{description}}", "{{name}}");

            var result = _service.Replace(toReplace, new { title = "Meu Titulo", description = "Minha descrição", name = "teste" });

            Assert.Equal("Meu Titulo", result?.Title);
            Assert.Equal("Minha descrição", result?.Description);
            Assert.Equal("teste", result?.Name);
        }

        //[Fact]
        public void ReplaceObject_With_Object_2()
        {
            var toReplace = new { title = "{{title -u}}", description = "{{description}}", name = "{{name}}" };

            var result = _service.Replace(toReplace, new { title = "Meu Titulo", description = "Minha descrição", name = "teste" });

            Assert.Equal("MEU TITULO", result?.title);
            Assert.Equal("Minha descrição", result?.description);
            Assert.Equal("teste", result?.name);
        }
    }
}
