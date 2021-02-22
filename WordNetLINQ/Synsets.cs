using System.Collections.Generic;

namespace WordNet.Linq
{
    public static class Synsets
    {
        public static IEnumerable<Synset> MatchingWord(string word) => WordNetData.Context.SynsetsWithSenseMatchingWord(word);            
    }

}
