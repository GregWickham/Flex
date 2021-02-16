using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using FlexibleRealization;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    public class AvailableSynsetsViewModel : INotifyPropertyChanged
    {
        internal void LookupSynsetsMatching(IElementTreeNode node)
        {
            switch (node)
            {
                case WordElementBuilder wordBuilder:
                    VisibleSynsets = Synsets.MatchingWord(wordBuilder.WordSource.DefaultWord)
                        .Where(synset => synset.MatchesPartOfSpeech(wordBuilder));
                    OnPropertyChanged("VisibleSynsets");
                    break;
            }
        }

        internal void LookupSynsetsMatching(string word)
        {
            VisibleSynsets = Synsets.MatchingWord(word);
            OnPropertyChanged("VisibleSynsets");
        }

        internal void LookupSynsetsMatching(string word, char partOfSpeechCode)
        {
            VisibleSynsets = Synsets.MatchingWord(word)
                .Where(synset => synset.POS.Equals(partOfSpeechCode));
            OnPropertyChanged("VisibleSynsets");
        }

        public IEnumerable<Synset> VisibleSynsets { get; set; }
                                                

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
