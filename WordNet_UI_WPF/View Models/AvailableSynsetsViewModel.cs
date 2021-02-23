using System.Collections.Generic;
using System.ComponentModel;
using WordNet.Linq;

namespace WordNet.UserInterface.ViewModels
{
    public class AvailableSynsetsViewModel : INotifyPropertyChanged
    {
        /// <summary>Lookup synsets with a part of speech and a word sense that match <paramref name="node"/>.</summary>
        internal void LookupSynsetsMatching(WordSpecification wordSpecification)
        {
            VisibleSynsets = Synsets.MatchingWordSpecification(wordSpecification);
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
