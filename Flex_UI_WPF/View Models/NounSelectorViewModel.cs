using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    public class NounSelectorViewModel : WordSelectorViewModel
    {
        internal NounSelectorViewModel(NounBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string noun)
        {
            SetPotential(await Noun.SynonymsFor(noun));
        }
    }
}
