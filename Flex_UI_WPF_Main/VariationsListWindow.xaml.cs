using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;


namespace Flex.UserInterface
{
    /// <summary>Interaction logic for VariationsListWindow.xaml</summary>
    public partial class VariationsListWindow : Window, INotifyPropertyChanged
    {
        public VariationsListWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // Don't allow the window to close.
            e.Cancel = true;
            Hide();
        }

        public string WindowTitle => $"Variations of \"{DefaultForm}\"";

        private string defaultForm;
        internal string DefaultForm 
        { 
            get => defaultForm;
            set 
            { 
                defaultForm = value; 
                OnPropertyChanged("WindowTitle"); 
            }
        }

        public ObservableCollection<string> Variations { get; } = new ObservableCollection<string>();

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged

    }
}
