using System.Windows.Controls;
using System.Windows.Input;
using Flex.UserInterface.ViewModels;
using Datamuse;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for FlexibleWordEditor.xaml</summary>
    public partial class FlexibleWordEditor : UserControl
    {
        public FlexibleWordEditor()
        {
            InitializeComponent();
        }

        public IWordEditorViewModel ViewModel
        {
            get => (IWordEditorViewModel)DataContext;
            set
            {
                DataContext = value;
                WeightsOrColumnsButton.IsChecked = false;
            }
        }

        /// <summary>The user has pressed a key in the PotentialList.</summary>
        private void PotentialList_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Insert) ViewModel.MoveFromPotentialToActual(PotentialList.SelectedItems); }

        /// <summary>The user has pressed a key in the ActualList.</summary>
        private void ActualList_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Delete) ViewModel.MoveFromActualToPotential(ActualList.SelectedItems); }

        #region Potential Control Panel

        private void SynonymsRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e) => ViewModel?.SetRelationToPivot(WordRelation.Synonym);

        private void MeaningLikeRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e) => ViewModel?.SetRelationToPivot(WordRelation.MeaningLike);

        private void PotentialListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem doubleClickedItem = (ListBoxItem)sender;
            string doubleClickedString = (string)doubleClickedItem.DataContext;
            PivotTextBox.Text = doubleClickedString;
            ViewModel.SetPivot(doubleClickedString);
        }

        private void PivotTextBox_KeyUp(object sender, KeyEventArgs e) { if (e.Key.Equals(Key.Enter)) ViewModel.SetPivot(PivotTextBox.Text); }

        #endregion Potential Control Panel

    }
}
