namespace Flex.Database
{
    internal interface IClause : IPhrase
    {
        bool? AggregateAuxiliary { get; }
        string Complementizer { get; }
        int? Form { get; }
        byte? InterrogativeType { get; }
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
