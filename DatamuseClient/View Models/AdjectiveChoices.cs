using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class AdjectiveChoices : WordChoices
    {
        public override sealed async Task GetRelatedWords(string adjective, WordRelation relation) => SetLookedUp(relation switch
        {
            WordRelation.Synonym => await Adjective.SynonymsOf(adjective),
            WordRelation.MeaningLike => await Adjective.MeaningLike(adjective)
        });
    }
}
