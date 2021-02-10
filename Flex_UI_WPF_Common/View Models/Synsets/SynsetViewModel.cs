using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordNet.Linq.ViewModels
{
    internal class SynsetViewModel 
    {
        internal SynsetViewModel(Synset model) { Model = model; }

        public Synset Model { get; private set; }

        private static readonly Regex QuotedSubstring = new Regex("\".*?\"");

        public string GlossWithoutExamples => Model.Gloss
            .Substring(0, Model.Gloss.IndexOf("\""))
            .TrimEnd()
            .TrimEnd(';');

        public IEnumerable<string> GlossExamples => QuotedSubstring.Matches(Model.Gloss).Cast<Match>().Select(match => match.ToString());

        public bool IsBound { get; set; }

        public int Weight { get; set; } = 0x7FFF;
    }
}
