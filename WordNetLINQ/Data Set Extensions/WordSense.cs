using System.Collections.Generic;
using System.Linq;

namespace WordNet.Linq
{
    public partial class WordSense
    {
        public Synset LinkedSynset => Synset.WithID(SynsetID);

        public string PartOfSpeechAndWordText => $"({WordNetData.UserFriendlyPartOfSpeechFrom(POS)}.) {WordText}";

        public bool IsNoun => WordNetData.IsNoun(POS);
        public bool IsVerb => WordNetData.IsVerb(POS);
        public bool IsAdjective => WordNetData.IsAdjective(POS);
        public bool IsAdverb => WordNetData.IsAdverb(POS);

        public IEnumerable<WordSense> Antonyms => WordNetData.Context.AntonymsOf(SynsetID, WordNumber);
        public void AddAntonym(WordSense newAntonym)
        {
            if (WordNetData.Context.AddAntonymyRelation(newAntonym.SynsetID, newAntonym.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("Antonyms");
                newAntonym.SendPropertyChanged("Antonyms");
            }
        }
        public void DeleteAntonym(WordSense existingAntonym)
        {
            if (WordNetData.Context.DeleteAntonymyRelation(existingAntonym.SynsetID, existingAntonym.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("Antonyms");
                existingAntonym.SendPropertyChanged("Antonyms");
            }
        }

        public IEnumerable<WordSense> Derivations => WordNetData.Context.DerivationsOf(SynsetID, WordNumber);
        public void AddDerivation(WordSense newDerivation)
        {
            if (WordNetData.Context.AddDerivationRelation(newDerivation.SynsetID, newDerivation.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("Derivations");
                newDerivation.SendPropertyChanged("Derivations");
            }
        }
        public void DeleteDerivation(WordSense existingDerivation)
        {
            if (WordNetData.Context.DeleteDerivationRelation(existingDerivation.SynsetID, existingDerivation.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("Derivations");
                existingDerivation.SendPropertyChanged("Derivations");
            }
        }

        public IEnumerable<WordSense> SeeAlso => WordNetData.Context.WordSensesWithSeeAlsoRelationTo(SynsetID, WordNumber);
        public void AddSeeAlso(WordSense newSeeAlso)
        {
            if (WordNetData.Context.AddSeeAlsoWordSenseRelation(newSeeAlso.SynsetID, newSeeAlso.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("SeeAlso");
                newSeeAlso.SendPropertyChanged("SeeAlso");
            }
        }
        public void DeleteSeeAlso(WordSense existingSeeAlso)
        {
            if (WordNetData.Context.DeleteSeeAlsoWordSenseRelation(existingSeeAlso.SynsetID, existingSeeAlso.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("SeeAlso");
                existingSeeAlso.SendPropertyChanged("SeeAlso");
            }
        }

        public IEnumerable<MorphosemanticRelationsForResult> MorphosemanticRelations => WordNetData.Context.MorphosemanticRelationsFor(SynsetID, WordNumber);
        public IEnumerable<TeleologyForResult> Teleologies => WordNetData.Context.TeleologyFor(SynsetID, WordNumber);

        #region Nouns or Adjectives only

        public IEnumerable<WordSense> Pertainers => WordNetData.Context.PertainersTo(SynsetID, WordNumber);
        public void AddPertainer(WordSense newPertainer)
        {
            if (WordNetData.Context.AddPertainymyRelation(newPertainer.SynsetID, newPertainer.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("Pertainers");
                newPertainer.SendPropertyChanged("PertainsTo");
            }
        }
        public void DeletePertainer(WordSense existingPertainer)
        {
            if (WordNetData.Context.DeletePertainymyRelation(existingPertainer.SynsetID, existingPertainer.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("Pertainers");
                existingPertainer.SendPropertyChanged("PertainsTo");
            }
        }

        #endregion Nouns or Adjectives only

        #region Verbs only

        public IEnumerable<WordSense> ParticipleForms => WordNetData.Context.ParticipleFormsOf(SynsetID, WordNumber);
        public void AddParticipleForm(WordSense newParticipleForm)
        {
            if (WordNetData.Context.AddParticipleRelation(SynsetID, WordNumber, newParticipleForm.SynsetID, newParticipleForm.WordNumber).Equals(0))
            {
                SendPropertyChanged("ParticipleForms");
                newParticipleForm.SendPropertyChanged("BaseVerbFormsOfParticiple");
            }
        }
        public void DeleteParticipleForm(WordSense existingParticipleForm)
        {
            if (WordNetData.Context.DeleteParticipleRelation(SynsetID, WordNumber, existingParticipleForm.SynsetID, existingParticipleForm.WordNumber).Equals(0))
            {
                SendPropertyChanged("ParticipleForms");
                existingParticipleForm.SendPropertyChanged("BaseVerbFormsOfParticiple");
            }
        }

        public IEnumerable<VerbFramesForWordSenseResult> VerbFrames => WordNetData.Context.VerbFramesForWordSense(SynsetID, WordNumber);

        #endregion Verbs only


        #region Adjectives only

        public IEnumerable<WordSense> PertainsTo => WordNetData.Context.PertainedToBy(SynsetID, WordNumber);
        public void AddPertainsTo(WordSense newPertainedTo)
        {
            if (WordNetData.Context.AddPertainymyRelation(SynsetID, WordNumber, newPertainedTo.SynsetID, newPertainedTo.WordNumber).Equals(0))
            {
                SendPropertyChanged("PertainsTo");
                newPertainedTo.SendPropertyChanged("Pertainers");
            }
        }
        public void DeletePertainsTo(WordSense existingPertainedTo)
        {
            if (WordNetData.Context.DeletePertainymyRelation(SynsetID, WordNumber, existingPertainedTo.SynsetID, existingPertainedTo.WordNumber).Equals(0))
            {
                SendPropertyChanged("PertainsTo");
                existingPertainedTo.SendPropertyChanged("Pertainers");
            }
        }

        /// <summary>Return a description of the adjective syntax for this word sense.</summary>
        /// <remarks>The function call will never return a result for any non-adjective part of speech, and only for some adjectives.
        /// I tried using a scalar-valued server-side function, but those are flaky in SQL Server so this seemed like a more reliable way.</remarks>
        public string AdjectiveSyntax => WordNetData.Context.SyntaxOfAdjective(SynsetID, WordNumber).FirstOrDefault()?.SyntaxCode switch
        {
            "p" => "Predicate",
            "a" => "Attributive",
            "ip" => "Postnominal",
            _ => "(Unspecified)"
        };

        public IEnumerable<WordSense> DerivedAdverbs => WordNetData.Context.AdverbsDerivedFrom(SynsetID, WordNumber);
        public void AddDerivedAdverb(WordSense newDerivedAdverb)
        {
            if (WordNetData.Context.AddAdverbDerivationRelation(SynsetID, WordNumber, newDerivedAdverb.SynsetID, newDerivedAdverb.WordNumber).Equals(0))
            {
                SendPropertyChanged("DerivedAdverbs");
                newDerivedAdverb.SendPropertyChanged("AdjectiveBasesOfDerivedAdverb");
            }
        }
        public void DeleteDerivedAdverb(WordSense existingDerivedAdverb)
        {
            if (WordNetData.Context.DeleteAdverbDerivationRelation(SynsetID, WordNumber, existingDerivedAdverb.SynsetID, existingDerivedAdverb.WordNumber).Equals(0))
            {
                SendPropertyChanged("DerivedAdverbs");
                existingDerivedAdverb.SendPropertyChanged("AdjectiveBasesOfDerivedAdverb");
            }
        }

        public IEnumerable<WordSense> BaseVerbFormsOfParticiple => WordNetData.Context.BaseVerbFormsOfParticiple(SynsetID, WordNumber);
        public void AddBaseVerbFormOfParticiple(WordSense newBaseVerbFormOfParticiple)
        {
            if (WordNetData.Context.AddParticipleRelation(newBaseVerbFormOfParticiple.SynsetID, newBaseVerbFormOfParticiple.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("BaseVerbFormsOfParticiple");
                newBaseVerbFormOfParticiple.SendPropertyChanged("ParticipleForms");
            }
        }
        public void DeleteBaseVerbFormOfParticiple(WordSense existingBaseVerbFormOfParticiple)
        {
            if (WordNetData.Context.DeleteParticipleRelation(existingBaseVerbFormOfParticiple.SynsetID, existingBaseVerbFormOfParticiple.WordNumber, SynsetID, WordNumber).Equals(0))
            {
                SendPropertyChanged("BaseVerbFormsOfParticiple");
                existingBaseVerbFormOfParticiple.SendPropertyChanged("ParticipleForms");
            }
        }

        #endregion Adjectives only


        #region Adverbs only

        public IEnumerable<WordSense> AdjectiveBasesOfDerivedAdverb => WordNetData.Context.AdjectiveBasesOfDerivedAdverb(SynsetID, WordNumber);
        public void AddAdjectiveBaseOfDerivedAdverb(WordSense newAdjectiveBase)
        {
            if (WordNetData.Context.AddAdverbDerivationRelation(SynsetID, WordNumber, newAdjectiveBase.SynsetID, newAdjectiveBase.WordNumber).Equals(0))
            {
                SendPropertyChanged("AdjectiveBasesOfDerivedAdverb");
                newAdjectiveBase.SendPropertyChanged("DerivedAdverbs");
            }
        }
        public void DeleteAdjectiveBaseOfDerivedAdverb(WordSense existingAdjectiveBase)
        {
            if (WordNetData.Context.DeleteAdverbDerivationRelation(SynsetID, WordNumber, existingAdjectiveBase.SynsetID, existingAdjectiveBase.WordNumber).Equals(0))
            {
                SendPropertyChanged("AdjectiveBasesOfDerivedAdverb");
                existingAdjectiveBase.SendPropertyChanged("DerivedAdverbs");
            }
        }

        #endregion Adverbs only
    }
}
