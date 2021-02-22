using System.Collections.Generic;
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

        /// <summary>The WordNetDataContext singleton used for all Linq-to-SQL operations on the WordNet database.</summary>
        public static WordNetDataContext Context { get; } = new WordNetDataContext(ConnectionString);

        /// <summary>The parts of speech recognized by WordNet.</summary>
        public enum PartOfSpeech
        {
            Unspecified,
            Noun,
            Verb,
            Adjective,
            Adverb
        }

        /// <summary>Maps WordNet part-of-speech codes to values of the <see cref="PartOfSpeech"/> enum</summary>
        private static readonly Dictionary<char, PartOfSpeech> PartOfSpeechMappings = new Dictionary<char, PartOfSpeech>
        {
            { 'n', PartOfSpeech.Noun },
            { 'v', PartOfSpeech.Verb },
            { 'a', PartOfSpeech.Adjective },
            { 's', PartOfSpeech.Adjective },
            { 'r', PartOfSpeech.Adverb },
        };

        internal static PartOfSpeech PartOfSpeechCorrespondingTo(WordElementBuilder wordBuilder) => wordBuilder switch
        {
            NounBuilder nounBuilder => PartOfSpeech.Noun,
            VerbBuilder verbBuilder => PartOfSpeech.Verb,
            AdjectiveBuilder adjectiveBuilder => PartOfSpeech.Adjective,
            AdverbBuilder adverbBuilder => PartOfSpeech.Adverb,

            _ => PartOfSpeech.Unspecified
        };

        /// <summary>Return the <see cref="PartOfSpeech"/> value corresponding to <paramref name="wordNetPartOfSpeechCode"/>.</summary>
        private static PartOfSpeech PartOfSpeechCorrespondingTo(char wordNetPartOfSpeechCode)
        {
            PartOfSpeech result;
            if (!PartOfSpeechMappings.TryGetValue(wordNetPartOfSpeechCode, out result)) result = PartOfSpeech.Unspecified;
            return result;
        }

        /// <summary>Return true if <paramref name="wordNetPartOfSpeechCode"/> represents PartOfSpeech.Noun</summary>
        internal static bool IsNoun(char wordNetPartOfSpeechCode) => PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode).Equals(PartOfSpeech.Noun);
        /// <summary>Return true if <paramref name="wordNetPartOfSpeechCode"/> represents PartOfSpeech.Verb</summary>
        internal static bool IsVerb(char wordNetPartOfSpeechCode) => PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode).Equals(PartOfSpeech.Verb);
        /// <summary>Return true if <paramref name="wordNetPartOfSpeechCode"/> represents PartOfSpeech.Adjective</summary>
        internal static bool IsAdjective(char wordNetPartOfSpeechCode) => PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode).Equals(PartOfSpeech.Adjective);
        /// <summary>Return true if <paramref name="wordNetPartOfSpeechCode"/> represents PartOfSpeech.Adverb</summary>
        internal static bool IsAdverb(char wordNetPartOfSpeechCode) => PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode).Equals(PartOfSpeech.Adverb);

        /// <summary>Return a string that describes the part of speech represented by <paramref name="wordNetPartOfSpeechCode"/> in a well-known form, like you might see in a dictionary.</summary>
        internal static string UserFriendlyPartOfSpeechFrom(char wordNetPartOfSpeechCode) => PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode) switch
        {
            PartOfSpeech.Noun => "n",
            PartOfSpeech.Verb => "v",
            PartOfSpeech.Adjective => "adj",
            PartOfSpeech.Adverb => "adv",
            _ => "Unspecified",
        };

        internal static bool PartsOfSpeechMatch(WordElementBuilder wordBuilder, PartOfSpeech pos) => PartOfSpeechCorrespondingTo(wordBuilder).Equals(pos);
        internal static bool PartsOfSpeechMatch(PartOfSpeech partOfSpeech, char wordNetPartOfSpeechCode) => partOfSpeech.Equals(PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode));
        internal static bool PartsOfSpeechMatch(WordElementBuilder wordBuilder, char wordNetPartOfSpeechCode) => PartOfSpeechCorrespondingTo(wordBuilder).Equals(PartOfSpeechCorrespondingTo(wordNetPartOfSpeechCode));
    }
}