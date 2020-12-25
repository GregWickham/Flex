using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;


namespace Flex.Database.UserInterface
{
    /// <summary>Interaction logic for FlexDB_BrowserWindow.xaml</summary>
    public partial class FlexDB_BrowserWindow : Window, INotifyPropertyChanged
    {
        public FlexDB_BrowserWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void NewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged

    }
}
