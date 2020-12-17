using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlexibleRealization;

namespace Flex.ElementSelectors
{
    public class WordSelector : IWordSource
    {
        public WordSelector(string word) 
        { 
            Default = new WeightedWord(word);
            Alternates = new List<WeightedWord>();
            Current = Default;
        }

        public string GetWord() => Current.Word;

        public WeightedWord Default { get; private set; }

        public List<WeightedWord> Alternates { get; set; }

        public void AddAlternates(IEnumerable<string> wordsToAdd) => Alternates.AddRange(wordsToAdd.Select(word => new WeightedWord(word)));

        public void RemoveAlternates(IEnumerable<WeightedWord> wordsToRemove) => Alternates = Alternates
            .Where(weightedWord => !wordsToRemove.Contains(weightedWord))
            .ToList();

        private WeightedWord Current;

        IEnumerator<string> IWordSource.GetVariationsEnumerator() => new Variations.Enumerator(this);

        public class Variations : IEnumerable<string>
        {
            internal Variations(WordSelector selector) => Selector = selector;

            private WordSelector Selector;

            public IEnumerator<string> GetEnumerator() => new Enumerator(Selector);
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(Selector);


            public class Enumerator : IEnumerator<string>
            {
                internal Enumerator(WordSelector selector)
                {
                    Selector = selector;
                    AllForms.Add(Selector.Default);
                    AllForms.AddRange(Selector.Alternates);
                    Reset();
                }

                private WordSelector Selector;

                private List<WeightedWord> AllForms = new List<WeightedWord>();

                private int CurrentIndex;

                public string Current => Selector.GetWord();
                object IEnumerator.Current => Current;

                public void Dispose() { }

                public bool MoveNext()
                {
                    if (CurrentIndex == AllForms.Count - 1)
                    {
                        Selector.Current = Selector.Default;
                        return false;
                    }
                    else
                    {
                        CurrentIndex++;
                        Selector.Current = AllForms[CurrentIndex];
                        return true;
                    }
                }

                public void Reset() => CurrentIndex = -1;
            }
        }
    }
}
