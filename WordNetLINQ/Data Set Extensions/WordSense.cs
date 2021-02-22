using System.Collections.Generic;
using System.Linq;
using FlexibleRealization;

namespace WordNet.Linq
{
    public partial class WordSense
    {
        public string PartOfSpeechAndWordText => $"({WordNetData.UserFriendlyPartOfSpeechFrom(POS)}.) {WordText}";

        public bool IsNoun => WordNetData.IsNoun(POS);
        public bool IsVerb => WordNetData.IsVerb(POS);
        public bool IsAdjective => WordNetData.IsAdjective(POS);
        public bool IsAdverb => WordNetData.IsAdverb(POS);

        public IEnumerable<WordSense> Derivations => WordNetData.Context.DerivationsOf(SynsetID, WordNumber);
        public IEnumerable<WordSense> Antonyms => WordNetData.Context.AntonymsOf(SynsetID, WordNumber);
        public IEnumerable<WordSense> PertainsTo => WordNetData.Context.PertainsTo(SynsetID, WordNumber);
        public IEnumerable<WordSense> Pertainers => WordNetData.Context.PertainersOf(SynsetID, WordNumber);
        public IEnumerable<WordSense> SeeAlso => WordNetData.Context.SeeAlso(SynsetID, WordNumber);
        public IEnumerable<TeleologyForResult2> Teleologies => WordNetData.Context.TeleologyFor(SynsetID, WordNumber);
        public IEnumerable<MorphosemanticRelationsForResult> MorphosemanticRelations => WordNetData.Context.MorphosemanticRelationsFor(SynsetID, WordNumber);


        #region Verbs only

        public IEnumerable<WordSense> ParticipleForms => WordNetData.Context.ParticipleFormsOf(SynsetID, WordNumber);
        public IEnumerable<VerbFramesForWordSenseResult> VerbFrames => WordNetData.Context.VerbFramesForWordSense(SynsetID, WordNumber);

        #endregion Verbs only


        #region Adjectives only

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
        public IEnumerable<WordSense> BaseVerbFormsOfParticiple => WordNetData.Context.BaseVerbFormsOfParticiple(SynsetID, WordNumber);

        #endregion Adjectives only


        #region Adverbs only

        public IEnumerable<WordSense> AdjectiveBasesOfDerivedAdverb => WordNetData.Context.AdjectivesBasesOfDerivedAdverb(SynsetID, WordNumber);

        #endregion Adverbs only
    }
}
