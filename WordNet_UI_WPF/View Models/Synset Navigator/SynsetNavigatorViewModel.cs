using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    internal class SynsetNavigatorViewModel : INotifyPropertyChanged
    {
        internal SynsetNavigatorViewModel()
        {
            Edit = new SynsetNavigatorEditHelper(this);
        }

        internal event SynsetSelected_EventHandler SynsetSelected;

        private Synset currentSynset;
        public Synset CurrentSynset
        {
            get => currentSynset;
            private set
            {
                // This assignment could be coming from inside or outside this control, so don't set off a recursion
                if (value != null && value != currentSynset)
                {
                    currentSynset = value;
                    SynsetSelected?.Invoke(value);
                    OnPropertyChanged("CurrentSynset");
                    OnPropertyChanged("LeftColumnWidth");
                    OnPropertyChanged("NounSpecificRelationsVisibility");
                    OnPropertyChanged("VerbSpecificRelationsVisibility");
                    OnPropertyChanged("NounOrVerbSpecificRelationsVisibility");
                    OnPropertyChanged("NounOrVerbOrAdjectiveSpecificRelationsVisibility");
                    OnPropertyChanged("AdjectiveSpecificRelationsVisibility");
                    OnPropertyChanged("AdjectiveHeadSpecificRelationsVisibility");
                    OnPropertyChanged("AdjectiveSatelliteSpecificRelationsVisibility");
                }
            }
        }

        internal void SetCurrentSynset(Synset synset)
        {
            RedoStack.Clear();
            UndoStack.Push(synset);
            OnPropertyChanged("UndoIsAvailable");
            OnPropertyChanged("RedoIsAvailable");
            CurrentSynset = synset;
        }

        public bool UndoIsAvailable => UndoStack.Count > 1;
        internal void Undo()
        {
            if (UndoStack.Count > 1)
            {
                RedoStack.Push(CurrentSynset);
                UndoStack.Pop();
                CurrentSynset = UndoStack.Peek();
                OnPropertyChanged("UndoIsAvailable");
                OnPropertyChanged("RedoIsAvailable");
            }
        }
        internal Stack<Synset> UndoStack = new Stack<Synset>();

        public bool RedoIsAvailable => RedoStack.Count > 0;
        internal void Redo()
        {
            if (RedoStack.Count > 0)
            {
                UndoStack.Push(CurrentSynset);
                CurrentSynset = RedoStack.Pop();
                OnPropertyChanged("UndoIsAvailable");
                OnPropertyChanged("RedoIsAvailable");
            }
        }
        internal Stack<Synset> RedoStack = new Stack<Synset>();

        public GridLength LeftColumnWidth => CurrentSynset != null && (CurrentSynset.IsNoun || CurrentSynset.IsVerb || CurrentSynset.IsAdjective) ? StarWidth : AutoWidth;
        private static readonly GridLength StarWidth = new GridLength(1, GridUnitType.Star);
        private static readonly GridLength AutoWidth = new GridLength(1, GridUnitType.Auto);

        public Visibility NounSpecificRelationsVisibility => CurrentSynset != null && CurrentSynset.IsNoun ? Visibility.Visible : Visibility.Collapsed;
        public Visibility VerbSpecificRelationsVisibility => CurrentSynset != null && CurrentSynset.IsVerb ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NounOrVerbSpecificRelationsVisibility => CurrentSynset != null && (CurrentSynset.IsNoun || CurrentSynset.IsVerb) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NounOrVerbOrAdjectiveSpecificRelationsVisibility => CurrentSynset != null && (CurrentSynset.IsNoun || CurrentSynset.IsVerb || CurrentSynset.IsAdjective) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdjectiveSpecificRelationsVisibility => CurrentSynset != null && CurrentSynset.IsAdjective ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdjectiveHeadSpecificRelationsVisibility => CurrentSynset != null && CurrentSynset.IsAdjectiveHead ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdjectiveSatelliteSpecificRelationsVisibility => CurrentSynset != null && CurrentSynset.IsAdjectiveSatellite ? Visibility.Visible : Visibility.Collapsed;

        public SynsetNavigatorEditHelper Edit { get; }

        public Brush CurrentSynsetGlossBorderBrush { get; private set; } = HighlightBrush;

        internal void SetDropTargets_ForWordSense(WordSense draggedWordSense) => WordSenseDropIsEnabled = draggedWordSense.LinkedSynset != CurrentSynset;
        internal void ClearDropTargets() => WordSenseDropIsEnabled = false;

        private bool wordSenseDropIsEnabled = false;
        public bool WordSenseDropIsEnabled
        {
            get => wordSenseDropIsEnabled;
            set
            {
                wordSenseDropIsEnabled = value;
                OnPropertyChanged("WordSenseDropIsEnabled");
                CurrentSynsetGlossBorderBrush = value ? WordSenseDropIsEnabledBrush : HighlightBrush;
                OnPropertyChanged("CurrentSynsetGlossBorderBrush");
            }
        }

        private static readonly Brush HighlightBrush = new SolidColorBrush(Colors.LightBlue);
        private static readonly Brush WordSenseDropIsEnabledBrush = new SolidColorBrush(Colors.ForestGreen);

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
