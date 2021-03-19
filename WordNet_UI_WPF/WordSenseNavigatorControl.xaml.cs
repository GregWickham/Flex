using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordNet.Linq;
using WordNet.UserInterface.ViewModels;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for WordSenseNavigatorControl.xaml</summary>
    public partial class WordSenseNavigatorControl : UserControl
    {
        public WordSenseNavigatorControl()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        internal WordSenseNavigatorViewModel ViewModel { get; } = new WordSenseNavigatorViewModel();

        internal bool EditingIsEnabled { get; set; } = false;

        #region Events

        public event WordSenseDragStarted_EventHandler WordSenseDragStarted;
        private void OnWordSenseDragStarted(WordSense wordSense) => WordSenseDragStarted?.Invoke(wordSense);

        public event WordSenseDragCancelled_EventHandler WordSenseDragCancelled;
        private void OnWordSenseDragCancelled(WordSense wordSense) => WordSenseDragCancelled?.Invoke(wordSense);

        public event WordSenseDropCompleted_EventHandler WordSenseDropCompleted;
        private void OnWordSenseDropCompleted(WordSense wordSense) => WordSenseDropCompleted?.Invoke(wordSense);

        public event WordSenseSelected_EventHandler WordSenseSelected;
        private void OnWordSenseSelected(WordSense wordSense)
        {
            ViewModel.CurrentWordSense = wordSense;
            WordSenseSelected?.Invoke(wordSense);
        }

        #endregion Events


        #region Word Sense Navigation

        #region Common to all word sense parts of speech
        private WordSense SelectedAntonym => (WordSense)AntonymsList.SelectedItem;
        private void AntonymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedAntonym;

        private WordSense SelectedDerivation => (WordSense)DerivationsList.SelectedItem;
        private void DerivationsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedDerivation;

        private WordSense SelectedSeeAlso => (WordSense)SeeAlsoList.SelectedItem;
        private void SeeAlsoList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedSeeAlso;

        #endregion Common to all word sense parts of speech


        #region Noun- or Adjective-specific relations
        private WordSense SelectedPertainer => (WordSense)PertainersList.SelectedItem;
        private void PertainersList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedPertainer;

        #endregion Noun- or Adjective-specific relations


        #region Verb-specific relations
        private WordSense SelectedParticipleForm => (WordSense)ParticipleFormsList.SelectedItem;
        private void ParticipleFormsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedParticipleForm;

        #endregion Verb-specific relations


        #region Adjective-specific relations
        private WordSense SelectedPertainsTo => (WordSense)PertainsToList.SelectedItem;
        private void PertainsToList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedPertainsTo;

        private WordSense SelectedDerivedAdverb => (WordSense)DerivedAdverbsList.SelectedItem;
        private void DerivedAdverbsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedDerivedAdverb;

        private WordSense SelectedBaseVerbFormOfParticiple => (WordSense)BaseVerbFormsOfParticipleList.SelectedItem;
        private void BaseVerbFormsOfParticipleList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedBaseVerbFormOfParticiple;

        #endregion Adjective-specific relations


        #region Adverb-specific relations
        private WordSense SelectedAdjectiveBaseOfDerivedAdverb => (WordSense)AdjectiveBasesOfDerivedAdverbList.SelectedItem;
        private void AdjectiveBasesOfDerivedAdverbList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ViewModel.CurrentWordSense = SelectedAdjectiveBaseOfDerivedAdverb;

        #endregion Adverb-specific relations


        #endregion Word Sense Navigation


        #region Drag / Drop of Word Senses from this control

        private Point mouseDownPosition;
        private void CurrentWordSenseHeader_MouseDown(object sender, MouseButtonEventArgs e) => mouseDownPosition = e.GetPosition(this);

        private void CurrentWordSenseHeader_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);
            Vector mouseDownDistanceMoved = mouseDownPosition - currentMousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (ViewModel.CurrentWordSense != null)
                {
                    OnWordSenseDragStarted(ViewModel.CurrentWordSense);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(WordSense), ViewModel.CurrentWordSense);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnWordSenseDragCancelled(ViewModel.CurrentWordSense);
                            break;
                        case DragDropEffects.Move:
                            OnWordSenseDropCompleted(ViewModel.CurrentWordSense);
                            break;
                        default: break;
                    }
                }
            }
        }

        #endregion Drag / Drop of Word Senses from this control

        #region Dragging and Dropping WordSenses onto this control
        private void AntonymsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnAntonymsDrop(e);
        private void DerivationsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnDerivationsDrop(e);
        private void SeeAlsoBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnSeeAlsoDrop(e);
        private void BaseVerbFormsOfParticipleBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnBaseVerbFormsOfParticipleDrop(e);
        private void ParticipleFormsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnParticipleFormsDrop(e);
        private void AdjectiveBasesOfDerivedAdverbBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnAdjectiveBasesOfDerivedAdverbDrop(e);
        private void DerivedAdverbsBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnDerivedAdverbsDrop(e);
        private void PertainersBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnPertainersDrop(e);
        private void PertainsToBorder_Drop(object sender, DragEventArgs e) => ViewModel.Edit.OnPertainsToDrop(e);

        #endregion Dragging and Dropping WordSenses onto this control

        #region Editing related word senses
        private void AntonymsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteAntonym(SelectedAntonym); }
        private void DerivationsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteDerivation(SelectedDerivation); }
        private void SeeAlsoBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteSeeAlso(SelectedSeeAlso); }
        private void BaseVerbFormsOfParticipleBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteBaseVerbFormOfParticiple(SelectedBaseVerbFormOfParticiple); }
        private void ParticipleFormsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteParticipleForm(SelectedParticipleForm); }
        private void AdjectiveBasesOfDerivedAdverbBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteAdjectiveBaseOfDerivedAdverb(SelectedAdjectiveBaseOfDerivedAdverb); }
        private void DerivedAdverbsBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeleteDerivedAdverb(SelectedDerivedAdverb); }
        private void PertainersBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeletePertainer(SelectedPertainer); }
        private void PertainsToBorder_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete && EditingIsEnabled) ViewModel.Edit.DeletePertainsTo(SelectedPertainsTo); }

        #endregion Editing related word senses
    }
}
