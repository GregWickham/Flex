namespace Flex.Database
{
    internal interface IVerbPhrase : IPhrase
    {
        bool? AggregateAuxiliary { get; }
        byte? Form { get; }
        string Modal { get; }
        bool? Negated { get; }
        bool? Passive { get; }
        bool? Perfect { get; }
        byte? Person { get; }
        bool? Progressive { get; }
        bool? SuppressGenitiveInGerund { get; }
        bool? SuppressedComplementizer { get; }
        byte? Tense { get; }
    }
}
