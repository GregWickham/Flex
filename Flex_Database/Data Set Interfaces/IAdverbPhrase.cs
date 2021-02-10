namespace Flex.Database
{
    internal interface IAdverbPhrase : IPhrase
    {
        bool? Comparative { get; }
        bool? Superlative { get; }
    }
}
