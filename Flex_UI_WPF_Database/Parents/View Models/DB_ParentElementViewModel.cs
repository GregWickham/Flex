namespace Flex.Database.UserInterface.ViewModels
{
    public class DB_ParentElementViewModel
    {
        internal DB_ParentElementViewModel(DB_Parent parent) => DB_Parent = parent;

        public byte ParentType => (byte)DB_Parent.ParentType;

        public string DefaultRealization => DB_Parent.DefaultRealization;

        public int Forms => DB_Parent.FormsCount;

        internal DB_Parent DB_Parent { get; set; }
    }
}
