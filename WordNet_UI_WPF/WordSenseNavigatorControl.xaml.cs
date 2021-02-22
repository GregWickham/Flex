using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordNet.Linq;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for WordSenseNavigatorControl.xaml</summary>
    public partial class WordSenseNavigatorControl : UserControl
    {
        public WordSenseNavigatorControl()
        {
            InitializeComponent();
        }

        public WordSense CurrentWordSense
        {
            get => (WordSense)DataContext;
            set
            {
                if (value != null & value != DataContext)
                {
                    DataContext = value;
                    NounSpecificRelations.Visibility = value.IsNoun ? Visibility.Visible : Visibility.Collapsed;
                    VerbSpecificRelations.Visibility = value.IsVerb ? Visibility.Visible : Visibility.Collapsed;
                    FullWidthVerbSpecificRelations.Visibility = value.IsVerb ? Visibility.Visible : Visibility.Collapsed;
                    AdjectiveSpecificRelations.Visibility = value.IsAdjective ? Visibility.Visible : Visibility.Collapsed;
                    AdverbSpecificRelations.Visibility = value.IsAdverb ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private static readonly GridLength StarColumnWidth = new GridLength(1, GridUnitType.Star);


        #region Events

        internal event WordSenseDragStarted_EventHandler WordSenseDragStarted;
        private void OnWordSenseDragStarted(WordSense wordSense) => WordSenseDragStarted?.Invoke(wordSense);

        internal event WordSenseDragCancelled_EventHandler WordSenseDragCancelled;
        private void OnWordSenseDragCancelled(WordSense wordSense) => WordSenseDragCancelled?.Invoke(wordSense);

        internal event WordSenseDropCompleted_EventHandler WordSenseDropCompleted;
        private void OnWordSenseDropCompleted(WordSense wordSense) => WordSenseDropCompleted?.Invoke(wordSense);

        #endregion Events


        #region Word Sense Navigation

        #region Common to all word sense parts of speech

        private WordSense SelectedAntonym => (WordSense)AntonymsList.SelectedItem;
        private void AntonymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedAntonym;

        private WordSense SelectedDerivation => (WordSense)DerivationsList.SelectedItem;
        private void DerivationsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedDerivation;

        private WordSense SelectedPertainsTo => (WordSense)PertainsToList.SelectedItem;
        private void PertainsToList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedPertainsTo;

        private WordSense SelectedPertainer => (WordSense)AntonymsList.SelectedItem;
        private void PertainersList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedPertainer;

        private WordSense SelectedSeeAlso => (WordSense)AntonymsList.SelectedItem;
        private void SeeAlsoList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedSeeAlso;

        #endregion Common to all word sense parts of speech


        #region Noun-specific relations


        #endregion Noun-specific relations


        #region Verb-specific relations
        private WordSense SelectedParticipleForm => (WordSense)ParticipleFormsList.SelectedItem;
        private void ParticipleFormsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedParticipleForm;

        #endregion Verb-specific relations


        #region Adjective-specific relations
        private WordSense SelectedDerivedAdverb => (WordSense)DerivedAdverbsList.SelectedItem;
        private void DerivedAdverbsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedDerivedAdverb;

        private WordSense SelectedBaseVerbFormOfParticiple => (WordSense)BaseVerbFormsOfParticipleList.SelectedItem;
        private void BaseVerbFormsOfParticipleList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedBaseVerbFormOfParticiple;

        #endregion Adjective-specific relations


        #region Adverb-specific relations
        private WordSense SelectedDerivedFromAdjective => (WordSense)DerivedFromAdjectivesList.SelectedItem;
        private void DerivedFromAdjectivesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => CurrentWordSense = SelectedDerivedFromAdjective;

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
                if (CurrentWordSense != null)
                {
                    //OnWordSenseDragStarted(CurrentWordSense.SynsetID, CurrentWordSense.WordNumber);
                    //DataObject dataObject = new DataObject();
                    //dataObject.SetData(typeof(int), CurrentWordSense.ID);
                    //DragDropEffects dragResult = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Link | DragDropEffects.None);
                    //switch (dragResult)
                    //{
                    //    case DragDropEffects.None:
                    //        OnSynsetDragCancelled(CurrentSynset.ID);
                    //        break;
                    //    case DragDropEffects.Link:
                    //        OnSynsetDropCompleted(CurrentSynset.ID);
                    //        break;
                    //    default: break;
                    //}
                }
            }
        }

        #endregion Drag / Drop of Word Senses from this control

    }
}
