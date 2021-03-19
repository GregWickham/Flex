using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WordNet.UserInterface.ViewModels
{
    public class RelatedSynsetsGroupEditHelper : INotifyPropertyChanged
    {
        internal RelatedSynsetsGroupEditHelper(SynsetNavigatorViewModel currentSynsetSource) => CurrentSynsetSource = currentSynsetSource;

        internal SynsetNavigatorViewModel CurrentSynsetSource;

        public Brush RelatedSynsetsBorderBrush { get; private set; } = NormalBrush;
        public Thickness RelatedSynsetsBorderThickness { get; private set; } = NormalThickness;

        private bool synsetDropIsEnabled = false;
        public bool SynsetDropIsEnabled
        {
            get => synsetDropIsEnabled;
            set
            {
                synsetDropIsEnabled = value;
                OnPropertyChanged("SynsetDropIsEnabled");
                RelatedSynsetsBorderBrush = value ? SynsetDropIsEnabledBrush : NormalBrush;
                OnPropertyChanged("RelatedSynsetsBorderBrush");
                RelatedSynsetsBorderThickness = value ? SynsetDropIsEnabledThickness : NormalThickness;
                OnPropertyChanged("RelatedSynsetsBorderThickness");
            }
        }

        private static readonly Brush NormalBrush = new SolidColorBrush(Colors.LightGray);
        private static readonly Brush SynsetDropIsEnabledBrush = new SolidColorBrush(Colors.ForestGreen);

        private static readonly Thickness NormalThickness = new Thickness(1);
        private static readonly Thickness SynsetDropIsEnabledThickness = new Thickness(2);


        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
