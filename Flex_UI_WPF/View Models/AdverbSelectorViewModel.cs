using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    public class AdverbSelectorViewModel : WordSelectorViewModel
    {
        internal AdverbSelectorViewModel(AdverbBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string adverb)
        {
            SetPotential(await Adverb.SynonymsFor(adverb));
        }
    }
}
