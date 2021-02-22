using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using FlexibleRealization;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    public class AvailableSynsetsViewModel : INotifyPropertyChanged
    {
        /// <summary>Lookup synsets with a part of speech and a word sense that match <paramref name="node"/>.</summary>
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

        /// <summary>Lookup synsets the have a word sense matching ><paramref name="word"/>.</summary>
        internal void LookupSynsetsMatching(string word)
        {
            VisibleSynsets = Synsets.MatchingWord(word);
            OnPropertyChanged("VisibleSynsets");
        }

        /// <summary>Lookup synsets with part of speech <paramref name="partOfSpeech"/> that have a word sense matching<paramref name="word"/>.</summary>
        internal void LookupSynsetsMatching(string word, WordNetData.PartOfSpeech partOfSpeech)
        {
            VisibleSynsets = Synsets.MatchingWord(word)
                .Where(synset => synset.MatchesPartOfSpeech(partOfSpeech));
            OnPropertyChanged("VisibleSynsets");
        }

        /// <summary>The collection of synsets being presented to the user as options.</summary>
        public IEnumerable<Synset> VisibleSynsets { get; private set; }
                                                

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
