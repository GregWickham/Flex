using System.Diagnostics;

namespace Flex.ElementSelectors
{
    [DebuggerDisplay("{Text}")]
    public class WeightedWord
    {
        public WeightedWord(string word)
        {
            Text = word;
            Weight = 0x7FFF;
        }

        public WeightedWord(string word, int weight)
        {
            Text = word;
            Weight = weight;
        }

        public string Text { get; set; }

        public int Weight { get; set; }

        public int FlexDB_ID { get; set; }
    }
}
