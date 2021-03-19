using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordNet.Linq;

namespace WordNet.UserInterface
{
    /// <summary>Interaction logic for WordSensesForSynsetControl.xaml</summary>
    public partial class WordSensesForSynsetControl : UserControl
    {
        public WordSensesForSynsetControl()
        {
            InitializeComponent();
        }

        internal void SetCurrentSynset(Synset newCurrentSynset)
        {
            if (newCurrentSynset != WordSensesForSynsetList.DataContext)
            {
                WordSensesForSynsetList.DataContext = newCurrentSynset;
                // If the new current synset has only one associated word sense, go ahead and select that word sense and make it current.
                // If there are multiple associated word senses, the user has to choose one in order to make it the focus of this control.
                if (newCurrentSynset.WordSensesForSynset.Count() == 1)
                {
                    WordSense theOnlyWordSenseForThisSynset = newCurrentSynset.WordSensesForSynset.Single();
                    WordSensesForSynsetList.SelectedItem = theOnlyWordSenseForThisSynset;
                    WordSenseNavigator.ViewModel.CurrentWordSense = theOnlyWordSenseForThisSynset;
                    WordSenseNavigator.Visibility = Visibility.Visible;
                }
                else
                {
                    WordSensesForSynsetList.SelectedItem = null;
                    WordSenseNavigator.ViewModel.CurrentWordSense = null;
                    WordSenseNavigator.Visibility = Visibility.Hidden;
                }
            }
        }

        private WordSense SelectedWordSenseForSynset => (WordSense)WordSensesForSynsetList.SelectedItem;

        private void WordSensesForSynsetList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WordSenseNavigator.ViewModel.CurrentWordSense = SelectedWordSenseForSynset;
            WordSenseNavigator.Visibility = Visibility.Visible;
        }

        #region Events

        public event WordSenseDragStarted_EventHandler WordSenseDragStarted;
        public event WordSenseDragCancelled_EventHandler WordSenseDragCancelled;
        public event WordSenseDropCompleted_EventHandler WordSenseDropCompleted;

        #endregion Events

        #region Control Event Handlers

        private void WordSenseNavigator_WordSenseDragStarted(WordSense wordSense) => WordSenseDragStarted?.Invoke(wordSense);
        private void WordSenseNavigator_WordSenseDragCancelled(WordSense wordSense) => WordSenseDragCancelled?.Invoke(wordSense);
        private void WordSenseNavigator_WordSenseDropCompleted(WordSense wordSense) => WordSenseDropCompleted?.Invoke(wordSense);

        #endregion Control Event Handlers
    }
}
