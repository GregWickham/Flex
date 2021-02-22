using System.Collections.Generic;
using System.Linq;

namespace WordNet.Linq
{
    public static class Senses
    {
        public static IEnumerable<WordSense> ForWord(string word) => WordNetData.Context.WordSenses
            .Where(sense => sense.WordText.Equals(word));
    }
}
