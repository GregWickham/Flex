namespace Flex.Database
{
    public partial class DB_Word
    {
        public bool SupportsVariations => FlexData.Word.SupportsVariations((FlexData.WordType)WordType);
    }
}
