using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    internal class AdjectiveSelectorViewModel : WordSelectorViewModel
    {
        internal AdjectiveSelectorViewModel(AdjectiveBuilder builder) : base(builder) { }

        internal override async void GetSynonyms()
        {
            AddPotential(await Adjective.SynonymsFor(Selector.Default));
        }
    }
}
