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
            Default = word;
            Alternates = new List<string>();
            Current = Default;
        }

        public string GetWord() => Current;

        public string Default { get; private set; }

        public List<string> Alternates { get; }

        private string Current;

        IEnumerator<string> IWordSource.EnumerateVariations() => new Variations.Enumerator(this);

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

                private List<string> AllForms = new List<string>();

                private int CurrentIndex;

                public string Current => Selector.GetWord();
                object IEnumerator.Current => Current;

                public void Dispose() { }

                public bool MoveNext()
                {
                    if (CurrentIndex == AllForms.Count - 1) return false;
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
