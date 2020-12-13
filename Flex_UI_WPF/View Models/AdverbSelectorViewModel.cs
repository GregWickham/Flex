using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    public class AdverbSelectorViewModel : WordSelectorViewModel
    {
        internal AdverbSelectorViewModel(AdverbBuilder builder) : base(builder) { }

        internal override async void GetSynonyms()
        {
            AddPotential(await Adverb.SynonymsFor(Selector.Default));
        }
    }
}
