using System.Collections.Generic;
using System.ComponentModel;
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
        } 

        public string Default { get; private set; }

        public List<string> Alternates { get; }

        public string GetWord() => Default;
    }
}
