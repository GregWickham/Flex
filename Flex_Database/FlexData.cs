using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SimpleNLG;

namespace Flex.Database
{
    public static class FlexData
    {
        private static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = $"{Properties.Settings.Default.FlexDB_ServerHost},{Properties.Settings.Default.FlexDB_ServerPort}",
                    InitialCatalog = "Flex",
                    PersistSecurityInfo = true,
                    UserID = "Flex",
                    Password = "d^%fVdYr1BCVFkSpk0vuZs%i"
                };
                return builder.ToString();
            }
        }

        public static FlexDataContext Context { get; } = new FlexDataContext(ConnectionString);

        //internal static class LexicalCategory
        //{
        //    internal static int Mapping(lexicalCategory category) => Mappings[category];

        //    internal static lexicalCategory FromMapping(int mapping) => Mappings.FirstOrDefault(kvp => kvp.Value == mapping).Key;

        //    private static readonly Dictionary<lexicalCategory, int> Mappings = new Dictionary<lexicalCategory, int>
        //    {
        //        { lexicalCategory.ANY, 1 },
        //        { lexicalCategory.SYMBOL, 2 },
        //        { lexicalCategory.NOUN, 3 },
        //        { lexicalCategory.ADJECTIVE, 4 },
        //        { lexicalCategory.ADVERB, 5 },
        //        { lexicalCategory.VERB, 6 },
        //        { lexicalCategory.DETERMINER, 7 },
        //        { lexicalCategory.PRONOUN, 8 },
        //        { lexicalCategory.CONJUNCTION, 9 },
        //        { lexicalCategory.PREPOSITION, 10 },
        //        { lexicalCategory.COMPLEMENTISER, 11 },
        //        { lexicalCategory.MODAL, 12 },
        //        { lexicalCategory.AUXILIARY, 13 }
        //    };
        //}
    }
}