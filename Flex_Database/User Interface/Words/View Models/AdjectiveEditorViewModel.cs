using FlexibleRealization;
using Datamuse;

namespace Flex.Database.UserInterface.ViewModels
{
    internal class AdjectiveEditorViewModel : WordEditorViewModel
    {
        internal AdjectiveEditorViewModel(AdjectiveBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string adjective)
        {
            SetPotential(await Adjective.SynonymsFor(adjective));
        }
    }
}
