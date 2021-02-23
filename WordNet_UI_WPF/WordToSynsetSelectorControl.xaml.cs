using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordNet.Linq;
using WordNet.UserInterface.ViewModels;

namespace WordNet.UserInterface
{
    public delegate WordSpecification DroppedWordConverter(DragEventArgs e);

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
            ViewModel.LookupSynsetsMatching(new WordSpecification(LookupWordTextBox.Text, SelectedPartOfSpeech));
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

        private void LookupSynsetsFor(WordSpecification wordSpecification)
        {
            LookupWordTextBox.Text = wordSpecification.WordText;
            switch (wordSpecification.POS)
            {
                case WordNetData.PartOfSpeech.Unspecified:
                    AnyRadioButton.IsChecked = true;
                    break;
                case WordNetData.PartOfSpeech.Noun:
                    NounRadioButton.IsChecked = true;
                    break;
                case WordNetData.PartOfSpeech.Verb:
                    VerbRadioButton.IsChecked = true;
                    break;
                case WordNetData.PartOfSpeech.Adjective:
                    AdjectiveRadioButton.IsChecked = true;
                    break;
                case WordNetData.PartOfSpeech.Adverb:
                    AdverbRadioButton.IsChecked = true;
                    break;
            }
            ViewModel.LookupSynsetsMatching(wordSpecification);
        }

        private Synset SelectedSynsetMatchingWord => (Synset)SynsetsMatchingWordList.SelectedItem;

        private void SynsetsMatchingWordList_SelectionChanged(object sender, SelectionChangedEventArgs e) => UsageExamplesList.DataContext = SelectedSynsetMatchingWord;

        private void SynsetsMatchingWordList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OnSynsetSelected(SelectedSynsetMatchingWord);

        #region Drag / Drop of IElementTreeNode onto WordLookup

        internal DroppedWordConverter ConvertDroppedWordFrom { get; set; }

        private void WordLookup_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void WordLookup_Drop(object sender, DragEventArgs e)
        {
            WordSpecification droppedWord = ConvertDroppedWordFrom(e);
            if (droppedWord != null) LookupSynsetsFor(droppedWord);
        }


        #endregion Drag / Drop of IElementTreeNode onto WordLookup        

    }
}
