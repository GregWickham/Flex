using FlexibleRealization;
using Datamuse;

namespace Flex.Database.UserInterface.ViewModels
{
    public class AdverbEditorViewModel : WordEditorViewModel
    {
        internal AdverbEditorViewModel(AdverbBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string adverb)
        {
            SetPotential(await Adverb.SynonymsFor(adverb));
        }
    }
}
