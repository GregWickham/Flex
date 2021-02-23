using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordNet.Linq
{
    public partial class Synset
    {
        public static Synset WithID(int synsetID) => WordNetData.Context.Synsets.Single(synset => synset.ID.Equals(synsetID));

        public bool IsNoun => WordNetData.IsNoun(POS);
        public bool IsVerb => WordNetData.IsVerb(POS);
        public bool IsAdjective => WordNetData.IsAdjective(POS);
        public bool IsAdverb => WordNetData.IsAdverb(POS);

        public string PartOfSpeechAndGloss => $"({WordNetData.UserFriendlyPartOfSpeechFrom(POS)}.) {GlossWithoutExamples}";

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
        public IEnumerable<Synset> Types => WordNetData.Context.TypesOf(ID);
        public IEnumerable<Synset> Instances => WordNetData.Context.InstancesOf(ID);


        #region Noun synsets only

        public IEnumerable<Synset> ValuesOfAttribute => WordNetData.Context.ValuesOfAttribute(ID);

        #endregion Noun synsets only


        #region Verb synsets only

        public IEnumerable<Synset> IsCausedBy => WordNetData.Context.CausesOf(ID);
        public IEnumerable<Synset> IsCauseOf => WordNetData.Context.CausedBy(ID);
        public IEnumerable<Synset> IsEntailedBy => WordNetData.Context.EntailersOf(ID);
        public IEnumerable<Synset> Entails => WordNetData.Context.EntailedBy(ID);
        public IEnumerable<VerbFramesForSynsetResult> VerbFrames => WordNetData.Context.VerbFramesForSynset(ID);
        public IEnumerable<Synset> VerbGroupMembers => WordNetData.Context.SynsetsInVerbGroupWith(ID);

        #endregion Verb synsets only


        #region Adjective synsets only

        public IEnumerable<Synset> Satellites => WordNetData.Context.SatellitesOf(ID);
        public IEnumerable<Synset> AttributesWithValue => WordNetData.Context.AttributesWithValue(ID);

        #endregion Adjective synsets only

        public IEnumerable<WordSense> WordSensesForSynset => WordNetData.Context.WordSensesForSynset(ID);
    }
}
