using System.Text.RegularExpressions;

namespace KhaleesiSharp
{
    public class KhaleesiReplaceRule
    {
        public KhaleesiReplaceRule() { }

        public KhaleesiReplaceRule(Regex regex, string replacement)
        {
            Regex = regex;
            Replacement = replacement;
        }

        public Regex Regex { get; set; }
        public string Replacement { get; set; }
    }
}