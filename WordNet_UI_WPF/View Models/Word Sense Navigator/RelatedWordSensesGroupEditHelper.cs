using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    public class RelatedWordSensesGroupEditHelper : INotifyPropertyChanged
    {
        public Brush RelatedWordSensesBorderBrush { get; private set; } = NormalBrush;
        public Thickness RelatedWordSensesBorderThickness { get; private set; } = NormalThickness;


        private bool dropIsEnabled = false;
        public bool DropIsEnabled
        {
            get => dropIsEnabled;
            set
            {
                dropIsEnabled = value;
                OnPropertyChanged("DropIsEnabled");
                RelatedWordSensesBorderBrush = value ? DropIsEnabledBrush : NormalBrush;
                OnPropertyChanged("RelatedWordSensesBorderBrush");
                RelatedWordSensesBorderThickness = value ? DropIsEnabledThickness : NormalThickness;
                OnPropertyChanged("RelatedWordSensesBorderThickness");
            }
        }

        private static readonly Brush NormalBrush = new SolidColorBrush(Colors.LightGray);
        private static readonly Brush DropIsEnabledBrush = new SolidColorBrush(Colors.ForestGreen);

        private static readonly Thickness NormalThickness = new Thickness(1);
        private static readonly Thickness DropIsEnabledThickness = new Thickness(2);


        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
