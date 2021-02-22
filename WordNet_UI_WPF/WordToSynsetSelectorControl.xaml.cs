using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlexibleRealization;
using WordNet.Linq;
using WordNet.UserInterface.ViewModels;

namespace WordNet.UserInterface
{
    internal delegate void SynsetSelected_EventHandler(Synset synset);

    /// <summary>Interaction logic for WordToSynsetSelector.xaml</summary>
    public partial class WordToSynsetSelectorControl : UserControl
    {
        public WordToSynsetSelectorControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => DataContext = new AvailableSynsetsViewModel();

        internal event SynsetSelected_EventHandler SynsetSelected;
        private void OnSynsetSelected(Synset synset) => SynsetSelected?.Invoke(synset);

        private AvailableSynsetsViewModel ViewModel => (AvailableSynsetsViewModel)DataContext;

        private void LookupWordTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) { if (e.Key == Key.Return) LookupEnteredWord(); }

        private void LookupEnteredWord()
        {
            if ((bool)AnyRadioButton.IsChecked) ViewModel.LookupSynsetsMatching(LookupWordTextBox.Text);
            else ViewModel.LookupSynsetsMatching(LookupWordTextBox.Text, SelectedPartOfSpeech);

        }

        private WordNetData.PartOfSpeech SelectedPartOfSpeech
        {
            get
            {
                if ((bool)NounRadioButton.IsChecked) return WordNetData.PartOfSpeech.Noun;
                else if ((bool)VerbRadioButton.IsChecked) return WordNetData.PartOfSpeech.Verb;
                else if ((bool)AdjectiveRadioButton.IsChecked) return WordNetData.PartOfSpeech.Adjective;
                else if ((bool)AdverbRadioButton.IsChecked) return WordNetData.PartOfSpeech.Adverb;
                else return WordNetData.PartOfSpeech.Unspecified;
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

        private void SynsetsMatchingWordList_SelectionChanged(object sender, SelectionChangedEventArgs e) => UsageExamplesList.DataContext = SelectedSynsetMatchingWord;

        private void SynsetsMatchingWordList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OnSynsetSelected(SelectedSynsetMatchingWord);

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
                Task<IElementTreeNode> getNodeTask = (Task<IElementTreeNode>)e.Data.GetData(typeof(Task));
                droppedNode = await getNodeTask;
            }
            if (droppedNode != null) LookupSynsetsFor(droppedNode);
        }


        #endregion Drag / Drop of IElementTreeNode onto WordLookup        

    }
}
