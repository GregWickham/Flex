using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlexibleRealization;
using Flex.ElementSelectors;
using Datamuse.ViewModels;

namespace Flex.UserInterface.ViewModels
{
    public class WordBuilderViewModel : IWordEditorViewModel, INotifyPropertyChanged
    {
        public WordBuilderViewModel(WordElementBuilder builder)
        {
            Builder = builder;
            Choices = WordChoicesFor(Builder);
        }

        private void FlexDataContext_WordChanged(int wordID)
        {
            throw new NotImplementedException();
        }

        /// <summary>The domain model for this view model.</summary>
        public WordElementBuilder Builder;

        public string DefaultWord => Builder.WordSource.DefaultWord;

        private WordChoices Choices;

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's actual list of Alternates</summary>
        public IEnumerable<string> Potential => Choices.Available
            .Where(availableChoice => Actual == null || !Actual.Any(WeightedWord => WeightedWord.Text.Equals(availableChoice)));

        /// <summary>The collection of DB_WeightedWords that have been selected as Alternates for DB_Word</summary>
        public IEnumerable<IWeightedWord> Actual => Selector?.Alternates;

        public WordSelector Selector => Builder.WordSource is WordSelector selector ? selector : null;

        /// <summary>Return a WordChoices view model of the appropriate type to edit <paramref name="dbWordElement"/>.</summary>
        private static WordChoices WordChoicesFor(WordElementBuilder builder) => builder switch
        {
            NounBuilder nb => new NounChoices(),
            VerbBuilder vb => new VerbChoices(),
            AdjectiveBuilder adjb => new AdjectiveChoices(),
            AdverbBuilder advb => new AdverbChoices(),
            _ => null,
        };

        /// <summary>Populate the Potential list.</summary>
        public async void PopulatePotential()
        {
            await Choices.GetSynonymsFor(DefaultWord);
            OnPropertyChanged("Potential");
        }

        /// <summary>Add <paramref name="wordsToMove"/> to Selector's list of Alternates.</summary>
        /// <remarks>If the Builder isn't already configured with a WordSelector as its WordSource, configure it with one so we can add Alternates to it.</remarks>
        void IWordEditorViewModel.MoveFromPotentialToActual(IList wordsToMove)
        {                
            if (Builder.WordSource is SingleWordSource single)
            {
                Builder.WordSource = new WordSelector(single.GetWord());
            }
            Selector.AddAlternates(wordsToMove.Cast<string>());
            OnPropertyChanged("Potential");
            OnPropertyChanged("Actual");
        }

        /// <summary>Remove <paramref name="wordsToMove"/> from the Selector's list of Alternates.</summary>
        void IWordEditorViewModel.MoveFromActualToPotential(IList wordsToMove)
        {
            Selector.RemoveAlternates(wordsToMove.Cast<WeightedWord>());
            OnPropertyChanged("Potential");
            OnPropertyChanged("Actual");
        }

        void IWordEditorViewModel.SetPivot(string newPivot) { }

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
