using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class NounChoices : WordChoices
    {
        public override sealed async Task GetRelatedWords(string noun, WordRelation relation) => SetLookedUp(relation switch
        {
            WordRelation.Synonym => await Noun.SynonymsOf(noun),
            WordRelation.MeaningLike => await Noun.MeaningLike(noun)
        });
    }
}
