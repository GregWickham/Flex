using System.Diagnostics;

namespace Flex.ElementSelectors
{
    [DebuggerDisplay("{Word}")]
    public class WeightedWord
    {
        public WeightedWord(string word)
        {
            Word = word;
            Weight = 0x7FFF;
        }

        public string Word { get; set; }

        public int Weight { get; set; }

        public int FlexDB_ID { get; set; }
    }
}
