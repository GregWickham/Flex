using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        /// <summary>The domain model for this view model</summary>
        private protected WordSelector Selector { get; set; }

        public string DefaultWord => $"Alternates for: {Selector.Default}";

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's list of Alternates</summary>
        public List<string> Potential { get; private set; } = new List<string>();

        /// <summary>
        /// The list 
        /// </summary>
        public List<string> Actual => Selector.Alternates;

        internal bool HasAlternates => Selector.Alternates.Count() > 0;

        internal void AddPotential(IEnumerable<string> words)
        {
            Potential.AddRange(words);
            OnPropertyChanged("Potential");
        }

        internal void MoveFromPotentialToActual(IEnumerable<string> wordsToMove)
        {
            Potential = Potential
                .Where(candidate => !wordsToMove.Contains(candidate))
                .ToList();
            OnPropertyChanged("Potential");
            Selector.Alternates.AddRange(wordsToMove);
            OnPropertyChanged("Actual");
        }

        internal abstract void GetSynonyms();


        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
