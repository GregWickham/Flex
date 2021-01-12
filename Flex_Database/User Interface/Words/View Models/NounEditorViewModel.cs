using FlexibleRealization;
using Datamuse;

namespace Flex.Database.UserInterface.ViewModels
{
    public class NounEditorViewModel : WordEditorViewModel
    {
        internal NounEditorViewModel(NounBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string noun)
        {
            SetPotential(await Noun.SynonymsFor(noun));
        }
    }
}
