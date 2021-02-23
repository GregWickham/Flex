using System.Collections.Generic;

namespace WordNet.Linq
{
    public static class Synsets
    {
        public static IEnumerable<Synset> MatchingWordSpecification(WordSpecification wordSpecification) => wordSpecification.POS switch
        {
            WordNetData.PartOfSpeech.Noun => WordNetData.Context.SynsetsWithNounSenseMatching(wordSpecification.WordText),
            WordNetData.PartOfSpeech.Verb => WordNetData.Context.SynsetsWithVerbSenseMatching(wordSpecification.WordText),
            WordNetData.PartOfSpeech.Adjective => WordNetData.Context.SynsetsWithAdjectiveSenseMatching(wordSpecification.WordText),
            WordNetData.PartOfSpeech.Adverb => WordNetData.Context.SynsetsWithAdverbSenseMatching(wordSpecification.WordText),
            _ => WordNetData.Context.SynsetsWithSenseMatchingWord(wordSpecification.WordText)
        };
    }

}
