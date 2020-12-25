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

        public string DefaultWord => Default.Word;

        public WeightedWord Default { get; private set; }

        public List<WeightedWord> Alternates { get; set; }

        public int WeightOf(string wordVariation) => GetWeightedWordVariations()
            .Where(weightedWord => weightedWord.Word.Equals(wordVariation))
            .Single()
            .Weight;

        public void AddAlternates(IEnumerable<string> wordsToAdd) => Alternates.AddRange(wordsToAdd.Select(word => new WeightedWord(word)));

        public void RemoveAlternates(IEnumerable<WeightedWord> wordsToRemove) => Alternates = Alternates
            .Where(weightedWord => !wordsToRemove.Contains(weightedWord))
            .ToList();

        private WeightedWord Current;

        public IEnumerator<WeightedWord> GetWeightedWordVariationsEnumerator() => new Variations.WeightedWordEnumerator(this);

        IEnumerator<string> IWordSource.GetStringVariationsEnumerator() => new Variations.StringEnumerator(this);

        /// <summary>Return the WeightedWord variations of this</summary>
        public IEnumerable<WeightedWord> GetWeightedWordVariations() => new WeightedWordVariations(this);

        /// <summary>Return the string variations of this</summary>
        public IEnumerable<string> GetStringVariations() => new StringVariations(this);

        /// <summary>There are two different ways that a WordSelector can enumerate its variations:
        /// <list type="number">
        /// <item>WeightedWords</item>
        /// <item>strings</item></list>
        /// Variations has a subclass to handle each of these cases.</summary>
        /// <remarks>WARNING!  Variations enumerators are stateful, so if you want the WordSelector to end up in its original state, you must complete the enumeration.</remarks>
        public class Variations
        {
            internal Variations(WordSelector selector) => Selector = selector;

            private protected WordSelector Selector;

            public class Enumerator
            {
                internal Enumerator(WordSelector selector)
                {
                    Selector = selector;
                    AllForms.Add(Selector.Default);
                    AllForms.AddRange(Selector.Alternates);
                    Reset();
                }

                private protected WordSelector Selector;

                private List<WeightedWord> AllForms = new List<WeightedWord>();

                private int CurrentIndex;

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

            public class WeightedWordEnumerator : Enumerator, IEnumerator<WeightedWord>
            {
                internal WeightedWordEnumerator(WordSelector selector) : base(selector) { }
                public WeightedWord Current => Selector.Current;
                object IEnumerator.Current => Current;
            }

            public class StringEnumerator : Enumerator, IEnumerator<string>
            {
                internal StringEnumerator(WordSelector selector) : base(selector) { }
                public string Current => Selector.Current.Word;
                object IEnumerator.Current => Current;
            }
        }

        public class WeightedWordVariations : Variations, IEnumerable<WeightedWord>
        {
            internal WeightedWordVariations(WordSelector selector) : base(selector) { }
            public IEnumerator<WeightedWord> GetEnumerator() => new WeightedWordEnumerator(Selector);
            IEnumerator IEnumerable.GetEnumerator() => new WeightedWordEnumerator(Selector);
        }

        public class StringVariations : Variations, IEnumerable<string>
        {
            internal StringVariations(WordSelector selector) : base(selector) { }
            public IEnumerator<string> GetEnumerator() => new StringEnumerator(Selector);
            IEnumerator IEnumerable.GetEnumerator() => new StringEnumerator(Selector);
        }
    }
}
