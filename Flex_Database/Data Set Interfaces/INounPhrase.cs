namespace Flex.Database
{
    internal interface INounPhrase : IPhrase
    {
        bool? AdjectiveOrdering { get; }
        bool? Elided { get; }
        byte? Number { get; }
        byte? Gender { get; }
        byte? Person { get; }
        bool? Possessive { get; }
        bool? Pronominal { get; }
    }
}
