using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class AdverbChoices : WordChoices
    {
        public override sealed async Task GetRelatedWords(string adverb, WordRelation relation) => SetLookedUp(relation switch
        {
            WordRelation.Synonym => await Adverb.SynonymsOf(adverb),
            WordRelation.MeaningLike => await Adverb.MeaningLike(adverb)
        });
    }
}
