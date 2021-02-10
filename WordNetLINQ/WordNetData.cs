using System.Data.SqlClient;
using FlexibleRealization;

namespace WordNet.Linq
{
    public static class WordNetData
    {
        private static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = $"{Properties.Settings.Default.WordNet_ServerHost},{Properties.Settings.Default.WordNet_ServerPort}",
                    InitialCatalog = "wordnet",
                    PersistSecurityInfo = true,
                    UserID = "Flex",
                    Password = "d^%fVdYr1BCVFkSpk0vuZs%i"
                };
                return builder.ToString();
            }
        }

        internal static WordNetDataContext Context { get; } = new WordNetDataContext(ConnectionString);

        public static char PartOfSpeechFor(WordElementBuilder builder) => builder switch
        {
            NounBuilder nb => 'n',
            VerbBuilder vb => 'v',
            AdjectiveBuilder adjb => 'a',
            AdverbBuilder advb => 'r'
        };
    }
}