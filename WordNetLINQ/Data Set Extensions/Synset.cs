using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FlexibleRealization;

namespace WordNet.Linq
{
    public partial class Synset
    {
        public static Synset WithID(int synsetID) => WordNetData.Context.Synsets.Single(synset => synset.ID.Equals(synsetID));

        public bool MatchesPartOfSpeech(WordElementBuilder wordBuilder) => MatchesPartOfSpeech(WordNetData.PartOfSpeechMatching(wordBuilder));

        public bool MatchesPartOfSpeech(WordNetData.PartOfSpeech? partOfSpeech) => partOfSpeech switch
        {
            WordNetData.PartOfSpeech.Noun => POS.Equals('n'),
            WordNetData.PartOfSpeech.Verb => POS.Equals('v'),
            WordNetData.PartOfSpeech.Adjective => POS.Equals('a') || POS.Equals('s'),
            WordNetData.PartOfSpeech.Adverb => POS.Equals('r'),
            _ => false
        };

        public string GlossWithoutExamples
        {
            get
            {
                int indexOfFirstDoubleQuote = Gloss.IndexOf("\"");
                return indexOfFirstDoubleQuote > 0
                    ? Gloss
                        .Substring(0, indexOfFirstDoubleQuote)
                        .TrimEnd()
                        .TrimEnd(';')
                    : Gloss;
            }
        }

        private static readonly Regex QuotedSubstring = new Regex("\".*?\"");
        public IEnumerable<string> GlossExamples => QuotedSubstring.Matches(Gloss).Cast<Match>().Select(match => match.ToString());

        public IEnumerable<Synset> Hypernyms => WordNetData.Context.HypernymsOf(ID);

        public IEnumerable<Synset> Hyponyms => WordNetData.Context.HyponymsOf(ID);

        public IEnumerable<Synset> Holonyms => WordNetData.Context.HolonymsOf(ID);

        public IEnumerable<Synset> Meronyms => WordNetData.Context.MeronymsOf(ID);


    }
}
