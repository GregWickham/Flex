using System.Linq;
using System.Windows;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    public class WordSenseNavigatorEditHelper
    {
        internal WordSenseNavigatorEditHelper(WordSenseNavigatorViewModel currentWordSenseSource)
        {
            CurrentWordSenseSource = currentWordSenseSource;

            Antonyms = new RelatedWordSensesGroupEditHelper();
            Derivations = new RelatedWordSensesGroupEditHelper();
            AdjectiveSyntax = new RelatedWordSensesGroupEditHelper();
            SeeAlso = new RelatedWordSensesGroupEditHelper();
            AdjectiveBasesOfDerivedAdverb = new RelatedWordSensesGroupEditHelper();
            DerivedAdverbs = new RelatedWordSensesGroupEditHelper();
            BaseVerbFormsOfParticiple = new RelatedWordSensesGroupEditHelper();
            ParticipleForms = new RelatedWordSensesGroupEditHelper();
            Pertainers = new RelatedWordSensesGroupEditHelper();
            PertainsTo = new RelatedWordSensesGroupEditHelper();
            Morphosemantic = new RelatedWordSensesGroupEditHelper();
            Teleology = new RelatedWordSensesGroupEditHelper();
            VerbFrames = new RelatedWordSensesGroupEditHelper();
        }

        private WordSenseNavigatorViewModel CurrentWordSenseSource;

        #region Bindable properties of various related Word Sense lists

        public RelatedWordSensesGroupEditHelper Antonyms { get; }
        public RelatedWordSensesGroupEditHelper Derivations { get; }
        public RelatedWordSensesGroupEditHelper AdjectiveSyntax { get; }
        public RelatedWordSensesGroupEditHelper SeeAlso { get; }
        public RelatedWordSensesGroupEditHelper AdjectiveBasesOfDerivedAdverb { get; }
        public RelatedWordSensesGroupEditHelper DerivedAdverbs { get; }
        public RelatedWordSensesGroupEditHelper BaseVerbFormsOfParticiple { get; } 
        public RelatedWordSensesGroupEditHelper ParticipleForms { get; }
        public RelatedWordSensesGroupEditHelper Pertainers { get; }
        public RelatedWordSensesGroupEditHelper PertainsTo { get; }  
        public RelatedWordSensesGroupEditHelper Morphosemantic { get; }
        public RelatedWordSensesGroupEditHelper Teleology { get; }
        public RelatedWordSensesGroupEditHelper VerbFrames { get; }


        #endregion Bindable properties of various related Word Sense lists

        internal void SetDropTargets_ForWordSense(WordSense draggedWordSense)
        {
            if (CurrentWordSenseSource.CurrentWordSense != null)
            {
                GetNewWordSenseRelationConstraintsResult addRelationConstraints = WordNetData.Context.GetNewWordSenseRelationConstraints(CurrentWordSenseSource.CurrentWordSense.SynsetID, CurrentWordSenseSource.CurrentWordSense.WordNumber, draggedWordSense.SynsetID, draggedWordSense.WordNumber).Single();
                Antonyms.DropIsEnabled = (bool)addRelationConstraints.CanAddAntonym;
                Derivations.DropIsEnabled = (bool)addRelationConstraints.CanAddDerivation;
                SeeAlso.DropIsEnabled = (bool)addRelationConstraints.CanAddSeeAlso;
                Pertainers.DropIsEnabled = (bool)addRelationConstraints.CanAddPertainer;
                ParticipleForms.DropIsEnabled = (bool)addRelationConstraints.CanAddParticipleForm;
                PertainsTo.DropIsEnabled = (bool)addRelationConstraints.CanAddPertainedTo;
                BaseVerbFormsOfParticiple.DropIsEnabled = (bool)addRelationConstraints.CanAddBaseFormOfParticiple;
                DerivedAdverbs.DropIsEnabled = (bool)addRelationConstraints.CanAddDerivedAdverb;
                AdjectiveBasesOfDerivedAdverb.DropIsEnabled = (bool)addRelationConstraints.CanAddAdjectiveBaseOfDerivedAdverb;
            }
        }

        internal void ClearDropTargets()
        {
            Antonyms.DropIsEnabled = false;
            Derivations.DropIsEnabled = false;
            SeeAlso.DropIsEnabled = false;
            Pertainers.DropIsEnabled = false;
            ParticipleForms.DropIsEnabled = false;
            PertainsTo.DropIsEnabled = false;
            BaseVerbFormsOfParticiple.DropIsEnabled = false;
            DerivedAdverbs.DropIsEnabled = false;
            AdjectiveBasesOfDerivedAdverb.DropIsEnabled = false;
        }

        private WordSense DroppedWordSenseFrom(DragEventArgs e) => e.Data.GetDataPresent(typeof(WordSense)) ? (WordSense)e.Data.GetData(typeof(WordSense)) : null;

        internal void OnAntonymsDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddAntonym(DroppedWordSenseFrom(e));
        internal void DeleteAntonym(WordSense existingAntonym) { CurrentWordSenseSource.CurrentWordSense.DeleteAntonym(existingAntonym); }

        internal void OnDerivationsDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddDerivation(DroppedWordSenseFrom(e));
        internal void DeleteDerivation(WordSense existingDerivation) { CurrentWordSenseSource.CurrentWordSense.DeleteDerivation(existingDerivation); }

        internal void OnSeeAlsoDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddSeeAlso(DroppedWordSenseFrom(e));
        internal void DeleteSeeAlso(WordSense existingSeeAlso) { CurrentWordSenseSource.CurrentWordSense.DeleteSeeAlso(existingSeeAlso); }

        internal void OnAdjectiveBasesOfDerivedAdverbDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddAdjectiveBaseOfDerivedAdverb(DroppedWordSenseFrom(e));
        internal void DeleteAdjectiveBaseOfDerivedAdverb(WordSense existingAdjectiveBase) { CurrentWordSenseSource.CurrentWordSense.DeleteAdjectiveBaseOfDerivedAdverb(existingAdjectiveBase); }

        internal void OnDerivedAdverbsDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddDerivedAdverb(DroppedWordSenseFrom(e));
        internal void DeleteDerivedAdverb(WordSense existingDerivedAdverb) { CurrentWordSenseSource.CurrentWordSense.DeleteDerivedAdverb(existingDerivedAdverb); }

        internal void OnBaseVerbFormsOfParticipleDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddBaseVerbFormOfParticiple(DroppedWordSenseFrom(e));
        internal void DeleteBaseVerbFormOfParticiple(WordSense existingBaseVerbFormOfParticiple) { CurrentWordSenseSource.CurrentWordSense.DeleteBaseVerbFormOfParticiple(existingBaseVerbFormOfParticiple); }

        internal void OnParticipleFormsDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddParticipleForm(DroppedWordSenseFrom(e));
        internal void DeleteParticipleForm(WordSense existingParticipleForm) { CurrentWordSenseSource.CurrentWordSense.DeleteParticipleForm(existingParticipleForm); }

        internal void OnPertainersDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddPertainer(DroppedWordSenseFrom(e));
        internal void DeletePertainer(WordSense existingPertainer) { CurrentWordSenseSource.CurrentWordSense.DeletePertainer(existingPertainer); }

        internal void OnPertainsToDrop(DragEventArgs e) => CurrentWordSenseSource.CurrentWordSense.AddPertainsTo(DroppedWordSenseFrom(e));
        internal void DeletePertainsTo(WordSense existingPertainsTo) { CurrentWordSenseSource.CurrentWordSense.DeleteParticipleForm(existingPertainsTo); }

    }
}
