using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FlexibleRealization;
using Flex.UserInterface.ViewModels;
using Datamuse.ViewModels;

namespace Flex.Database.UserInterface.ViewModels
{ 
    public class DB_WordElementViewModel : IWordEditorViewModel, INotifyPropertyChanged
    {
        public DB_WordElementViewModel(DB_Word dbWordElement, IEnumerable<DB_WeightedWord> weightedWords)
        {
            DB_Word = dbWordElement;
            WeightedWords = weightedWords;
            DefaultWeightedWord = WeightedWords.Single(weightedWord => DB_Word.DefaultWeightedWord.Equals(weightedWord.ID));
            actual.AddRange(WeightedWords.Where(weightedWord => weightedWord.ID != DB_Word.DefaultWeightedWord));          
            Choices = WordChoicesFor(DB_Word);
        }

        /// <summary>The domain model for this view model.</summary>
        public DB_Word DB_Word;

        private IEnumerable<DB_WeightedWord> WeightedWords;

        public byte WordType => (byte)DB_Word.WordType;

        public string DefaultWord => DefaultWeightedWord.Text;

        private DB_WeightedWord DefaultWeightedWord;

        private WordChoices Choices;

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's actual list of Alternates</summary>
        public IEnumerable<string> Potential => Choices.Available
            .Where(availableChoice => !Actual.Any(dbWeightedWord => dbWeightedWord.Text.Equals(availableChoice)));

        /// <summary>A list of DB_WeightedWords that have been selected as Alternates for DB_Word</summary>
        private List<DB_WeightedWord> actual = new List<DB_WeightedWord>();
        /// <summary>The property that exposes actual to implement IWordEditorViewModel.</summary>
        public IEnumerable<IWeightedWord> Actual => actual;

        public Task Save()
        {
            if (actual.Count == 0)
            {
                return FlexData.Context.SaveWordAsync(DB_Word, null, actual);
            }
            else
            {
                return FlexData.Context.SaveWordAsync(DB_Word, DefaultWeightedWord, actual);
            }
        }

        /// <summary>Return a WordChoices view model of the appropriate type to edit <paramref name="dbWordElement"/>.</summary>
        private static WordChoices WordChoicesFor(DB_Word dbWordElement) => dbWordElement.WordType switch
        {
            (byte)FlexData.WordType.Noun => new NounChoices(),
            (byte)FlexData.WordType.Verb => new VerbChoices(),
            (byte)FlexData.WordType.Adjective => new AdjectiveChoices(),
            (byte)FlexData.WordType.Adverb => new AdverbChoices(),
            _ => null,
        };

        /// <summary>Populate the Potential list.</summary>
        public async void PopulatePotential()
        {
            await Choices.GetSynonymsFor(DefaultWord);
            OnPropertyChanged("Potential");
        }

        /// <summary>Add <paramref name="wordsToMove"/> to the Actual list.</summary>
        void IWordEditorViewModel.MoveFromPotentialToActual(IList wordsToMove)
        {
            actual.AddRange(wordsToMove.Cast<string>().Select(word => new DB_WeightedWord { Text = word, Weight = ElementSelectors.WeightedWord.DefaultWeight }));
            OnPropertyChanged("Potential");
            OnPropertyChanged("Actual");
        }

        /// <summary>Remove <paramref name="wordsToMove"/> from the Selector's list of Alternates.</summary>
        void IWordEditorViewModel.MoveFromActualToPotential(IList dbWeightedWordsToMove)
        {
            foreach (DB_WeightedWord eachWeightedWord in dbWeightedWordsToMove) actual.Remove(eachWeightedWord);
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
