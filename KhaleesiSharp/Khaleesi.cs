namespace KhaleesiSharp
{
    public class Khaleesi
    {
        public KhaleesiEngine Engine { get; set; } = new KhaleesiEngine();
        public KhaleesiPostCorrection PostCorrection { get; set; } = new KhaleesiPostCorrection();

        public string Process(string message)
        {
            string[] words = KhaleesiUtils.GetWords(message.Trim());
            string[] result = new string[words.Length];

            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];

                if (word.Length < 2)
                    result[i] = word;
                else
                    result[i] = Engine.ReplaceWord(word);
            }

            return string.Join("", PostCorrection.PostCorrection(result));
        }
    }
}