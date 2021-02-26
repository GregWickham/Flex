using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class VerbChoices : WordChoices
    {
        public override sealed async Task GetRelatedWords(string verb, WordRelation relation) => SetLookedUp(relation switch
        {
            WordRelation.Synonym => await Verb.SynonymsOf(verb),
            WordRelation.MeaningLike => await Verb.MeaningLike(verb)
        });
    }
}
