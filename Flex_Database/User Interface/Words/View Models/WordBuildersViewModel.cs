using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlexibleRealization;

namespace Flex.Database.UserInterface.ViewModels
{
    internal class WordBuildersViewModel : INotifyPropertyChanged
    {
        internal WordBuildersViewModel()
        {
            LoadWords();
        }

        public IQueryable<WordElementBuilder> VisibleWords { get; private set; } 

        private async void LoadWords()
        {
            VisibleWords = await FlexData.Context.LoadAllWordsAsync();
            OnPropertyChanged("VisibleWords");
        }


        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
