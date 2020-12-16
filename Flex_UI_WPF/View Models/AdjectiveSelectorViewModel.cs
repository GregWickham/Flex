using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    internal class AdjectiveSelectorViewModel : WordSelectorViewModel
    {
        internal AdjectiveSelectorViewModel(AdjectiveBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string adjective)
        {
            SetPotential(await Adjective.SynonymsFor(adjective));
        }
    }
}
