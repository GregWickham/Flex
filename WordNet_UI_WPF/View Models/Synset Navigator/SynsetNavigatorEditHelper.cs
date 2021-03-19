using System.Linq;
using System.Windows;
using System.Windows.Input;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    public class SynsetNavigatorEditHelper 
    {
        internal SynsetNavigatorEditHelper(SynsetNavigatorViewModel currentSynsetSource)
        {
            CurrentSynsetSource = currentSynsetSource;

            Hypernyms = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Hyponyms = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Holonyms = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Meronyms = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Types = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Instances = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            ValuesOfAttribute = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            IsCausedBy = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            IsCauseOf = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            IsEntailedBy = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Entails = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            SeeAlso = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            VerbFrames = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            VerbGroupMembers = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            ClusterHead = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            ClusterMembers = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            Satellites = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
            AttributesWithValue = new RelatedSynsetsGroupEditHelper(currentSynsetSource);
        }

        private SynsetNavigatorViewModel CurrentSynsetSource;

        #region Edit helpers for various related Synset lists

        public RelatedSynsetsGroupEditHelper Hypernyms { get; } 
        public RelatedSynsetsGroupEditHelper Hyponyms { get; }
        public RelatedSynsetsGroupEditHelper Holonyms { get; }
        public RelatedSynsetsGroupEditHelper Meronyms { get; }
        public RelatedSynsetsGroupEditHelper Types { get; }
        public RelatedSynsetsGroupEditHelper Instances { get; }
        public RelatedSynsetsGroupEditHelper ValuesOfAttribute { get; }
        public RelatedSynsetsGroupEditHelper IsCausedBy { get; }
        public RelatedSynsetsGroupEditHelper IsCauseOf { get; }
        public RelatedSynsetsGroupEditHelper IsEntailedBy { get; }
        public RelatedSynsetsGroupEditHelper Entails { get; }
        public RelatedSynsetsGroupEditHelper SeeAlso { get; }
        public RelatedSynsetsGroupEditHelper VerbFrames { get; }
        public RelatedSynsetsGroupEditHelper VerbGroupMembers { get; }
        public RelatedSynsetsGroupEditHelper ClusterHead { get; }
        public RelatedSynsetsGroupEditHelper ClusterMembers { get; }
        public RelatedSynsetsGroupEditHelper Satellites { get; }
        public RelatedSynsetsGroupEditHelper AttributesWithValue { get; }

        #endregion Edit helpers for various related Synset lists

        internal void SetDropTargets_ForSynset(Synset draggedSynset)
        {
            if (CurrentSynsetSource.CurrentSynset != null)
            {
                GetNewSynsetRelationConstraintsResult addRelationConstraints = WordNetData.Context.GetNewSynsetRelationConstraints(CurrentSynsetSource.CurrentSynset.ID, draggedSynset.ID).Single();
                Hypernyms.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddHypernym;
                Hyponyms.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddHyponym;
                Holonyms.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddHolonym;
                Meronyms.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddMeronym;
                Types.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddType;
                Instances.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddInstance;
                ValuesOfAttribute.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddValueOfAttribute;
                IsCausedBy.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddCausedBy;
                IsCauseOf.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddCauseOf;
                IsEntailedBy.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddEntailedBy;
                SeeAlso.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddSeeAlso;
                Entails.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddEntails;
                VerbGroupMembers.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddVerbGroupMember;
                Satellites.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddSatellite;
                AttributesWithValue.SynsetDropIsEnabled = (bool)addRelationConstraints.CanAddAttributeWithValue;
            }
        }

        internal void ClearDropTargets()
        {
            Hypernyms.SynsetDropIsEnabled = false;
            Hyponyms.SynsetDropIsEnabled = false;
            Holonyms.SynsetDropIsEnabled = false;
            Meronyms.SynsetDropIsEnabled = false;
            Types.SynsetDropIsEnabled = false;
            Instances.SynsetDropIsEnabled = false;
            ValuesOfAttribute.SynsetDropIsEnabled = false;
            IsCausedBy.SynsetDropIsEnabled = false;
            IsCauseOf.SynsetDropIsEnabled = false;
            IsEntailedBy.SynsetDropIsEnabled = false;
            Entails.SynsetDropIsEnabled = false;
            SeeAlso.SynsetDropIsEnabled = false;
            VerbGroupMembers.SynsetDropIsEnabled = false;
            Satellites.SynsetDropIsEnabled = false;
            AttributesWithValue.SynsetDropIsEnabled = false;
        }

        private Synset DroppedSynset;

        private Synset DroppedSynsetFrom(DragEventArgs e) => e.Data.GetDataPresent(typeof(Synset)) ? (Synset)e.Data.GetData(typeof(Synset)) : null;

        internal void OnHypernymsDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddHypernym(DroppedSynsetFrom(e));
        internal void DeleteHypernym(Synset existingHypernym) { CurrentSynsetSource.CurrentSynset.DeleteHypernym(existingHypernym); }

        internal void OnHyponymsDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddHyponym(DroppedSynsetFrom(e));
        internal void DeleteHyponym(Synset existingHyponym) { CurrentSynsetSource.CurrentSynset.DeleteHyponym(existingHyponym); }

        internal void OnHolonymsDrop(DragEventArgs e) { DroppedSynset = DroppedSynsetFrom(e); }
        internal void OnHolonymTypeSelected(char holonymType) => CurrentSynsetSource.CurrentSynset.AddHolonym(DroppedSynset, holonymType);
        internal void DeleteHolonym(HolonymsOfResult existingHolonym) { CurrentSynsetSource.CurrentSynset.DeleteHolonym(Synset.WithID(existingHolonym.ID)); }

        internal void OnMeronymsDrop(DragEventArgs e) { DroppedSynset = DroppedSynsetFrom(e); }
        internal void OnMeronymTypeSelected(char meronymType) => CurrentSynsetSource.CurrentSynset.AddMeronym(DroppedSynset, meronymType);
        internal void DeleteMeronym(MeronymsOfResult existingMeronym) { CurrentSynsetSource.CurrentSynset.DeleteMeronym(Synset.WithID(existingMeronym.ID)); }

        internal void OnTypesDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddType(DroppedSynsetFrom(e));
        internal void DeleteType(Synset existingType) { CurrentSynsetSource.CurrentSynset.DeleteType(existingType); }

        internal void OnInstancesDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddInstance(DroppedSynsetFrom(e));
        internal void DeleteInstance(Synset existingInstance) { CurrentSynsetSource.CurrentSynset.DeleteInstance(existingInstance); }

        internal void OnIsCausedByDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddCauser(DroppedSynsetFrom(e));
        internal void DeleteIsCausedBy(Synset existingCauser) { CurrentSynsetSource.CurrentSynset.DeleteCauser(existingCauser); }

        internal void OnIsCauseOfDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddCaused(DroppedSynsetFrom(e));
        internal void DeleteIsCauseOf(Synset existingCaused) { CurrentSynsetSource.CurrentSynset.DeleteCaused(existingCaused); }

        internal void OnIsEntailedByDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddEntailer(DroppedSynsetFrom(e));
        internal void DeleteEntailedBy(Synset existingEntailer) { CurrentSynsetSource.CurrentSynset.DeleteEntailer(existingEntailer); }

        internal void OnEntailsDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddEntailed(DroppedSynsetFrom(e));
        internal void DeleteEntails(Synset existingEntailed) { CurrentSynsetSource.CurrentSynset.DeleteEntailed(existingEntailed); }

        internal void OnSeeAlsoDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddSeeAlso(DroppedSynsetFrom(e));
        internal void DeleteSeeAlso(Synset existingSeeAlso) { CurrentSynsetSource.CurrentSynset.DeleteSeeAlso(existingSeeAlso); }

        internal void OnAttributesWithValueDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddAttributeWithValue(DroppedSynsetFrom(e));
        internal void DeleteAttributeWithValue(Synset existingAttributeWithValue) { CurrentSynsetSource.CurrentSynset.DeleteAttributeWithValue(existingAttributeWithValue); }

        internal void OnValuesOfAttributeDrop(DragEventArgs e) => CurrentSynsetSource.CurrentSynset.AddValueOfAttribute(DroppedSynsetFrom(e));
        internal void DeleteValueOfAttribute(Synset existingValueOfAttribute) { CurrentSynsetSource.CurrentSynset.DeleteValueOfAttribute(existingValueOfAttribute); }
    }
}
