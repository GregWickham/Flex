namespace Flex.Database
{
    internal interface IAdjectivePhrase : IPhrase
    {
        bool? Comparative { get; }
        bool? Superlative { get; }
    }
}
