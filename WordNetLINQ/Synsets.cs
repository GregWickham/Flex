using System.Collections.Generic;
using System.Linq;

namespace WordNet.Linq
{
    public static class Synsets
    {
        public static IEnumerable<Synset> MatchingWord(string word) => WordNetData.Context.Senses
            .Where(sense => sense.WordText.Equals(word))
            .Select(sense => WordNetData.Context.Synsets.Single(synset => synset.ID.Equals(sense.SynsetID)));
    }

    public partial class Synset
    {
        public IEnumerable<Sense> WordSenses => WordNetData.Context.Senses
            .Where(sense => sense.SynsetID.Equals(ID));
    }
}
