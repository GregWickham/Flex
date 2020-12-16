using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Flex.ElementSelectors;
using Flex.UserInterface.ViewModels;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for WordAlternativesSelector.xaml</summary>
    public partial class WordSelectorControl : UserControl
    {
        public WordSelectorControl()
        {
            InitializeComponent();
            ExpandCollapsePotentialControlImage.Source = ExpandImage;
            WeightsOrColumnsImage.Source = WeightsImage;
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
            ViewModel.Initialize();
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
                ViewModel.MoveFromActualToPotential(ActualList.SelectedItems.Cast<WeightedWord>().ToList());
            }
        }

        private void PotentialListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Border potentialItem = (Border)PotentialList.InputHitTest(e.GetPosition(PotentialList));
            string doubleClickedString = (string)potentialItem.DataContext;
            ViewModel.SetPivot(doubleClickedString);
        }

        private BitmapImage ExpandImage = new BitmapImage(new Uri("/Resources/Images/Chevron_Up.png", UriKind.Relative));

        private BitmapImage CollapseImage = new BitmapImage(new Uri("/Resources/Images/Chevron_Down.png", UriKind.Relative));

        private void ExpandCollapsePotentialControlButton_Checked(object sender, RoutedEventArgs e)
        {
            ExpandCollapsePotentialControlImage.Source = CollapseImage;
            PotentialControlPanel.Visibility = Visibility.Visible;
        }

        private void ExpandCollapsePotentialControlButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ExpandCollapsePotentialControlImage.Source = ExpandImage;
            PotentialControlPanel.Visibility = Visibility.Collapsed;
        }

        private BitmapImage WeightsImage = new BitmapImage(new Uri("/Resources/Images/Weight.png", UriKind.Relative));

        private BitmapImage ColumnsImage = new BitmapImage(new Uri("/Resources/Images/Columns.png", UriKind.Relative));

        private void WeightsOrColumnsButton_Checked(object sender, RoutedEventArgs e)
        {
            WeightsOrColumnsImage.Source = ColumnsImage;
            Columns.Visibility = Visibility.Collapsed;
            Weights.Visibility = Visibility.Visible;
        }

        private void WeightsOrColumnsButton_Unchecked(object sender, RoutedEventArgs e)
        {
            WeightsOrColumnsImage.Source = WeightsImage;
            Weights.Visibility = Visibility.Collapsed;
            Columns.Visibility = Visibility.Visible;
        }
    }
}
