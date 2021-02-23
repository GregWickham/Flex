using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using WordNet.Linq;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class WordNetBrowserWindow : Window
    {
        public WordNetBrowserWindow()
        {
            InitializeComponent();
        }

        public WordNetBrowserWindow(
            SynsetDragStarted_EventHandler synsetDragStartedHandler, 
            SynsetDragCancelled_EventHandler synsetDragCancelledHandler,
            SynsetDropCompleted_EventHandler synsetDropCompletedHandler,
            WordSenseDragStarted_EventHandler wordSenseDragStartedHandler,
            WordSenseDragCancelled_EventHandler wordSenseDragCancelledHandler,
            WordSenseDropCompleted_EventHandler wordSenseDropCompletedHandler)
        {
            InitializeComponent();
            // Hook up external event handlers supplied to the constructor, and keep track of them
            if (synsetDragStartedHandler != null)
            {
                SynsetDragStarted += synsetDragStartedHandler;
                External_SynsetDragStarted_EventHandler = synsetDragStartedHandler;
            }
            if (synsetDragCancelledHandler != null)
            {
                SynsetDragCancelled += synsetDragCancelledHandler;
                External_SynsetDragCancelled_EventHandler = synsetDragCancelledHandler;
            }
            if (synsetDragStartedHandler != null)
            {
                SynsetDragStarted += synsetDragStartedHandler;
                External_SynsetDropCompleted_EventHandler = synsetDropCompletedHandler;
            }
            if (wordSenseDragStartedHandler != null)
            {
                WordSenseDragStarted += wordSenseDragStartedHandler;
                External_WordSenseDragStarted_EventHandler = wordSenseDragStartedHandler;
            }
            if (wordSenseDragCancelledHandler != null)
            {
                WordSenseDragCancelled += wordSenseDragCancelledHandler;
                External_WordSenseDragCancelled_EventHandler = wordSenseDragCancelledHandler;
            }
            if (wordSenseDragStartedHandler != null)
            {
                WordSenseDropCompleted += wordSenseDropCompletedHandler;
                External_WordSenseDropCompleted_EventHandler = wordSenseDropCompletedHandler;
            }
        }

        public WordNetBrowserWindow SetDroppedWordConverter(DroppedWordConverter converter)
        {
            WordToSynsetSelector.ConvertDroppedWordFrom = converter;
            return this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (External_SynsetDragStarted_EventHandler != null) SynsetDragStarted -= External_SynsetDragStarted_EventHandler;
            if (External_SynsetDragCancelled_EventHandler != null) SynsetDragCancelled -= External_SynsetDragCancelled_EventHandler;
            if (External_SynsetDropCompleted_EventHandler != null) SynsetDropCompleted -= External_SynsetDropCompleted_EventHandler;
            if (External_WordSenseDragStarted_EventHandler != null) WordSenseDragStarted -= External_WordSenseDragStarted_EventHandler;
            if (External_WordSenseDragCancelled_EventHandler != null) WordSenseDragCancelled -= External_WordSenseDragCancelled_EventHandler;
            if (External_WordSenseDropCompleted_EventHandler != null) WordSenseDropCompleted -= External_WordSenseDropCompleted_EventHandler;
        }

        private Dictionary<Type, Func<WordSpecification>> DroppedObjectConverters = new Dictionary<Type, Func<WordSpecification>>();

        #region External event handlers that can optionally be attached on construction of this Window

        private SynsetDragStarted_EventHandler External_SynsetDragStarted_EventHandler;
        private SynsetDragCancelled_EventHandler External_SynsetDragCancelled_EventHandler;
        private SynsetDropCompleted_EventHandler External_SynsetDropCompleted_EventHandler;
        private WordSenseDragStarted_EventHandler External_WordSenseDragStarted_EventHandler;
        private WordSenseDragCancelled_EventHandler External_WordSenseDragCancelled_EventHandler;
        private WordSenseDropCompleted_EventHandler External_WordSenseDropCompleted_EventHandler;

        #endregion External event handlers that can optionally be attached on construction of this Window

        #region Events

        public event SynsetDragStarted_EventHandler SynsetDragStarted;
        public event SynsetDragCancelled_EventHandler SynsetDragCancelled;
        public event SynsetDropCompleted_EventHandler SynsetDropCompleted;

        public event WordSenseDragStarted_EventHandler WordSenseDragStarted;
        public event WordSenseDragCancelled_EventHandler WordSenseDragCancelled;
        public event WordSenseDropCompleted_EventHandler WordSenseDropCompleted;

        #endregion Events

        #region Control Event Handlers

        private void SynsetNavigator_SynsetDragStarted(Synset synset) => SynsetDragStarted?.Invoke(synset);
        private void SynsetNavigator_SynsetDragCancelled(Synset synset) => SynsetDragCancelled?.Invoke(synset);
        private void SynsetNavigator_SynsetDropCompleted(Synset synset) => SynsetDropCompleted?.Invoke(synset);

        private void WordToSynsetSelector_SynsetSelected(Synset synset)
        {
            SynsetNavigator.Visibility = Visibility.Visible;
            ExpandCollapseWordFinderButton.IsChecked = false;
            OnSelectedSynsetChanged(synset);
        }

        private void SynsetNavigator_SynsetSelected(Synset synset) => OnSelectedSynsetChanged(synset);

        private void OnSelectedSynsetChanged(Synset selectedSynset)
        {
            SynsetNavigator.CurrentSynset = selectedSynset;
            WordSensesControl.CurrentSynset = selectedSynset;
        }

        #endregion Control Event Handlers

        #region Menu and Toolbar

        private void NewWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Duplicate the external events handlers from this window to the new window.
            new WordNetBrowserWindow(
                External_SynsetDragStarted_EventHandler,
                External_SynsetDragCancelled_EventHandler,
                External_SynsetDropCompleted_EventHandler,
                External_WordSenseDragStarted_EventHandler,
                External_WordSenseDragCancelled_EventHandler,
                External_WordSenseDropCompleted_EventHandler)
            .Show();
        }

        private void ExpandCollapseWordFinderButton_Checked(object sender, RoutedEventArgs e) 
        { 
            if (ExpandCollapseWordFinderImage != null) ExpandCollapseWordFinderImage.Source = ChevronUpImage;
            if (ExpandCollapseWordFinderTextBlock != null) ExpandCollapseWordFinderTextBlock.Text = "Hide synset from word finder";
        }
        private void ExpandCollapseWordFinderButton_Unchecked(object sender, RoutedEventArgs e) 
        { 
            if (ExpandCollapseWordFinderImage != null) ExpandCollapseWordFinderImage.Source = ChevronDownImage;
            if (ExpandCollapseWordFinderTextBlock != null) ExpandCollapseWordFinderTextBlock.Text = "Find synset from word";
        }
        private void ExpandCollapseWordSensesButton_Checked(object sender, RoutedEventArgs e) 
        { 
            if (ExpandCollapseWordSensesImage != null) ExpandCollapseWordSensesImage.Source = ChevronLeftImage;
            if (ExpandCollapseWordSensesTextBlock != null) ExpandCollapseWordSensesTextBlock.Text = "Hide word senses";
        }
        private void ExpandCollapseWordSensesButton_Unchecked(object sender, RoutedEventArgs e) 
        { 
            if (ExpandCollapseWordSensesImage != null) ExpandCollapseWordSensesImage.Source = ChevronRightImage;
            if (ExpandCollapseWordSensesTextBlock != null) ExpandCollapseWordSensesTextBlock.Text = "Show word senses";
        }

        private static readonly BitmapImage ChevronDownImage = new BitmapImage(new Uri("./Resources/Images/Chevron_Down.png", UriKind.Relative));
        private static readonly BitmapImage ChevronUpImage = new BitmapImage(new Uri("./Resources/Images/Chevron_Up.png", UriKind.Relative));
        private static readonly BitmapImage ChevronLeftImage = new BitmapImage(new Uri("./Resources/Images/Chevron_Left.png", UriKind.Relative));
        private static readonly BitmapImage ChevronRightImage = new BitmapImage(new Uri("./Resources/Images/Chevron_Right.png", UriKind.Relative));

        #endregion Menu and Toolbar

    }
}
