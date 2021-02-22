using System.Data.Linq.Mapping;

namespace WordNet.Linq
{
    // The Table attribute fixes a weird problem in Linq-to-SQL with generated classes that get their contents from a SQL Server function
    [Table]
    public partial class TeleologyForResult2
    {
        public string SourcePartOfSpeechAndWordText => $"({UserFriendlySourcePartOfSpeech}.) {SourceWordText}";

        public string TelosPartOfSpeechAndWordText => $"({UserFriendlyTelosPartOfSpeech}.) {TelosWordText}";

        //private bool SourceIsNoun => WordNetData.IsNoun(SourcePOS);
        //private bool SourceIsVerb => WordNetData.IsVerb(SourcePOS);
        //private bool SourceIsAdjective => WordNetData.IsAdjective(SourcePOS);
        //private bool SourceIsAdverb => WordNetData.IsAdverb(SourcePOS);

        private string UserFriendlySourcePartOfSpeech => WordNetData.UserFriendlyPartOfSpeechFrom(SourcePOS);

        //private bool TelosIsNoun => WordNetData.IsNoun(TelosPOS);
        //private bool TelosIsVerb => WordNetData.IsVerb(TelosPOS);
        //private bool TelosIsAdjective => WordNetData.IsAdjective(TelosPOS);
        //private bool TelosIsAdverb => WordNetData.IsAdverb(TelosPOS);

        private string UserFriendlyTelosPartOfSpeech => WordNetData.UserFriendlyPartOfSpeechFrom(TelosPOS);
    }
}
