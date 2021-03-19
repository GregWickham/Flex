using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordNet.Linq
{
    public partial class Synset
    {
        public static Synset WithID(int synsetID) => WordNetData.Context.Synsets.Single(synset => synset.ID.Equals(synsetID));
        public IEnumerable<WordSense> WordSensesForSynset => WordNetData.Context.WordSensesForSynset(ID);

        public bool IsNoun => WordNetData.IsNoun(POS);
        public bool IsVerb => WordNetData.IsVerb(POS);
        public bool IsAdjective => WordNetData.IsAdjective(POS);
        public bool IsAdjectiveHead => WordNetData.IsAdjectiveHead(POS);
        public bool IsAdjectiveSatellite => WordNetData.IsAdjectiveSatellite(POS);
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

        /// <summary>This Regex is used for extracting quoted substrings from Synset glosses.</summary>
        private static readonly Regex QuotedSubstring = new Regex("\".*?\"");
        public IEnumerable<string> GlossExamples => QuotedSubstring.Matches(Gloss).Cast<Match>().Select(match => match.ToString());


        #region Related synsets that apply to all parts of speech

        public IEnumerable<Synset> Hypernyms => WordNetData.Context.HypernymsOf(ID);
        public void AddHypernym(Synset newHypernym)
        {
            if (WordNetData.Context.AddHyponymyRelation(newHypernym.ID, ID).Equals(0))
            {
                SendPropertyChanged("Hypernyms");
                newHypernym.SendPropertyChanged("Hyponyms");
            }
        }
        public void DeleteHypernym(Synset existingHypernym)
        {
            if (WordNetData.Context.DeleteHyponymyRelation(existingHypernym.ID, ID).Equals(0))
            {
                SendPropertyChanged("Hypernyms");
                existingHypernym.SendPropertyChanged("Hyponyms");
            }
        }

        public IEnumerable<Synset> Hyponyms => WordNetData.Context.HyponymsOf(ID);
        public void AddHyponym(Synset newHyponym) 
        {
            if (WordNetData.Context.AddHyponymyRelation(ID, newHyponym.ID).Equals(0))
            {
                SendPropertyChanged("Hyponyms");
                newHyponym.SendPropertyChanged("Hypernyms");
            }
        }
        public void DeleteHyponym(Synset existingHyponym) 
        {
            if (WordNetData.Context.DeleteHyponymyRelation(ID, existingHyponym.ID).Equals(0))
            {
                SendPropertyChanged("Hyponyms");
                existingHyponym.SendPropertyChanged("Hypernyms");
            }
        }

        public IEnumerable<Synset> SeeAlso => WordNetData.Context.SynsetsWithSeeAlsoRelationTo(ID);
        public void AddSeeAlso(Synset newSeeAlso)
        {
            if (WordNetData.Context.AddSeeAlsoSynsetRelation(ID, newSeeAlso.ID).Equals(0))
            {
                SendPropertyChanged("SeeAlso");
                newSeeAlso.SendPropertyChanged("SeeAlso");
            }
        }
        public void DeleteSeeAlso(Synset existingHyponym)
        {
            if (WordNetData.Context.DeleteSeeAlsoSynsetRelation(ID, existingHyponym.ID).Equals(0))
            {
                SendPropertyChanged("SeeAlso");
                existingHyponym.SendPropertyChanged("SeeAlso");
            }
        }

        #endregion Related synsets that apply to all parts of speech


        #region Related synsets that apply to Noun synsets only

        public IEnumerable<HolonymsOfResult> Holonyms => WordNetData.Context.HolonymsOf(ID);
        public void AddHolonym(Synset newHolonym, char holonymType) 
        {
            if (WordNetData.Context.AddMeronymyRelation(newHolonym.ID, ID, holonymType).Equals(0))
            {
                SendPropertyChanged("Holonyms");
                newHolonym.SendPropertyChanged("Meronyms");
            }
        }
        public void DeleteHolonym(Synset existingHolonym) 
        {
            if (WordNetData.Context.DeleteMeronymyRelation(existingHolonym.ID, ID).Equals(0))
            {
                SendPropertyChanged("Holonyms");
                existingHolonym.SendPropertyChanged("Meronyms");
            }
        }

        public IEnumerable<MeronymsOfResult> Meronyms => WordNetData.Context.MeronymsOf(ID);
        public void AddMeronym(Synset newMeronym, char meronymType) 
        {
            if (WordNetData.Context.AddMeronymyRelation(ID, newMeronym.ID, meronymType).Equals(0))
            {
                SendPropertyChanged("Meronyms");
                newMeronym.SendPropertyChanged("Holonyms");
            }
        }
        public void DeleteMeronym(Synset existingMeronym) 
        {
            if (WordNetData.Context.DeleteMeronymyRelation(ID, existingMeronym.ID).Equals(0))
            {
                SendPropertyChanged("Meronyms");
                existingMeronym.SendPropertyChanged("Holonyms");
            }
        }

        public IEnumerable<Synset> Types => WordNetData.Context.TypesOf(ID);
        public void AddType(Synset newType)
        {
            if (WordNetData.Context.AddInstanceRelation(newType.ID, ID).Equals(0))
            {
                SendPropertyChanged("Types");
                newType.SendPropertyChanged("Instances");
            }
        }
        public void DeleteType(Synset existingType)
        {
            if (WordNetData.Context.DeleteInstanceRelation(existingType.ID, ID).Equals(0))
            {
                SendPropertyChanged("Types");
                existingType.SendPropertyChanged("Instances");
            }
        }

        public IEnumerable<Synset> Instances => WordNetData.Context.InstancesOf(ID);
        public void AddInstance(Synset newInstance)
        {
            if (WordNetData.Context.AddInstanceRelation(ID, newInstance.ID).Equals(0))
            {
                SendPropertyChanged("Instances");
                newInstance.SendPropertyChanged("Types");
            }
        }
        public void DeleteInstance(Synset existingInstance)
        {
            if (WordNetData.Context.DeleteInstanceRelation(ID, existingInstance.ID).Equals(0))
            {
                SendPropertyChanged("Instances");
                existingInstance.SendPropertyChanged("Types");
            }
        }

        public IEnumerable<Synset> ValuesOfAttribute => WordNetData.Context.ValuesOfAttribute(ID);
        public void AddValueOfAttribute(Synset newValue)
        {
            if (WordNetData.Context.AddAttributeRelation(ID, newValue.ID).Equals(0))
            {
                SendPropertyChanged("ValuesOfAttribute");
                newValue.SendPropertyChanged("AttributesWithValue");
            }
        }
        public void DeleteValueOfAttribute(Synset existingValue)
        {
            if (WordNetData.Context.DeleteAttributeRelation(ID, existingValue.ID).Equals(0))
            {
                SendPropertyChanged("ValuesOfAttribute");
                existingValue.SendPropertyChanged("AttributesWithValue");
            }
        }

        #endregion Related synsets that apply to Noun synsets only


        #region Related synsets that apply to Verb synsets only

        public IEnumerable<Synset> IsCausedBy => WordNetData.Context.CausesOf(ID);
        public void AddCauser(Synset newCauser)
        {
            if (WordNetData.Context.AddCausationRelation(newCauser.ID, ID).Equals(0))
            {
                SendPropertyChanged("IsCausedBy");
                newCauser.SendPropertyChanged("IsCauseOf");
            }
        }
        public void DeleteCauser(Synset existingCauser)
        {
            if (WordNetData.Context.DeleteCausationRelation(existingCauser.ID, ID).Equals(0))
            {
                SendPropertyChanged("IsCausedBy");
                existingCauser.SendPropertyChanged("IsCauseOf");
            }
        }

        public IEnumerable<Synset> IsCauseOf => WordNetData.Context.CausedBy(ID);
        public void AddCaused(Synset newCaused)
        {
            if (WordNetData.Context.AddCausationRelation(ID, newCaused.ID).Equals(0))
            {
                SendPropertyChanged("IsCauseOf");
                newCaused.SendPropertyChanged("IsCausedBy");
            }
        }
        public void DeleteCaused(Synset existingCaused)
        {
            if (WordNetData.Context.DeleteCausationRelation(ID, existingCaused.ID).Equals(0))
            {
                SendPropertyChanged("IsCauseOf");
                existingCaused.SendPropertyChanged("IsCausedBy");
            }
        }

        public IEnumerable<Synset> IsEntailedBy => WordNetData.Context.EntailersOf(ID);
        public void AddEntailer(Synset newEntailer)
        {
            if (WordNetData.Context.AddEntailmentRelation(newEntailer.ID, ID).Equals(0))
            {
                SendPropertyChanged("IsEntailedBy");
                newEntailer.SendPropertyChanged("Entails");
            }
        }
        public void DeleteEntailer(Synset existingEntailer)
        {
            if (WordNetData.Context.DeleteEntailmentRelation(existingEntailer.ID, ID).Equals(0))
            {
                SendPropertyChanged("IsEntailedBy");
                existingEntailer.SendPropertyChanged("Entails");
            }
        }

        public IEnumerable<Synset> Entails => WordNetData.Context.EntailedBy(ID);
        public void AddEntailed(Synset newEntailed)
        {
            if (WordNetData.Context.AddEntailmentRelation(ID, newEntailed.ID).Equals(0))
            {
                SendPropertyChanged("Entails");
                newEntailed.SendPropertyChanged("IsEntailedBy");
            }
        }
        public void DeleteEntailed(Synset existingEntailed)
        {
            if (WordNetData.Context.DeleteEntailmentRelation(ID, existingEntailed.ID).Equals(0))
            {
                SendPropertyChanged("Entails");
                existingEntailed.SendPropertyChanged("IsEntailedBy");
            }
        }

        public IEnumerable<VerbFramesForSynsetResult> VerbFrames => WordNetData.Context.VerbFramesForSynset(ID);
        public IEnumerable<Synset> VerbGroupMembers => WordNetData.Context.SynsetsInVerbGroupWith(ID);

        #endregion Related synsets that apply to Verb synsets only


        #region Related synsets that apply to Adjective synsets only

        public IEnumerable<Synset> Satellites => WordNetData.Context.SatellitesOf(ID);

        public IEnumerable<Synset> AttributesWithValue => WordNetData.Context.AttributesWithValue(ID);
        public void AddAttributeWithValue(Synset newAttribute)
        {
            if (WordNetData.Context.AddAttributeRelation(newAttribute.ID, ID).Equals(0))
            {
                SendPropertyChanged("AttributesWithValue");
                newAttribute.SendPropertyChanged("ValuesOfAttribute");
            }
        }
        public void DeleteAttributeWithValue(Synset existingAttribute)
        {
            if (WordNetData.Context.DeleteAttributeRelation(existingAttribute.ID, ID).Equals(0))
            {
                SendPropertyChanged("AttributesWithValue");
                existingAttribute.SendPropertyChanged("ValuesOfAttribute");
            }
        }

        #endregion Related synsets that apply to Adjective synsets only


        #region Related synsets that apply to Adjective Head synsets only

        #endregion Related synsets that apply to Adjective Head synsets only


        #region Related synsets that apply to Adjective Satellite synsets only

        public Synset ClusterHead => WordNetData.Context.ClusterHeadOf(ID).FirstOrDefault();

        public IEnumerable<Synset> ClusterMembers => WordNetData.Context.MembersOfSameAdjectiveClusterAs(ID);

        #endregion Related synsets that apply to Adjective Satellite synsets only
    }
}
