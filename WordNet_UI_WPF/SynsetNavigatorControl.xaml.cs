using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordNet.Linq;
using WordNet.UserInterface.ViewModels;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for SynsetNavigator.xaml</summary>
    public partial class SynsetNavigatorControl : UserControl
    {
        public SynsetNavigatorControl()
        {
            InitializeComponent();
            ViewModel.SynsetSelected += ViewModel_SynsetSelected;
            DataContext = ViewModel;
        }

        private void ViewModel_SynsetSelected(Synset synset) => SynsetSelected?.Invoke(synset);

        internal SynsetNavigatorViewModel ViewModel { get; } = new SynsetNavigatorViewModel();

        internal bool EditingIsEnabled { get; set; } = false;

        #region Events

        internal event SynsetSelected_EventHandler SynsetSelected;

        internal event SynsetDragStarted_EventHandler SynsetDragStarted;
        private void OnSynsetDragStarted(Synset synset) => SynsetDragStarted?.Invoke(synset);

        internal event SynsetDragCancelled_EventHandler SynsetDragCancelled;
        private void OnSynsetDragCancelled(Synset synset) => SynsetDragCancelled?.Invoke(synset);

        internal event SynsetDropCompleted_EventHandler SynsetDropCompleted;
        private void OnSynsetDropCompleted(Synset synset) => SynsetDropCompleted?.Invoke(synset);

        #endregion Events

        #region Synset Navigation

        internal void GoToPrevious() => ViewModel.Undo();
        internal void GoToNext() => ViewModel.Redo();


        #region Common to all synset parts of speech

        private Synset SelectedHypernym => (Synset)HypernymsList.SelectedItem;
        private void HypernymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedHypernym);

        private Synset SelectedHyponym => (Synset)HyponymsList.SelectedItem;
        private void HyponymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedHyponym);

        private Synset SelectedSeeAlso => (Synset)SeeAlsoList.SelectedItem;
        private void SeeAlsoList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedSeeAlso);

        #endregion Common to all synset parts of speech


        #region Noun-specific relations

        private HolonymsOfResult SelectedHolonym => (HolonymsOfResult)HolonymsList.SelectedItem;
        private void HolonymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(Synset.WithID(SelectedHolonym.ID));

        private MeronymsOfResult SelectedMeronym => (MeronymsOfResult)MeronymsList.SelectedItem;
        private void MeronymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(Synset.WithID(SelectedMeronym.ID));

        private Synset SelectedType => (Synset)TypesList.SelectedItem;
        private void TypesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedType);

        private Synset SelectedInstance => (Synset)InstancesList.SelectedItem;
        private void InstancesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedInstance);

        private Synset SelectedValueOfAttribute => (Synset)ValuesOfAttributeList.SelectedItem;
        private void ValuesOfAttributeList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedValueOfAttribute);

        #endregion Noun-specific relations


        #region Verb-specific relations

        private Synset SelectedIsCausedBy => (Synset)IsCausedByList.SelectedItem;
        private void IsCausedByList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedIsCausedBy);

        private Synset SelectedIsCauseOf => (Synset)IsCauseOfList.SelectedItem;
        private void IsCauseOfList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedIsCauseOf);

        private Synset SelectedIsEntailedBy => (Synset)IsEntailedByList.SelectedItem;
        private void IsEntailedByList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedIsEntailedBy);

        private Synset SelectedEntails => (Synset)EntailsList.SelectedItem;
        private void EntailsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedEntails);

        private Synset SelectedVerbGroupMember => (Synset)VerbGroupMembersList.SelectedItem;
        private void VerbGroupMembersList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedVerbGroupMember);

        #endregion Verb-specific relations


        #region Adjective-specific relations

        private Synset SelectedSatellite => (Synset)SatellitesList.SelectedItem;
        private void SatellitesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedSatellite);

        /// <summary>Simulates a MouseDoubleClick event, which is not available on TextBlock</summary>
        private void ClusterHeadTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2) ViewModel.SetCurrentSynset(ViewModel.CurrentSynset.ClusterHead);
        }

        private Synset SelectedClusterMember => (Synset)ClusterMembersList.SelectedItem;
        private void ClusterMembersList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedClusterMember);

        private Synset SelectedAttributeWithValue => (Synset)AttributesWithValueList.SelectedItem;
        private void AttributesWithValueList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.SetCurrentSynset(SelectedAttributeWithValue);

        #endregion Adjective-specific relations

        #endregion Synset Navigation

        #region Dragging Synsets from this control

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
                if (EditingIsEnabled && ViewModel.CurrentSynset != null)
                {
                    OnSynsetDragStarted(ViewModel.CurrentSynset);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(Synset), ViewModel.CurrentSynset);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Link | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnSynsetDragCancelled(ViewModel.CurrentSynset);
                            break;
                        case DragDropEffects.Link:
                            OnSynsetDropCompleted(ViewModel.CurrentSynset);
                            break;
                        default: break;
                    }
                }
            }
        }

        #endregion Dragging Synsets from this control

        #region Dragging and Dropping Synsets onto this control

        private void HypernymsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnHypernymsDrop(e);

        private void HyponymsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnHyponymsDrop(e);

        /// <summary>Adding a new Holonym has an extra step because we have to specify the type.</summary>
        private void HolonymsBorder_Drop(object sender, DragEventArgs e)
        {
            ViewModel.Edit.OnHolonymsDrop(e);
            HolonymTypesContextMenu.PlacementTarget = HolonymsList;
            HolonymTypesContextMenu.IsOpen = true;
        }
        private ContextMenu HolonymTypesContextMenu => FindResource("HolonymTypesContextMenu") as ContextMenu;
        private void HolonymType_Part_Click(object sender, RoutedEventArgs e) => OnAddHolonymSpecificationCompleted('p');
        private void HolonymType_Substance_Click(object sender, RoutedEventArgs e) => OnAddHolonymSpecificationCompleted('s');
        private void HolonymType_Member_Click(object sender, RoutedEventArgs e) => OnAddHolonymSpecificationCompleted('m');
        private void OnAddHolonymSpecificationCompleted(char newHolonymType)
        {
            HolonymTypesContextMenu.IsOpen = false;
            ViewModel.Edit.OnHolonymTypeSelected(newHolonymType);
        }

        /// <summary>Adding a new Meronym has an extra step because we have to specify the type.</summary>
        private void MeronymsBorder_Drop(object sender, DragEventArgs e)
        {
            ViewModel.Edit.OnMeronymsDrop(e);
            MeronymTypesContextMenu.PlacementTarget = HolonymsList;
            MeronymTypesContextMenu.IsOpen = true;
        }
        private ContextMenu MeronymTypesContextMenu => FindResource("MeronymTypesContextMenu") as ContextMenu;
        private void MeronymType_Part_Click(object sender, RoutedEventArgs e) => OnAddMeronymSpecificationCompleted('p');
        private void MeronymType_Substance_Click(object sender, RoutedEventArgs e) => OnAddMeronymSpecificationCompleted('s');
        private void MeronymType_Member_Click(object sender, RoutedEventArgs e) => OnAddMeronymSpecificationCompleted('m');
        private void OnAddMeronymSpecificationCompleted(char newMeronymType)
        {
            MeronymTypesContextMenu.IsOpen = false;
            ViewModel.Edit.OnMeronymTypeSelected(newMeronymType);
        }

        private void TypesBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnTypesDrop(e);

        private void InstancesBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnInstancesDrop(e);

        private void IsCausedByBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnIsCausedByDrop(e);

        private void IsCauseOfBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnIsCauseOfDrop(e);

        private void IsEntailedByBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnIsEntailedByDrop(e);

        private void EntailsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnEntailsDrop(e);

        private void SeeAlsoBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnSeeAlsoDrop(e);

        private void AttributesWithValueBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnAttributesWithValueDrop(e);

        private void ValuesOfAttributeBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnValuesOfAttributeDrop(e);


        #endregion Dragging and Dropping Synsets onto this control

        #region Dragging and Dropping Word Senses onto this control

        private void CurrentSynsetGlossBorder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(WordSense)))
            {
                WordSense droppedWordSense = (WordSense)e.Data.GetData(typeof(WordSense));
                ViewModel.SetCurrentSynset(droppedWordSense.LinkedSynset);
            }
        }

        #endregion Dragging and Dropping Word Senses onto this control

        #region Editing related synsets

        private void HypernymsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteHypernym(SelectedHypernym); }
        private void HyponymsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteHyponym(SelectedHyponym); }
        private void HolonymsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteHolonym(SelectedHolonym); }
        private void MeronymsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteMeronym(SelectedMeronym); }
        private void TypesBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteType(SelectedType); }
        private void InstancesBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteInstance(SelectedInstance); }
        private void IsCausedByBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteIsCausedBy(SelectedIsCausedBy); }
        private void IsCauseOfBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteIsCauseOf(SelectedIsCauseOf); }
        private void IsEntailedByBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteEntailedBy(SelectedIsEntailedBy); }
        private void EntailsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteEntails(SelectedEntails); }
        private void SeeAlsoBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteSeeAlso(SelectedSeeAlso); }
        private void AttributesWithValueBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteAttributeWithValue(SelectedAttributeWithValue); }
        private void ValuesOfAttributeBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteValueOfAttribute(SelectedValueOfAttribute); }

        #endregion Editing related synsets

    }
}
