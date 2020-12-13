using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    internal class VerbSelectorViewModel : WordSelectorViewModel
    {
        internal VerbSelectorViewModel(VerbBuilder builder) : base(builder) { }

        internal override async void GetSynonyms()
        {
            AddPotential(await Verb.SynonymsFor(Selector.Default));
        }
    }
}
