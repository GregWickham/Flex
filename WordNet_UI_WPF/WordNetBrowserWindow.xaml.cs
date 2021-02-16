using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using FlexibleRealization;
using WordNet.Linq;
using WordNet.UserInterface.ViewModels;
using System;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class WordNetBrowserWindow : Window
    {
        public WordNetBrowserWindow()
        {
            InitializeComponent();
        }

        public event SynsetDragStarted_EventHandler SynsetDragStarted;
        private void OnSynsetDragStarted(int draggedSynsetID) => SynsetDragStarted?.Invoke(draggedSynsetID);


        public event SynsetDragCancelled_EventHandler SynsetDragCancelled;
        private void OnSynsetDragCancelled(int draggedSynsetID) => SynsetDragCancelled?.Invoke(draggedSynsetID);


        public event SynsetDropCompleted_EventHandler SynsetDropCompleted;
        private void OnSynsetDropCompleted(int draggedSynsetID) => SynsetDropCompleted?.Invoke(draggedSynsetID);


        private void Window_Loaded(object sender, RoutedEventArgs e) => DataContext = new AvailableSynsetsViewModel();


        #region Synset Navigation

        private Synset NavigatorCurrentSynset => (Synset)SynsetNavigator.DataContext;


        private Synset SelectedHypernym => (Synset)HypernymsList.SelectedItem;
        private void HypernymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SynsetNavigator.DataContext = SelectedHypernym;


        private Synset SelectedHyponym => (Synset)HyponymsList.SelectedItem;
        private void HyponymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SynsetNavigator.DataContext = SelectedHyponym;


        private Synset SelectedHolonym => (Synset)HolonymsList.SelectedItem;
        private void HolonymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SynsetNavigator.DataContext = SelectedHolonym;


        private Synset SelectedMeronym => (Synset)MeronymsList.SelectedItem;
        private void MeronymsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SynsetNavigator.DataContext = SelectedMeronym;


        #endregion Synset Navigation


        #region Available Synsets

        private AvailableSynsetsViewModel ViewModel => (AvailableSynsetsViewModel)DataContext;

        private void LookupWordTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) { if (e.Key == Key.Return) LookupEnteredWord(); }

        private void LookupEnteredWord()
        {
            if ((bool)AnyRadioButton.IsChecked) ViewModel.LookupSynsetsMatching(LookupWordTextBox.Text);
            else ViewModel.LookupSynsetsMatching(LookupWordTextBox.Text, SelectedPartOfSpeech);

        }

        private char SelectedPartOfSpeech
        {
            get
            {
                if ((bool)NounRadioButton.IsChecked) return 'n';
                else if ((bool)VerbRadioButton.IsChecked) return 'v';
                else if ((bool)AdjectiveRadioButton.IsChecked) return 'a';
                else return 'r';
            }
        }

        private void LookupSynsetsFor(IElementTreeNode node)
        {
            if (node is WordElementBuilder wordBuilder)
            {
                switch (wordBuilder)
                {
                    case NounBuilder noun:
                        LookupWordTextBox.Text = noun.WordSource.DefaultWord;
                        NounRadioButton.IsChecked = true;
                        ViewModel.LookupSynsetsMatching(noun);
                        break;
                    case VerbBuilder verb:
                        LookupWordTextBox.Text = verb.WordSource.DefaultWord;
                        VerbRadioButton.IsChecked = true;
                        ViewModel.LookupSynsetsMatching(verb);
                        break;
                    case AdjectiveBuilder adjective:
                        LookupWordTextBox.Text = adjective.WordSource.DefaultWord;
                        AdjectiveRadioButton.IsChecked = true;
                        ViewModel.LookupSynsetsMatching(adjective);
                        break;
                    case AdverbBuilder adverb:
                        LookupWordTextBox.Text = adverb.WordSource.DefaultWord;
                        AdverbRadioButton.IsChecked = true;
                        ViewModel.LookupSynsetsMatching(adverb);
                        break;
                }
            }
        }

        private Synset SelectedSynsetMatchingWord => (Synset)SynsetsMatchingWordList.SelectedItem;

        private void SynsetsMatchingWordList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => UsageExamplesList.DataContext = SelectedSynsetMatchingWord;

        private void SynsetsMatchingWordList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SynsetNavigator.DataContext = SelectedSynsetMatchingWord;


        #region Drag / Drop of IElementTreeNode onto WordLookup

        private void WordLookup_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private async void WordLookup_Drop(object sender, DragEventArgs e)
        {
            IElementTreeNode droppedNode = null;
            // We could get a dropped IElementTree node in one of two forms:
            // 1.  It's in the IDataObject as an IElementTreeNode, ready to use; or
            // 2.  It's in the IDataObject as a Task<ElementBuilder> that we can run to get the IElementTreeNode
            if (e.Data.GetDataPresent(typeof(IElementTreeNode)))
            {
                droppedNode = (IElementTreeNode)e.Data.GetData(typeof(IElementTreeNode));
            }
            else if (e.Data.GetDataPresent(typeof(Task)))
            {
                Task<ElementBuilder> getElementBuilderTask = (Task<ElementBuilder>)e.Data.GetData(typeof(Task));
                droppedNode = await getElementBuilderTask;
            }
            if (droppedNode != null) LookupSynsetsFor(droppedNode); 
        }


        #endregion Drag / Drop of IElementTreeNode onto WordLookup        

        #region Drag / Drop of Synsets from this window

        private Point mouseDownPosition;
        private void CurrentSynsetTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => mouseDownPosition = e.GetPosition(this);

        private void CurrentSynsetTextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);
            Vector mouseDownDistanceMoved = mouseDownPosition - currentMousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (NavigatorCurrentSynset != null)
                {
                    OnSynsetDragStarted(NavigatorCurrentSynset.ID);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(int), NavigatorCurrentSynset.ID);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Link | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnSynsetDragCancelled(NavigatorCurrentSynset.ID);
                            break;
                        case DragDropEffects.Link:
                            OnSynsetDropCompleted(NavigatorCurrentSynset.ID);
                            break;
                        default: break;
                    }
                }
            }
        }

        #endregion Drag / Drop of Synsets from this window

        #endregion Available Synsets

    }
}
