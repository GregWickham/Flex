using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using FlexibleRealization;
using Flex.ElementSelectors;

namespace Flex.UserInterface.ViewModels
{
    public abstract class WordSelectorViewModel : INotifyPropertyChanged
    {
        /// <summary>Return an instance of the appropriate view model for <paramref name="builder"/>, or null if there is no such view model</summary>
        public static WordSelectorViewModel For(WordElementBuilder builder) => builder switch
        {
            NounBuilder noun => new NounSelectorViewModel(noun),
            VerbBuilder verb => new VerbSelectorViewModel(verb),
            AdjectiveBuilder adjective => new AdjectiveSelectorViewModel(adjective), 
            AdverbBuilder adverb => new AdverbSelectorViewModel(adverb),
            _ => null
        };

        /// <summary>Check to see if <paramref name="builder"/> is already configured with a WordSelector as its WordSource.  If not, swap out the existing SingleWordSource for a WordSelector.</summary>
        private protected WordSelectorViewModel(WordElementBuilder builder)
        {
            switch (builder.WordSource)
            {
                case SingleWordSource single:
                    Selector = new WordSelector(single.GetWord());
                    builder.WordSource = Selector;
                    break;
                case WordSelector selector:
                    Selector = selector;
                    break;
                default: throw new InvalidOperationException("Unrecognized WordSource type");
            }
        }

        /// <summary>Called after this has been set as DataContext for the view</summary>
        internal void Initialize()
        {
            PivotWord = Selector.Default.Word;
            OnPropertyChanged("DefaultWord");
            GetSynonymsFor(PivotWord);
        }

        /// <summary>The domain model for this view model</summary>
        public WordSelector Selector { get; set; }

        /// <summary>The word currently being used in Datamuse queries to select Potential alternates</summary>
        private protected string PivotWord { get; set; }

        internal void SetPivot(string word)
        {
            PivotWord = word;
            GetSynonymsFor(PivotWord);
        }

        public string DefaultWord => Selector.Default.Word;

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's actual list of Alternates</summary>
        public List<string> Potential { get; private set; } = new List<string>();

        /// <summary>The list of actual alternates for the selector</summary>
        public List<WeightedWord> Actual => Selector.Alternates;

        private bool isPotentialControlExpanded = false;
        public bool IsPotentialControlExpanded
        {
            get => isPotentialControlExpanded;
            set
            {
                isPotentialControlExpanded = value;
                OnPropertyChanged("IsPotentialControlExpanded");
            }
        }

        internal void SetPotential(IEnumerable<string> words)
        {
            Potential.Clear();
            Potential.AddRange(words);
            OnPropertyChanged("Potential");
        }

        internal void MoveFromPotentialToActual(IEnumerable<string> wordsToMove)
        {
            Potential = Potential
                .Where(candidate => !wordsToMove.Contains(candidate))
                .ToList();
            OnPropertyChanged("Potential");
            Selector.AddAlternates(wordsToMove);
            OnPropertyChanged("Actual");
        }

        internal void MoveFromActualToPotential(IEnumerable<WeightedWord> wordsToMove)
        {
            Selector.RemoveAlternates(wordsToMove);
            OnPropertyChanged("Actual");
            Potential.AddRange(wordsToMove.Select(weightedWord => weightedWord.Word));
            OnPropertyChanged("Potential");
        }

        private protected abstract void GetSynonymsFor(string word);


        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
