using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    internal class VerbSelectorViewModel : WordSelectorViewModel
    {
        internal VerbSelectorViewModel(VerbBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string verb)
        {
            SetPotential(await Verb.SynonymsFor(verb));
        }
    }
}
