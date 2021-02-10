using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlexibleRealization;

namespace WordNet.Linq.ViewModels
{ 
    internal class SynsetsViewModel : INotifyPropertyChanged
    {
        private WordElementBuilder wordBuilder;
        internal WordElementBuilder WordBuilder
        {
            get => wordBuilder;
            set
            {
                wordBuilder = value;
                OnPropertyChanged("VisibleSynsets");
            }
        }

        public IEnumerable<SynsetViewModel> VisibleSynsets => wordBuilder != null 
            ? Synsets.MatchingWord(wordBuilder.WordSource.DefaultWord)
                .Where(synset => synset.POS.Equals(WordNetData.PartOfSpeechFor(wordBuilder)))
                .Select(synset => new SynsetViewModel(synset))
            : null;

        private SynsetViewModel selectedSynsetViewModel;
        public SynsetViewModel SelectedSynsetViewModel
        {
            get => selectedSynsetViewModel;
            set
            {
                selectedSynsetViewModel = value;
                OnPropertyChanged("SelectedSynsetIsBound");
                OnPropertyChanged("SelectedSynsetGlossExamples");
            }
        }

        private Synset SelectedSynset => selectedSynsetViewModel?.Model;

        public bool SelectedSynsetIsBound
        {
            get => selectedSynsetViewModel?.IsBound ?? false;
            set
            {
                if (selectedSynsetViewModel != null)
                {
                    selectedSynsetViewModel.IsBound = value;
                    OnPropertyChanged("SelectedSynsetIsBound");
                }
            }
        }

        internal void OnSynsetBindingChanged() => OnPropertyChanged("SelectedSynsetIsBound");

        public IEnumerable<string> SelectedSynsetGlossExamples => selectedSynsetViewModel?.GlossExamples;


        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
