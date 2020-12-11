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
        public static WordSelectorViewModel For(WordElementBuilder builder) => builder switch
        {
            AdjectiveBuilder adjective => new AdjectiveSelectorViewModel(adjective), 
            _ => throw new InvalidOperationException("No view model available for that type of WordElementBuilder")
        };

        public WordSelectorViewModel(WordElementBuilder builder)
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

        private protected WordSelector Selector { get; set; }

        public string DefaultWord => $"Alternates for: {Selector.Default}";

        public List<string> Potential { get; private set; } = new List<string>();

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
