namespace Flex.ElementSelectors
{
    public class WeightedWord
    {
        public WeightedWord(string word)
        {
            Word = word;
            Weight = 0x7FFF;
        }

        public string Word { get; set; }
        public int Weight { get; set; }
    }
}
