using System.Data.Linq.Mapping;

namespace WordNet.Linq
{
    // The Table attribute fixes a weird problem in Linq-to-SQL with generated classes that get their contents from a SQL Server function
    [Table]
    public partial class TeleologyForResult
    {
        public string SourcePartOfSpeechAndWordText => $"({UserFriendlySourcePartOfSpeech}.) {SourceWordText}";

        public string TelosPartOfSpeechAndWordText => $"({UserFriendlyTelosPartOfSpeech}.) {TelosWordText}";

        private string UserFriendlySourcePartOfSpeech => WordNetData.UserFriendlyPartOfSpeechFrom(SourcePOS);

        private string UserFriendlyTelosPartOfSpeech => WordNetData.UserFriendlyPartOfSpeechFrom(TelosPOS);
    }
}
