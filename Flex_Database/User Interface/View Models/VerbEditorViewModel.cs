using FlexibleRealization;
using Datamuse;

namespace Flex.Database.UserInterface.ViewModels
{
    internal class VerbEditorViewModel : WordEditorViewModel
    {
        internal VerbEditorViewModel(VerbBuilder builder) : base(builder) { }

        private protected override async void GetSynonymsFor(string verb)
        {
            SetPotential(await Verb.SynonymsFor(verb));
        }
    }
}
