namespace WordNet.Linq
{
    public class WordSpecification
    {
        public WordSpecification(string wordText, WordNetData.PartOfSpeech partOfSpeech)
        {
            WordText = wordText;
            POS = partOfSpeech;
        }

        public string WordText { get; set; }

        public WordNetData.PartOfSpeech POS { get; set; }

    }
}
