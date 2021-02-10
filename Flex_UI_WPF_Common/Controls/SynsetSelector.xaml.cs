using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FlexibleRealization;
using WordNet.Linq;
using WordNet.Linq.ViewModels;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for SynsetSelector.xaml</summary>
    public partial class SynsetSelector : UserControl
    {
        public SynsetSelector()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = new SynsetsViewModel();
        }

        private SynsetsViewModel ViewModel => (SynsetsViewModel)DataContext;

        public WordElementBuilder WordBuilder
        {
            set => ViewModel.WordBuilder = value;
        }

        public bool SelectedSynsetIsBound => ViewModel.SelectedSynsetViewModel?.IsBound ?? false;

        private void SynsetsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedSynsetViewModel = (SynsetViewModel)SynsetsListView.SelectedItem;
        }

        private void SynsetBoundCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SynsetsListView.SelectedItem = ((CheckBox)sender).DataContext;
            ViewModel.OnSynsetBindingChanged();
        }

        private void SynsetBoundCheckBox_Unchecked(object sender, RoutedEventArgs e) => ViewModel.OnSynsetBindingChanged();

        private BitmapImage ExpandImage = new BitmapImage(new Uri("../Resources/Images/Chevron_Down.png", UriKind.Relative));
        private BitmapImage CollapseImage = new BitmapImage(new Uri("../Resources/Images/Chevron_Up.png", UriKind.Relative));

        private void ExpandCollapseButton_Checked(object sender, RoutedEventArgs e) => ExpandCollapseImage.Source = ExpandImage;
        private void ExpandCollapseButton_Unchecked(object sender, RoutedEventArgs e) => ExpandCollapseImage.Source = CollapseImage;

    }
}
