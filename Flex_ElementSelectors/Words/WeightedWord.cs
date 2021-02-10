using System.Diagnostics;
using FlexibleRealization;

namespace Flex.ElementSelectors
{
    [DebuggerDisplay("{Text}")]
    public class WeightedWord : IWeightedWord
    {
        public WeightedWord(string word)
        {
            Text = word;
            Weight = DefaultWeight;
        }

        public WeightedWord(string word, int weight)
        {
            Text = word;
            Weight = weight;
        }

        public static readonly int DefaultWeight = 0x7FFF;

        public string Text { get; set; }

        public int Weight { get; set; }

        public int FlexDB_ID { get; set; }
    }
}
