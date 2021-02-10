using System.Linq;
using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class NounChoices : WordChoices
    {
        public override sealed Task GetSynonymsFor(string noun) => Task.Run(() =>
        {
            SetLookedUp(Noun.SynonymsFor(noun).Result.OrderBy(word => word.Score));
        });
    }
}
