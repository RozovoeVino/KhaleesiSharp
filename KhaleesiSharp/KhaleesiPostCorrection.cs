using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KhaleesiSharp
{
    public class KhaleesiPostCorrection
    {
        private readonly Regex _pcRegex1 = new Regex(@"ийи", RegexOptions.IgnoreCase);
        private readonly Regex _pcRegex2 = new Regex(@"(и)й(и)", RegexOptions.IgnoreCase);
        private readonly Regex _pcRegex3 = new Regex(@"^(сьто|чьто)", RegexOptions.IgnoreCase);

        private readonly Random _random = new Random();

        public IEnumerable<string> PostCorrection(string[] words)
        {
            return words.Select((word, i) =>
            {
                if (word.Length < 2)
                    return word;

                if (_pcRegex1.IsMatch(word))
                    return _pcRegex2.Replace(word, "$1$2");

                if (_pcRegex3.IsMatch(word))
                {
                    var randomWhat = Whats[_random.Next(Whats.Length)];
                    return _pcRegex3.Replace(word, randomWhat);
                }

                return RandomMixWord(word);
            });
        }

        public string RandomMixWord(string word)
        {
            var mixedUpRules = PostCorrectionRules
                .OrderBy(x => _random.Next())
                .Take(10);

            foreach (var rule in mixedUpRules)
            {
                (string from, string to) = (rule[0], rule[1]);
                if (word.Contains(from))
                    word = word.Replace(from, to);
            }

            return word;
        }

        public string[] Whats { get; set; } = new[] { "чьто", "сто", "шьто", "што" };

        public string[][] PostCorrectionRules { get; set; } =
            new string[][]
            {
            new []{"ожк", "озьг"},
            new []{"кол", "га"},
            new []{"ко", "га"},
            new []{"колгот", "гагот"},
            new []{"шо", "ша"},
            new []{"дка", "ка"},
            new []{"он", "онь"},
            new []{"б", "п"},
            new []{"хи", "ни"},
            new []{"шк", "к"},
            new []{"тро", "го"},
            new []{"тка", "пка"},
            new []{"кров", "кав"},
            new []{"ра", "я"},
            new []{"дюк", "дю"},
            new []{"ойд", "анд"},
            new []{"дка", "та"},
            new []{"ро", "мо"},
            new []{"ны", "ни"},
            new []{"ре", "е"},
            new []{"ле", "не"},
            new []{"ки", "ке"},
            new []{"ш", "ф"},
            new []{"шка", "вха"},
            new []{"гри", "ги"},
            new []{"ч", "щ"},
            new []{"ре", "ле"},
            new []{"го", "хо"},
            new []{"ль", "й"},
            new []{"иг", "ег"},
            new []{"ра", "ва"},
            new []{"к", "г"},
            new []{"зо", "йо"},
            new []{"зо", "ё"},
            new []{"рё", "йо"},
            new []{"ск", "фк"},
            new []{"ры", "вы"},
            new []{"шо", "фо"},
            new []{"ло", "ле"},
            new []{"сы", "фи"},
            new []{"еня", "ея"},
            new []{"пель", "пюль"},
            new []{"а", "я"},
            new []{"у", "ю"},
            new []{"о", "ё"},
            new []{"ща", "ся"},
            new []{"ы", "и"},
            new []{"ри", "ви"},
            new []{"ло", "во"},
            new []{"е", "и"},
            new []{"и", "е"},
            new []{"а", "о"},
            new []{"о", "а"}
            };
    }
}