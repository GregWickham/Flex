using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Flex.UserInterface.ViewModels;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for WordAlternativesSelector.xaml</summary>
    public partial class WordSelectorControl : UserControl
    {
        public WordSelectorControl()
        {
            InitializeComponent();
        }

        private WordSelectorViewModel viewModel;
        internal WordSelectorViewModel ViewModel
        {
            get => viewModel;
            set
            {
                viewModel = value;
                DataContext = viewModel;
            }
        }

        private void WordSelectorControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel.GetSynonyms();
            //AlternatesTab.IsSelected = ViewModel.HasAlternates;
        }

        private void PotentialList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
            {
                ViewModel.MoveFromPotentialToActual(PotentialList.SelectedItems.Cast<string>().ToList());
            }
        }

        private void ActualList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {

            }
        }

    }
}
