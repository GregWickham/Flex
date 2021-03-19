using System.ComponentModel;
using System.Windows;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    internal class WordSenseNavigatorViewModel : INotifyPropertyChanged
    {
        internal WordSenseNavigatorViewModel()
        {
            Edit = new WordSenseNavigatorEditHelper(this);
        }

        private WordSense currentWordSense;
        public WordSense CurrentWordSense
        {
            get => currentWordSense;
            set
            {
                // This assignment could be coming from inside or outside this control, so don't set off a recursion
                if (value != null && value != currentWordSense)
                {
                    currentWordSense = value;
                    OnPropertyChanged("CurrentWordSense");
                    OnPropertyChanged("NounSpecificRelationsVisibility");
                    OnPropertyChanged("VerbSpecificRelationsVisibility");
                    OnPropertyChanged("AdjectiveSpecificRelationsVisibility");
                    OnPropertyChanged("NounOrAdjectiveSpecificRelationsVisibility");
                    OnPropertyChanged("AdverbSpecificRelationsVisibility");
                }
            }
        }

        public Visibility NounSpecificRelationsVisibility => CurrentWordSense != null && CurrentWordSense.IsNoun ? Visibility.Visible : Visibility.Collapsed;
        public Visibility VerbSpecificRelationsVisibility => CurrentWordSense != null && CurrentWordSense.IsVerb ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdjectiveSpecificRelationsVisibility => CurrentWordSense != null && CurrentWordSense.IsAdjective ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NounOrAdjectiveSpecificRelationsVisibility => CurrentWordSense != null && (CurrentWordSense.IsNoun || CurrentWordSense.IsAdjective) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdverbSpecificRelationsVisibility => CurrentWordSense != null && CurrentWordSense.IsAdverb ? Visibility.Visible : Visibility.Collapsed;

        public WordSenseNavigatorEditHelper Edit { get; } 

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
