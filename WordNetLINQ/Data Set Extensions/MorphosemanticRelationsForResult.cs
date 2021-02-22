using System.Data.Linq.Mapping;

namespace WordNet.Linq
{
    // The Table attribute fixes a weird problem in Linq-to-SQL with generated classes that get their contents from a SQL Server function
    [Table]
    public partial class MorphosemanticRelationsForResult
    {
        public string SourcePartOfSpeechAndWordText => $"({UserFriendlySourcePartOfSpeech}.) {SourceWordText}";

        public string TargetPartOfSpeechAndWordText => $"({UserFriendlyTargetPartOfSpeech}.) {TargetWordText}";

        //private bool SourceIsNoun => WordNetData.IsNoun(SourcePOS);
        //private bool SourceIsVerb => WordNetData.IsVerb(SourcePOS);
        //private bool SourceIsAdjective => WordNetData.IsAdjective(SourcePOS);
        //private bool SourceIsAdverb => WordNetData.IsAdverb(SourcePOS);

        private string UserFriendlySourcePartOfSpeech => WordNetData.UserFriendlyPartOfSpeechFrom(SourcePOS);

        //private bool TargetIsNoun => WordNetData.IsNoun(TargetPOS);
        //private bool TargetIsVerb => WordNetData.IsVerb(TargetPOS);
        //private bool TargetIsAdjective => WordNetData.IsAdjective(TargetPOS);
        //private bool TargetIsAdverb => WordNetData.IsAdverb(TargetPOS);

        private string UserFriendlyTargetPartOfSpeech => WordNetData.UserFriendlyPartOfSpeechFrom(TargetPOS);
    }
}
