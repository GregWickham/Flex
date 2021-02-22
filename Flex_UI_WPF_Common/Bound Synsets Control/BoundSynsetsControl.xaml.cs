using System.Windows;
using System.Windows.Controls;
using FlexibleRealization;
using WordNet.Linq;
using Flex.Database;
using Flex.UserInterface.ViewModels;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for BoundSynsetsControl.xaml</summary>
    public partial class BoundSynsetsControl : UserControl 
    {
        public BoundSynsetsControl()
        {
            InitializeComponent();
        }

        public void Clear() => ViewModel.Clear();

        public IElementTreeNode Element { set => ViewModel.ElementFilter = value; }

        public void BindSynsetToNode(IElementTreeNode boundNode, Synset boundSynset) => ViewModel.AddBinding(boundNode, boundSynset);

        public void LoadBindingsFor(IElementTreeNode tree) => ViewModel.LoadBindingsFor(tree);

        public void SaveBindingsFor(IElementTreeNode tree) => ViewModel.SaveBindingsFor(tree);

        /// <summary>On load, create a view model that will last for the lifetime of this control.</summary>
        private void BoundSynsetsControl_Loaded(object sender, RoutedEventArgs e) => DataContext = new BoundSynsetsViewModel();

        private BoundSynsetsViewModel ViewModel => (BoundSynsetsViewModel)DataContext;

        private SynsetToElementBinding SelectedBinding => (SynsetToElementBinding)SynsetBindingsGrid.SelectedItem;

        private void SynsetBindingsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = SelectedBinding != null;
        }

        /// <summary>Delete the selected synset binding.</summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e) => ViewModel.DeleteBinding(SelectedBinding);

    }
}
