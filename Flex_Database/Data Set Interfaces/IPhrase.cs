namespace Flex.Database
{
    internal interface IPhrase : IElement
    {
        byte? DiscourseFunction { get; }
        bool? Appositive { get; }
    }
}
