using System.Collections.Generic;
using System.Linq;
using FlexibleRealization;
using Flex.ElementSelectors;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    public class AdjectiveSelectorViewModel : WordSelectorViewModel
    {
        internal AdjectiveSelectorViewModel(AdjectiveBuilder builder) : base(builder) { }

        internal override async void GetSynonyms()
        {
            AddPotential(await Adjective.SynonymsFor(Selector.Default));
        }
    }
}
