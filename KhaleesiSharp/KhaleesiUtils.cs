using System.Text.RegularExpressions;

namespace KhaleesiSharp
{
    internal class KhaleesiUtils
    {
        private static Regex _splitRegex = new Regex(@"(\s+)");
        private static Regex _hasCyrillicsRegex = new Regex(@"[а-яё]", RegexOptions.IgnoreCase);

        public static string[] GetWords(string str) => _splitRegex.Split(str);

        public static bool HasCyrillics(string word) => _hasCyrillicsRegex.IsMatch(word);

        public static char[][] PreviousAndNext(string word)
        {
            var result = new char[word.Length][];

            for (var i = 0; i < word.Length; i++)
                result[i] = new char[] {
                i > 0 ? word[i - 1] : ' ',
                word[i],
                i + 1 < word.Length ? word[i + 1] : ' '
            };
            //result[i] = word.Substring(i - 1, 3);

            return result;
        }

        public static char ReplaceWithCase(char @char, char replacement)
        {
            if (char.IsUpper(@char))
                return char.ToUpper(replacement);
            else
                return char.ToLower(replacement);
        }
    }
}