using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using WordNet.Linq;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for WordNetBrowserWindow.xaml</summary>
    public partial class WordNetBrowserWindow : Window
    {
        public WordNetBrowserWindow()
        {
            InitializeComponent();
        }

        public WordNetBrowserWindow AddDroppedWordConverter(DroppedWordConverter converter)
        {
            WordToSynsetSelector.ConvertDroppedWordFrom = converter;
            return this;
        }

        internal void RegisterForEventsFrom(WordNetBrowserWindow anotherWordNetBrowser)
        {
            anotherWordNetBrowser.SynsetDragStarted += WordNetBrowser_SynsetDragStarted;
            anotherWordNetBrowser.SynsetDragCancelled += WordNetBrowser_SynsetDragCancelled;
            anotherWordNetBrowser.SynsetDropCompleted += WordNetBrowser_SynsetDropCompleted;

            anotherWordNetBrowser.WordSenseDragStarted += WordNetBrowser_WordSenseDragStarted;
            anotherWordNetBrowser.WordSenseDragCancelled += WordNetBrowser_WordSenseDragCancelled;
            anotherWordNetBrowser.WordSenseDropCompleted += WordNetBrowser_WordSenseDropCompleted;

            anotherWordNetBrowser.EditingEnabledChanged += WordNetBrowser_EditingEnabledChanged;
        }

        #region Events

        public event SynsetDragStarted_EventHandler SynsetDragStarted;
        public event SynsetDragCancelled_EventHandler SynsetDragCancelled;
        public event SynsetDropCompleted_EventHandler SynsetDropCompleted;

        public event WordSenseDragStarted_EventHandler WordSenseDragStarted;
        public event WordSenseDragCancelled_EventHandler WordSenseDragCancelled;
        public event WordSenseDropCompleted_EventHandler WordSenseDropCompleted;

        internal event EditingEnabledChanged_EventHandler EditingEnabledChanged;
        private void OnEditingEnabledChanged(bool editingEnabled)
        {
            SynsetNavigator.EditingIsEnabled = editingEnabled;
            WordSensesControl.WordSenseNavigator.EditingIsEnabled = editingEnabled;
            EditingEnabledChanged?.Invoke(editingEnabled);
        }

        #endregion Events

        #region Control Event Handlers

        private void EditEnabledButton_Checked(object sender, RoutedEventArgs e) => OnEditingEnabledChanged(true);
        private void EditEnabledButton_Unchecked(object sender, RoutedEventArgs e) => OnEditingEnabledChanged(false);

        private void SynsetNavigator_SynsetDragStarted(Synset synset) => SynsetDragStarted?.Invoke(synset);
        private void SynsetNavigator_SynsetDragCancelled(Synset synset) => SynsetDragCancelled?.Invoke(synset);
        private void SynsetNavigator_SynsetDropCompleted(Synset synset) => SynsetDropCompleted?.Invoke(synset);

        private void WordToSynsetSelector_SynsetSelected(Synset selectedSynset)
        {
            SynsetNavigator.Visibility = Visibility.Visible;
            ExpandCollapseWordFinderButton.IsChecked = false;
            SynsetNavigator.ViewModel.SetCurrentSynset(selectedSynset);
        }

        private void SynsetNavigator_SynsetSelected(Synset selectedSynset) => WordSensesControl.SetCurrentSynset(selectedSynset);

        private void WordSensesControl_WordSenseDragStarted(WordSense wordSense)
        {
            SynsetNavigator.ViewModel.SetDropTargets_ForWordSense(wordSense);
            WordSenseDragStarted?.Invoke(wordSense);
        }

        private void WordSensesControl_WordSenseDragCancelled(WordSense wordSense)
        {
            SynsetNavigator.ViewModel.ClearDropTargets();
            WordSenseDragCancelled?.Invoke(wordSense);
        }

        private void WordSensesControl_WordSenseDropCompleted(WordSense wordSense)
        {
            SynsetNavigator.ViewModel.ClearDropTargets();
            WordSenseDropCompleted?.Invoke(wordSense);
        }

        #endregion Control Event Handlers

        #region Handlers for events from other windows

        private void WordNetBrowser_SynsetDragStarted(Synset synset) => SynsetNavigator.ViewModel.Edit.SetDropTargets_ForSynset(synset);
        private void WordNetBrowser_SynsetDragCancelled(Synset synset) => SynsetNavigator.ViewModel.Edit.ClearDropTargets();
        private void WordNetBrowser_SynsetDropCompleted(Synset synset) => SynsetNavigator.ViewModel.Edit.ClearDropTargets();

        private void WordNetBrowser_WordSenseDragStarted(WordSense wordSense)
        {
            if (WordSensesControl.WordSenseNavigator.EditingIsEnabled)
            {
                WordSensesControl.WordSenseNavigator.ViewModel.Edit.SetDropTargets_ForWordSense(wordSense);
            }
        }
        private void WordNetBrowser_WordSenseDragCancelled(WordSense wordSense) => WordSensesControl.WordSenseNavigator.ViewModel.Edit.ClearDropTargets();
        private void WordNetBrowser_WordSenseDropCompleted(WordSense wordSense) => WordSensesControl.WordSenseNavigator.ViewModel.Edit.ClearDropTargets();

        private void WordNetBrowser_EditingEnabledChanged(bool editingEnabled) 
        { 
            if (editingEnabled != EditEnabledButton.IsChecked) EditEnabledButton.IsChecked = editingEnabled; 
        }

        #endregion Handlers for events from other windows

        #region Menu and Toolbar

        private void NewWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WordNetBrowserWindow newWindow = new WordNetBrowserWindow();

            // Duplicate the external events handlers from this window to the new window.
            newWindow.SynsetDragStarted += SynsetDragStarted;
            newWindow.SynsetDragCancelled += SynsetDragCancelled;
            newWindow.SynsetDropCompleted += SynsetDropCompleted;
            newWindow.WordSenseDragStarted += WordSenseDragStarted;
            newWindow.WordSenseDragCancelled += WordSenseDragCancelled;
            newWindow.WordSenseDropCompleted += WordSenseDropCompleted;

            // Register to receive events from the new window
            RegisterForEventsFrom(newWindow);
            // Have the new window register to receive drag drop events from this window
            newWindow.RegisterForEventsFrom(this);

            newWindow.Show();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e) => SynsetNavigator.GoToPrevious();
        private void NextButton_Click(object sender, RoutedEventArgs e) => SynsetNavigator.GoToNext();

        #endregion Menu and Toolbar

    }
}
