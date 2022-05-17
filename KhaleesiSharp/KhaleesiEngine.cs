using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KhaleesiSharp
{
    public class KhaleesiEngine
    {
        public Dictionary<char, KhaleesiReplaceRule[]> GlobalReplaces { get; set; }
            = new Dictionary<char, KhaleesiReplaceRule[]>();

        public string Vovels { get; set; } = "аеёиоуыэюя";
        public string Consonants { get; set; } = "йцкнгшщзхфвпрлджбтмсч";

        /// <summary>
        /// <para>@ означает текущую букву</para>
        /// <para>^ и $ - начало/конец слова (как в регулярных выражениях)</para>
        /// <para>С и Г - любая согласная/гласная буквы</para>
        /// <para>до знака "=" у нас искомый паттерн, а после знака – на что мы будем заменять эту букву</para>
        /// <para>Символ точки – любая буква или символ</para>
        /// </summary>
        public Dictionary<char, string[]> ReplaceRules = new Dictionary<char, string[]>()
    {
        {
            'а', new[]
            {
                "^ @ .  = @",
                "[тбвкпмнг]@$  = я",
                ". @ $  = @",
                ". @ я  = @",
                "С @ .  = я",
                "Г @ .  = я",
            }
        },
        {
            'в', new[]
            {
                "з @ . = ь@",
            }
        },
        {
            'е', new[]
            {
                ". @ С + $ = @",
                "С @ .   = и",
                "Г @ .   = и",
            }
        },
        {
            'ж', new[]
            {
                ". @ . = з",
            }
        },
        {
            'л', new[]
            {
                "^ @ . = @",
                ". @ $ = @",
                ". @р$ = @",
                "л @ . = @@",
                ". @к  = @",
                ". @п  = @",
                "С @ . = @",
                "Г @ . = _",
            }
        },
        {
            'н', new[]
            {
                "ко@$ = н",
            }
        },
        {
            'о', new[]
            {
                "[мпжзгтс]@[цкнгшщзхфвпджбтмсч] = ё",
            }
        },
        {
            'р', new[]
            {
                "^дра = _",
                "^ @ . = л",
                "Г @ . = й",
                "С @ . = ь",
            }
        },
        {
            'у', new[]
            {
                "^ @ . = @",
                ". @ . = ю",
            }
        },
        {
            'ч', new[]
            {
                "^что = сь",
            }
        },
        {
            'щ', new[]
            {
                "^тыщ$ = сь",
                "^ @ . = @",
                ". @ . = с",
            }
        },
        {
            'ь', new[]
            {
                "л@ .  = й",
                ". @ $ = @",
                ".@Г$  = @",
                "С@ .  = @",
                ". @ . = й",
            }
        },
    };


        public Dictionary<char, KhaleesiReplaceRule[]> GetReplaces()
        {
            return ReplaceRules.ToDictionary(x => x.Key, x => x.Value
                .Select((stringPattern) => stringPattern.Split('='))
                .Select((stringPattern) =>
                {
                    var regexpPatternSB = new StringBuilder();
                    (var search, var replacement) = (stringPattern[0], stringPattern[1]);
                    replacement = replacement.Trim().Replace('@', x.Key);

                    if (replacement == "_")
                        replacement = "";

                    regexpPatternSB.Append('(');
                    foreach (var element in search.Replace(" ", ""))
                    {
                        if (element == '@')
                            regexpPatternSB.Append($")({x.Key})(");
                        else if (element == 'Г')
                            regexpPatternSB.Append($"[{Vovels}]");
                        else if (element == 'С')
                            regexpPatternSB.Append($"[{Consonants}]");
                        else
                            regexpPatternSB.Append(element);
                    }
                    regexpPatternSB.Append(')');

                    var regexp = new Regex(regexpPatternSB.ToString(), RegexOptions.IgnoreCase);

                    return new KhaleesiReplaceRule(regexp, replacement);
                }).ToArray());

            //foreach ((char @char, var stringPatterns) in ReplaceRules)
            //{
            //    //var tripples = new KhaleesiReplaceRule[stringPatterns.Length];

            //    var tripples = stringPatterns
            //        .Select((stringPattern) => stringPattern.Split('='))
            //        .Select((stringPattern) =>
            //        {
            //            var regexpPatternSB = new StringBuilder();
            //            (var search, var replacement) = (stringPattern[0], stringPattern[1]);
            //            replacement = replacement.Trim().Replace('@', @char);

            //            if (replacement == "_")
            //                replacement = "";

            //            regexpPatternSB.Append('(');
            //            foreach (var element in search.Replace(" ", ""))
            //            {
            //                if (element == '@')
            //                    regexpPatternSB.Append($")({@char})(");
            //                else if (element == 'Г')
            //                    regexpPatternSB.Append($"[{Vovels}]");
            //                else if (element == 'С')
            //                    regexpPatternSB.Append($"[{Consonants}]");
            //                else
            //                    regexpPatternSB.Append(element);
            //            }
            //            regexpPatternSB.Append(')');

            //            var regexp = new Regex(regexpPatternSB.ToString(), RegexOptions.IgnoreCase);

            //            return new KhaleesiReplaceRule(regexp, replacement);
            //        });
            //}
        }

        public string ReplaceWord(string word)
        {
            if (GlobalReplaces.Count == 0)
                GlobalReplaces = GetReplaces();

            if (!KhaleesiUtils.HasCyrillics(word))
                return word;

            var result = new StringBuilder();

            foreach (var group in KhaleesiUtils.PreviousAndNext(word))
            {
                var prevChar = group[0];
                var currentChar = group[1];
                var nextChar = group[2];
                var lowerCurrentChar = char.ToLower(currentChar);

                if (GlobalReplaces.ContainsKey(lowerCurrentChar))
                {
                    var replaceChar = ReplaceChar(prevChar, currentChar, nextChar, lowerCurrentChar);
                    if (replaceChar != null)
                        result.Append(replaceChar);
                }
                else
                    result.Append(currentChar);
            }

            return result.ToString();
        }

        public char? ReplaceChar(char prevChar, char currentChar, char nextChar, char lowerCurrentChar)
        {
            var tr = new string(new[] { prevChar, currentChar, nextChar }).Trim();
            foreach (var tripple in GlobalReplaces[lowerCurrentChar])
            {
                if (tripple.Regex.IsMatch(tr))
                {
                    var result = tripple.Regex.Replace(tr, tripple.Replacement);
                    if (result.Length == 0)
                        return null;
                    return KhaleesiUtils.ReplaceWithCase(
                        currentChar, result[0]);
                }
            }
            return currentChar;
        }
    }
}