using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SimpleNLG;
using FlexibleRealization;

namespace Flex.Database
{
    public static partial class FlexData
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

        internal enum ElementType
        {
            None = 0,
            DB_Element = 1,
            DB_WordElement = 2,
            DB_ParentElement = 3
        }

        internal static class Word
        {
            internal static byte TypeOf(WordElementBuilder word) => word switch
            {

                WhAdverbBuilder => 1,
                WhDeterminerBuilder => 2,

                AdjectiveBuilder => 17,
                AdverbBuilder => 18,
                ConjunctionBuilder => 19,
                DeterminerBuilder => 20,
                ModalBuilder => 21,
                NounBuilder => 22,
                ParticleBuilder => 23,
                PrepositionBuilder => 24,
                PronounBuilder => 25,
                VerbBuilder => 26,

                _ => throw new InvalidOperationException("Unrecognized WordElementBuilder type")
            };

            internal static WordElementBuilder ElementOfType(byte wordType) => wordType switch
            {
                1 => new WhAdverbBuilder(),
                2 => new WhDeterminerBuilder(),

                17 => new AdjectiveBuilder(),
                18 => new AdverbBuilder(),
                19 => new ConjunctionBuilder(),
                20 => new DeterminerBuilder(),
                21 => new ModalBuilder(),
                22 => new NounBuilder(),
                23 => new ParticleBuilder(),
                24 => new PrepositionBuilder(),
                25 => new PronounBuilder(),
                26 => new VerbBuilder(),

                _ => throw new InvalidOperationException("Unrecognized WordElementBuilder type")
            };

            internal static Dictionary<int, WordElementBuilder> Cache = new Dictionary<int, WordElementBuilder>();
        }

        internal static class Parent
        {
            internal static byte TypeOf(ParentElementBuilder parent) => parent switch
            {
                // Clauses
                IndependentClauseBuilder => 1,
                SubordinateClauseBuilder => 2,

                // Phrases
                NounPhraseBuilder => 17,
                VerbPhraseBuilder => 18,
                AdjectivePhraseBuilder => 19,
                AdverbPhraseBuilder => 20,
                PrepositionalPhraseBuilder => 21,

                _ => throw new InvalidOperationException("Unrecognized ParentElementBuilder type")
            };

            internal static ParentElementBuilder ElementOfType(byte parentType) => parentType switch
            {
                1 => new IndependentClauseBuilder(),
                2 => new SubordinateClauseBuilder(),

                17 => new NounPhraseBuilder(),
                18 => new VerbPhraseBuilder(),
                19 => new AdjectivePhraseBuilder(),
                20 => new AdverbPhraseBuilder(),
                21 => new PrepositionalPhraseBuilder(),

                _ => throw new InvalidOperationException("Unrecognized ParentElementBuilder type")
            };

            internal static string TypeDescriptionFor(ParentElementBuilder parent) => parent switch
            {
                // Clauses
                IndependentClauseBuilder => "Independent Clause",
                SubordinateClauseBuilder => "Subordinate Clause",

                // Phrases
                NounPhraseBuilder => "Noun Phrase",
                VerbPhraseBuilder => "Verb Phrase",
                AdjectivePhraseBuilder => "Adjective Phrase",
                AdverbPhraseBuilder => "Adverb Phrase",
                PrepositionalPhraseBuilder => "Prepositional Phrase",

                _ => throw new InvalidOperationException("No type description for this ParentElementBuilder")
            };
        }

    }
}