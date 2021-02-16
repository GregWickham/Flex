using System;
using FlexibleRealization;
using WordNet.Linq;
using Flex.Database;

namespace Flex.UserInterface.ViewModels
{
    public class SynsetToElementBindingViewModel
    {
        internal SynsetToElementBindingViewModel(Synset synset, object element)
        {
            Synset = synset;
            Element = element;
        }

        private readonly object Element;
        private readonly Synset Synset;

        public string Gloss => Synset.GlossWithoutExamples;

        public short Weight { get; set; } = 0x7FFF;

        internal bool Matches(object elementToMatch) => Element == elementToMatch;

        internal void Save()
        {
            int elementID = Element switch
            {
                IElementTreeNode node => node.FlexDB_ID,
                DB_Element dbElement => dbElement.ID,
                _ => throw new InvalidOperationException("Unknown Element type in SynsetToElementBindingViewModel")
            };
            //FlexData.Context.SaveSynsetToElementBinding(Synset.ID, elementID, Weight);
        }
    }
}
