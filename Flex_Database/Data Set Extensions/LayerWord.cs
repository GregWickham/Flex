namespace Flex.Database
{
    public partial class LayerWord
    {
        public LayerWord(int id, FlexData.WordType type) 
        {
            ID = id;
            WordType = (byte)type; 
        }
    }
}
