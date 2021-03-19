using System.Data.Linq.Mapping;

namespace WordNet.Linq
{
    // The Table attribute fixes a weird problem in Linq-to-SQL with generated classes that get their contents from a SQL Server function
    [Table]
    public partial class HolonymsOfResult
    {
        public string GlossAndRelationType => $"{GlossWithoutExamples}   ({UserFriendlyRelationType})";

        public string GlossWithoutExamples
        {
            get
            {
                int indexOfFirstDoubleQuote = Gloss.IndexOf("\"");
                return indexOfFirstDoubleQuote > 0
                    ? Gloss
                        .Substring(0, indexOfFirstDoubleQuote)
                        .TrimEnd()
                        .TrimEnd(';')
                    : Gloss;
            }
        }

        public string UserFriendlyRelationType => RelationType switch
        {
            'p' => "Part",
            's' => "Substance",
            'm' => "Member"
        };
    }
}
