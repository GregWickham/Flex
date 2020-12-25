using System.Data.SqlClient;

namespace WordNet.Linq
{
    internal static class WordNetData
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
    }
}