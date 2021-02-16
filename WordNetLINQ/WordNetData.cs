using System;
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

        public static WordNetDataContext Context { get; } = new WordNetDataContext(ConnectionString);

        public enum PartOfSpeech
        {
            Noun,
            Verb,
            Adjective,
            Adverb
        }

        public static PartOfSpeech? PartOfSpeechMatching(WordElementBuilder wordBuilder) => wordBuilder switch
        {
            NounBuilder nounBuilder => PartOfSpeech.Noun,
            VerbBuilder verbBuilder => PartOfSpeech.Verb,
            AdjectiveBuilder adjectiveBuilder => PartOfSpeech.Adjective,
            AdverbBuilder adverbBuilder => PartOfSpeech.Adverb,

            _ => null
        };
    }
}