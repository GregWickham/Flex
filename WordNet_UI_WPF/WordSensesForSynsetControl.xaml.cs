using System;
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

        internal Synset CurrentSynset
        {
            get => (Synset)WordSensesForSynsetList.DataContext;
            set
            {
                WordSensesForSynsetList.DataContext = value;
                WordSensesForSynsetList.SelectedItem = null;
                WordSenseNavigator.CurrentWordSense = null;
                WordSenseNavigator.Visibility = Visibility.Hidden;
            }
        }

        private WordSense SelectedWordSenseForSynset => (WordSense)WordSensesForSynsetList.SelectedItem;

        private void WordSensesForSynsetList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WordSenseNavigator.CurrentWordSense = SelectedWordSenseForSynset;
            WordSenseNavigator.Visibility = Visibility.Visible;
        }
    }
}
