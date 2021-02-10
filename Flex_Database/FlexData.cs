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
                    MultipleActiveResultSets = true,
                    UserID = "Flex",
                    Password = "d^%fVdYr1BCVFkSpk0vuZs%i"
                };
                return builder.ToString();
            }
        }

        public static FlexDataContext Context { get; } = new FlexDataContext(ConnectionString);

        public enum ElementType : byte
        {
            None = 0,
            DB_Word = 1,
            DB_Parent = 2
        }

        public enum WordType : byte
        {
            Unspecified = 0,

            WhAdverb = 1,
            WhDeterminer = 2,

            Adjective = 17,
            Adverb = 18,
            CardinalNumber = 19,
            Conjunction = 20,
            Determiner = 21,
            Modal = 22,
            Noun = 23,
            Particle = 24,
            Preposition = 25,
            Pronoun = 26,
            Verb = 27
        }

        public static Type BuilderTypeFrom(byte elementType, byte wordOrParentType) => elementType switch
        {
            (byte)ElementType.DB_Word => Word.BuilderTypeFrom(wordOrParentType),
            (byte)ElementType.DB_Parent => Parent.BuilderTypeFrom(wordOrParentType),
            _ => throw new ArgumentException("Unknown Flex database element type")
        };

        public static class Word
        {
            private static readonly Dictionary<Type, WordType> BuilderTypeToWordType = new Dictionary<Type, WordType>
            {
                { typeof(WhAdverbBuilder), WordType.WhAdverb },
                { typeof(WhDeterminerBuilder), WordType.WhDeterminer },

                { typeof(AdjectiveBuilder), WordType.Adjective },
                { typeof(AdverbBuilder), WordType.Adverb },
                { typeof(CardinalNumberBuilder), WordType.CardinalNumber },
                { typeof(ConjunctionBuilder), WordType.Conjunction },
                { typeof(DeterminerBuilder), WordType.Determiner },
                { typeof(ModalBuilder), WordType.Modal },
                { typeof(NounBuilder), WordType.Noun },
                { typeof(ParticleBuilder), WordType.Particle },
                { typeof(PrepositionBuilder), WordType.Preposition },
                { typeof(PronounBuilder), WordType.Pronoun },
                { typeof(VerbBuilder), WordType.Verb }
            };

            public static Type BuilderTypeFrom(byte type) => BuilderTypeToWordType.Single(kvp => kvp.Value.Equals((WordType)type)).Key;

            public static WordType TypeOf(WordElementBuilder word) => BuilderTypeToWordType[word.GetType()];

            public static WordElementBuilder BuilderOfType(WordType type) => type switch
            {
                WordType.WhAdverb => new WhAdverbBuilder(),
                WordType.WhDeterminer => new WhDeterminerBuilder(),

                WordType.Adjective => new AdjectiveBuilder(),
                WordType.Adverb => new AdverbBuilder(),
                WordType.CardinalNumber => new CardinalNumberBuilder(),
                WordType.Conjunction => new ConjunctionBuilder(),
                WordType.Determiner => new DeterminerBuilder(),
                WordType.Modal => new ModalBuilder(),
                WordType.Noun => new NounBuilder(),
                WordType.Particle => new ParticleBuilder(),
                WordType.Preposition => new PrepositionBuilder(),
                WordType.Pronoun => new PronounBuilder(),
                WordType.Verb => new VerbBuilder(),

                _ => throw new InvalidOperationException("Unrecognized WordElementBuilder type")
            };

            /// <summary>Return true if wordsa of type <paramref name="wordType"/> supports variations.  The Flex database enforces uniqueness for WordElementBuilders that do NOT support variations.</summary>
            /// <remarks>Uniqeness means, for example, that the database only contains one instance of the determiner "a", regardless of how many parent elements use that instance.</remarks>
            public static bool SupportsVariations(WordType wordType) => wordType switch
            {
                WordType.Noun => true,
                WordType.Verb => true,
                WordType.Adjective => true,
                WordType.Adverb => true,
                _ => false
            };

            public static Dictionary<int, WordElementBuilder> Cache = new Dictionary<int, WordElementBuilder>();
        }
        public enum ParentType : byte
        {
            Unspecified = 0,

            IndependentClause = 1,
            SubordinateClause = 2,

            NounPhrase = 17,
            VerbPhrase = 18,
            AdjectivePhrase = 19,
            AdverbPhrase = 20,
            PrepositionalPhrase = 21,

            WhNounPhrase = 33,
            WhAdverbPhrase = 34,

            CompoundNoun = 49,

            NominalModifier = 65,
        }

        public static class Parent
        {
            private static readonly Dictionary<Type, ParentType> BuilderTypeToParentType = new Dictionary<Type, ParentType>
            {
                { typeof(IndependentClauseBuilder), ParentType.IndependentClause },
                { typeof(SubordinateClauseBuilder), ParentType.SubordinateClause },

                { typeof(NounPhraseBuilder), ParentType.NounPhrase },
                { typeof(VerbPhraseBuilder), ParentType.VerbPhrase },
                { typeof(AdjectivePhraseBuilder), ParentType.AdjectivePhrase },
                { typeof(AdverbPhraseBuilder), ParentType.AdverbPhrase },
                { typeof(PrepositionalPhraseBuilder), ParentType.PrepositionalPhrase },

                { typeof(WhNounPhraseBuilder), ParentType.WhNounPhrase },
                { typeof(WhAdverbPhraseBuilder), ParentType.WhAdverbPhrase },

                { typeof(CompoundNounBuilder), ParentType.CompoundNoun },

                { typeof(NominalModifierBuilder), ParentType.NominalModifier }
            };

            public static Type BuilderTypeFrom(byte type) => BuilderTypeToParentType.Single(kvp => kvp.Value.Equals((ParentType)type)).Key;

            public static ParentType TypeOf(ParentElementBuilder parent) => BuilderTypeToParentType[parent.GetType()];

            public static ParentElementBuilder BuilderOfType(ParentType type) => type switch
            {
                ParentType.IndependentClause => new IndependentClauseBuilder(),
                ParentType.SubordinateClause => new SubordinateClauseBuilder(),

                ParentType.NounPhrase => new NounPhraseBuilder(),
                ParentType.VerbPhrase => new VerbPhraseBuilder(),
                ParentType.AdjectivePhrase => new AdjectivePhraseBuilder(),
                ParentType.AdverbPhrase => new AdverbPhraseBuilder(),
                ParentType.PrepositionalPhrase => new PrepositionalPhraseBuilder(),

                ParentType.CompoundNoun => new CompoundNounBuilder(),

                ParentType.NominalModifier => new NominalModifierBuilder(),

                _ => throw new InvalidOperationException("Unrecognized ParentElementBuilder type")
            };

            public static Dictionary<int, ParentElementBuilder> Cache = new Dictionary<int, ParentElementBuilder>();
        }

    }
}