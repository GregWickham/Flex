using FlexibleRealization;

namespace Flex.Database.UserInterface.ViewModels
{
    public class ParentViewModel
    {
        public string ParentType => FlexData.Parent.TypeDescriptionFor(Model);

        public string DefaultRealization { get; internal set; }

        public ParentElementBuilder Model { get; set; }
    }
}
