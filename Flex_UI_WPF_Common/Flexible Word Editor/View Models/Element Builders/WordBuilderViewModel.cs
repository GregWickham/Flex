using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlexibleRealization;
using Flex.ElementSelectors;
using Datamuse;
using Datamuse.ViewModels;

namespace Flex.UserInterface.ViewModels
{
    public class WordBuilderViewModel : IWordEditorViewModel, INotifyPropertyChanged
    {
        public WordBuilderViewModel(WordElementBuilder builder)
        {
            Builder = builder;
            Choices = WordChoicesFor(Builder);
            Pivot = Builder.WordSource.DefaultWord;
        }

        public string DefaultWord => Builder.WordSource.DefaultWord;

        public bool HasAlternates => Builder.WordSource is WordSelector selector && selector.HasAlternates;

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's actual list of Alternates</summary>
        public IEnumerable<string> PotentialAlternates => Choices.Available
            .Where(availableChoice => ActualAlternates == null || !ActualAlternates.Any(WeightedWord => WeightedWord.Text.Equals(availableChoice)));

        /// <summary>The collection of DB_WeightedWords that have been selected as Alternates for DB_Word</summary>
        public IEnumerable<IWeightedWord> ActualAlternates => Selector?.Alternates;

        public IEnumerable<IWeightedWord> AllChoices
        {
            get
            {
                if (Builder.WordSource is SingleWordSource singleWord)
                {
                    return new List<IWeightedWord> { new WeightedWord(singleWord.DefaultWord) };
                }
                else
                {
                    List<IWeightedWord> result = new List<IWeightedWord>();
                    result.Add(Selector.Default);
                    result.AddRange(ActualAlternates);
                    return result;
                }
            }
        }

        public string Pivot { get; private set; }

        public WordRelation RelationToPivot { get; private set; } = WordRelation.Synonym;

        /// <summary>The domain model for this view model.</summary>
        private WordElementBuilder Builder;

        private WordChoices Choices;

        private WordSelector Selector => Builder.WordSource is WordSelector selector ? selector : null;

        /// <summary>Return a WordChoices view model of the appropriate type to edit <paramref name="dbWordElement"/>.</summary>
        private static WordChoices WordChoicesFor(WordElementBuilder builder) => builder switch
        {
            NounBuilder nb => new NounChoices(),
            VerbBuilder vb => new VerbChoices(),
            AdjectiveBuilder adjb => new AdjectiveChoices(),
            AdverbBuilder advb => new AdverbChoices(),
            _ => null,
        };

        /// <summary>Populate the PotentialAlternates list.</summary>
        public async void PopulatePotential()
        {
            await Choices.GetRelatedWords(DefaultWord, RelationToPivot);
            OnPropertyChanged("PotentialAlternates");
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
            OnPropertyChanged("PotentialAlternates");
            OnPropertyChanged("ActualAlternates");
            OnPropertyChanged("AllChoices");
            OnPropertyChanged("HasAlternates");
        }

        /// <summary>Remove <paramref name="wordsToMove"/> from the Selector's list of Alternates.</summary>
        void IWordEditorViewModel.MoveFromActualToPotential(IList wordsToMove)
        {
            Selector.RemoveAlternates(wordsToMove.Cast<WeightedWord>());
            OnPropertyChanged("PotentialAlternates");
            OnPropertyChanged("ActualAlternates");
            OnPropertyChanged("AllChoices");
            OnPropertyChanged("HasAlternates");
        }

        async void IWordEditorViewModel.SetPivot(string newPivot)
        {
            Pivot = newPivot;
            OnPropertyChanged("Pivot");
            await Choices.GetRelatedWords(newPivot, RelationToPivot);
            OnPropertyChanged("PotentialAlternates");
        }

        async void IWordEditorViewModel.SetRelationToPivot(WordRelation relation)
        {
            RelationToPivot = relation;
            await Choices.GetRelatedWords(Pivot, RelationToPivot);
            OnPropertyChanged("PotentialAlternates");
        }

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
