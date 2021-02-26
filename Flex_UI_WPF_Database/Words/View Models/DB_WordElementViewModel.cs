using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FlexibleRealization;
using Flex.UserInterface.ViewModels;
using Datamuse;
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
            Pivot = DB_Word.DefaultForm;
        }

        public string DefaultWord => DefaultWeightedWord.Text;

        public bool HasAlternates => actual.Count > 0;

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's actual list of Alternates</summary>
        public IEnumerable<string> PotentialAlternates => Choices.Available
            .Where(availableChoice => !ActualAlternates.Any(dbWeightedWord => dbWeightedWord.Text.Equals(availableChoice)));

        /// <summary>The property that exposes actual to implement IWordEditorViewModel.</summary>
        public IEnumerable<IWeightedWord> ActualAlternates => actual;

        public IEnumerable<IWeightedWord> AllChoices
        {
            get
            {
                List<IWeightedWord> result = new List<IWeightedWord>();
                result.Add(DefaultWeightedWord);
                result.AddRange(ActualAlternates);
                return result;
            }
        }

        public string Pivot { get; private set; }

        public WordRelation RelationToPivot { get; private set; } = WordRelation.Synonym;

        /// <summary>The domain model for this view model.</summary>
        internal DB_Word DB_Word;

        private IEnumerable<DB_WeightedWord> WeightedWords;

        internal byte WordType => (byte)DB_Word.WordType;


        private DB_WeightedWord DefaultWeightedWord;

        private WordChoices Choices;

        /// <summary>A list of DB_WeightedWords that have been selected as Alternates for DB_Word</summary>
        private List<DB_WeightedWord> actual = new List<DB_WeightedWord>();

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
            await Choices.GetRelatedWords(DefaultWord, RelationToPivot);
            OnPropertyChanged("PotentialAlternates");
        }

        /// <summary>Add <paramref name="wordsToMove"/> to the Actual list.</summary>
        void IWordEditorViewModel.MoveFromPotentialToActual(IList wordsToMove)
        {
            actual.AddRange(wordsToMove.Cast<string>().Select(word => new DB_WeightedWord { Text = word, Weight = ElementSelectors.WeightedWord.DefaultWeight }));
            OnPropertyChanged("PotentialAlternates");
            OnPropertyChanged("ActualAlternates");
            OnPropertyChanged("AllChoices");
            OnPropertyChanged("HasAlternates");
        }

        /// <summary>Remove <paramref name="wordsToMove"/> from the Selector's list of Alternates.</summary>
        void IWordEditorViewModel.MoveFromActualToPotential(IList dbWeightedWordsToMove)
        {
            foreach (DB_WeightedWord eachWeightedWord in dbWeightedWordsToMove) actual.Remove(eachWeightedWord);
            OnPropertyChanged("PotentialAlternates");
            OnPropertyChanged("ActualAlternates");
            OnPropertyChanged("AllChoices");
            OnPropertyChanged("HasAlternates");
        }

        async void IWordEditorViewModel.SetPivot(string newPivot)
        {
            Pivot = newPivot;
            OnPropertyChanged("Pivot");
            await Choices.GetRelatedWords(Pivot, RelationToPivot);
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
