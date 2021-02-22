using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordNet.Linq;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for SynsetNavigator.xaml</summary>
    public partial class SynsetNavigatorControl : UserControl
    {
        public SynsetNavigatorControl()
        {
            InitializeComponent();
        }

        internal Synset CurrentSynset
        {
            get => (Synset)DataContext;
            set
            {
                // This assignment could be coming from inside or outside this control, so don't set off a recursion
                if (value != null && value != DataContext)
                {
                    DataContext = value;
                    // Changing this column width to star when it's visible prevents the Auto-width column from getting too wide (i.e., force its contents to use their horizontal scrollbar).
                    // Making the column Auto-width when it's NOT visible causes the column to collapse when its contents collapse.
                    PartOfSpeechSpecificRelationsColumn.Width = value.IsNoun || value.IsVerb || value.IsAdjective
                        ? StarColumnWidth
                        : GridLength.Auto;
                    NounSpecificRelations.Visibility = value.IsNoun ? Visibility.Visible : Visibility.Collapsed;
                    VerbSpecificRelations.Visibility = value.IsVerb ? Visibility.Visible : Visibility.Collapsed;
                    AdjectiveSpecificRelations.Visibility = value.IsAdjective ? Visibility.Visible : Visibility.Collapsed;
                    OnSynsetSelected(value);
                }
            }
        }

        private static readonly GridLength StarColumnWidth = new GridLength(1, GridUnitType.Star);

        #region Events

        internal event SynsetDragStarted_EventHandler SynsetDragStarted;
        private void OnSynsetDragStarted(Synset synset) => SynsetDragStarted?.Invoke(synset);

        internal event SynsetDragCancelled_EventHandler SynsetDragCancelled;
        private void OnSynsetDragCancelled(Synset synset) => SynsetDragCancelled?.Invoke(synset);

        internal event SynsetDropCompleted_EventHandler SynsetDropCompleted;
        private void OnSynsetDropCompleted(Synset synset) => SynsetDropCompleted?.Invoke(synset);

        internal event SynsetSelected_EventHandler SynsetSelected;
        private void OnSynsetSelected(Synset synset) => SynsetSelected?.Invoke(synset);

        #endregion Events


        #region Synset Navigation

        #region Common to all synset parts of speech

        private Synset SelectedHypernym => (Synset)HypernymsList.SelectedItem;
        private void HypernymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedHypernym;

        private Synset SelectedHyponym => (Synset)HyponymsList.SelectedItem;
        private void HyponymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedHyponym;

        private Synset SelectedHolonym => (Synset)HolonymsList.SelectedItem;
        private void HolonymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedHolonym;

        private Synset SelectedMeronym => (Synset)MeronymsList.SelectedItem;
        private void MeronymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedMeronym;

        #endregion Common to all synset parts of speech


        #region Noun-specific relations

        private Synset SelectedValueOfAttribute => (Synset)ValuesOfAttributeList.SelectedItem;
        private void ValuesOfAttributeList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedValueOfAttribute;

        #endregion Noun-specific relations


        #region Verb-specific relations

        private Synset SelectedIsCausedBy => (Synset)IsCausedByList.SelectedItem;
        private void IsCausedByList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedIsCausedBy;

        private Synset SelectedIsCauseOf => (Synset)IsCauseOfList.SelectedItem;
        private void IsCauseOfList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedIsCauseOf;

        private Synset SelectedIsEntailedBy => (Synset)IsEntailedByList.SelectedItem;
        private void IsEntailedByList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedIsEntailedBy;

        private Synset SelectedEntails => (Synset)EntailsList.SelectedItem;
        private void EntailsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedEntails;

        private Synset SelectedVerbGroupMember => (Synset)VerbGroupMembersList.SelectedItem;
        private void VerbGroupMembersList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedVerbGroupMember;

        #endregion Verb-specific relations


        #region Adjective-specific relations

        private Synset SelectedSatellite => (Synset)SatellitesList.SelectedItem;
        private void SatellitesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedSatellite;

        private Synset SelectedAttributeWithValue => (Synset)AttributesWithValueList.SelectedItem;
        private void AttributesWithValueList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentSynset = SelectedAttributeWithValue;

        #endregion Adjective-specific relations


        #endregion Synset Navigation


        #region Drag / Drop of Synsets from this control

        private Point mouseDownPosition;
        private void CurrentSynsetHeader_MouseDown(object sender, MouseButtonEventArgs e) => mouseDownPosition = e.GetPosition(this);

        private void CurrentSynsetHeader_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);
            Vector mouseDownDistanceMoved = mouseDownPosition - currentMousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (CurrentSynset != null)
                {
                    OnSynsetDragStarted(CurrentSynset);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(Synset), CurrentSynset);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Link | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnSynsetDragCancelled(CurrentSynset);
                            break;
                        case DragDropEffects.Link:
                            OnSynsetDropCompleted(CurrentSynset);
                            break;
                        default: break;
                    }
                }
            }
        }

        #endregion Drag / Drop of Synsets from this control
    }
}
