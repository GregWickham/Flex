using System;
using System.Collections.Generic;

namespace Datamuse
{
    public class Word : IComparable<Word>
    {
        public string word { get; set; }

        public string Text => word;

        public int Score { get; set; }

        public IList<string> Tags { get; set; }

        public int CompareTo(Word anotherWord) => Score.CompareTo(anotherWord.Score);
    }
}
