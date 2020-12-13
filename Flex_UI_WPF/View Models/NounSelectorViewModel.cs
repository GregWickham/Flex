using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    public class NounSelectorViewModel : WordSelectorViewModel
    {
        internal NounSelectorViewModel(NounBuilder builder) : base(builder) { }

        internal override async void GetSynonyms()
        {
            AddPotential(await Noun.SynonymsFor(Selector.Default));
        }
    }
}
