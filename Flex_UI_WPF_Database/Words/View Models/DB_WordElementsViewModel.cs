using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Flex.Database.UserInterface.ViewModels
{
    internal class DB_WordElementsViewModel : INotifyPropertyChanged
    {
        internal DB_WordElementsViewModel()
        {
            LoadAllWordsAsync();
            FlexData.Context.SaveCompleted += DataContext_SaveCompleted;
        }

        private void DataContext_SaveCompleted(FlexibleRealization.IElementTreeNode saved)
        {
            LoadedWordViewModels.AddRange(ViewModelsFor(NewWordElements));
            OnPropertyChanged("VisibleWords");
        }

        internal void Detach() => FlexData.Context.SaveCompleted -= DataContext_SaveCompleted;

        private DB_WordElementViewModel ViewModelFor(DB_Word wordElement) => new DB_WordElementViewModel(wordElement, FlexData.Context.DB_WeightedWords.Where(weightedWord => weightedWord.WordElement.Equals(wordElement.ID)));

        private IEnumerable<DB_WordElementViewModel> ViewModelsFor(IEnumerable<DB_Word> wordElements) => wordElements.Select(wordElement => ViewModelFor(wordElement));

        private IEnumerable<DB_Word> NewWordElements => FlexData.Context.DB_Words.ToList().Where(dbWord => !LoadedWordViewModels.Any(viewModel => viewModel.DB_Word == dbWord));


        private List<DB_WordElementViewModel> LoadedWordViewModels = new List<DB_WordElementViewModel>();

        public IEnumerable<DB_WordElementViewModel> VisibleWords
        {
            get
            {
                if (LoadedWordViewModels == null) return null;
                else
                {
                    IEnumerable<DB_WordElementViewModel> filteredByWordType = WordTypeFilter == FlexData.WordType.Unspecified
                        ? LoadedWordViewModels
                        : LoadedWordViewModels.Where(dbWordViewModel => dbWordViewModel.WordType.Equals((byte)WordTypeFilter));
                    return AlphabeticFilter == null
                        ? filteredByWordType
                        : filteredByWordType.Where(dbWordViewModel => AlphabeticFilter == null || dbWordViewModel.DefaultWord.StartsWith(AlphabeticFilter, StringComparison.CurrentCultureIgnoreCase));
                }
            }
        }

        private Task LoadAllWordsAsync() => Task.Run(() =>
        {
            LoadedWordViewModels.AddRange(ViewModelsFor(FlexData.Context.DB_Words));
            OnPropertyChanged("VisibleWords");
        });

        private FlexData.WordType WordTypeFilter = FlexData.WordType.Unspecified;

        private string AlphabeticFilter = null;

        /// <summary>Filter the VisibleWords to display only those of type <paramref name="filter"/>.</summary>
        internal void SetWordTypeFilter(FlexData.WordType wordType)
        {
            WordTypeFilter = wordType;                
            OnPropertyChanged("VisibleWords");
        }

        /// <summary>Filter the VisibleWords to display only those of type <paramref name="filter"/>.</summary>
        internal void SetAlphabeticFilter(string filter)
        {
            AlphabeticFilter = filter;
            OnPropertyChanged("VisibleWords");
        }

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
