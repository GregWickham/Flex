using System.Collections.Generic;
using System.Linq;

namespace WordNet.Linq
{
    public static class Senses
    {
        public static IEnumerable<Sense> ForWord(string word) => WordNetData.Context.Senses
            .Where(sense => sense.WordText.Equals(word));
    }
}
