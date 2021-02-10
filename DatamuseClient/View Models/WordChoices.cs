using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public abstract class WordChoices : INotifyPropertyChanged
    {
        
        /// <summary>The word currently being used in Datamuse queries to select Potential alternates</summary>
        private protected string PivotWord { get; set; }

        internal void SetPivot(string word)
        {
            PivotWord = word;
            GetSynonymsFor(PivotWord);
        }

        public abstract Task GetSynonymsFor(string word);


        private List<Word> LookedUp = new List<Word>();

        internal void SetLookedUp(IEnumerable<Datamuse.Word> words)
        {
            LookedUp.Clear();
            LookedUp.AddRange(words);
        }

        /// <summary>A list of options presented to the user.  The user can select from this list to configure the Selector's actual list of Alternates</summary>
        public IEnumerable<string> Available => LookedUp
            .Where(datamuseWord => !Selected.Any(selectedWord => selectedWord.Equals(datamuseWord.Text)))
            .OrderByDescending(datamuseWord => datamuseWord)
            .Select(datamuseWord => datamuseWord.Text);

        private IEnumerable<Word> Selected = new List<Word>();

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
