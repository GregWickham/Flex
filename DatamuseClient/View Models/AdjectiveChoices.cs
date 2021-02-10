using System.Linq;
using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class AdjectiveChoices : WordChoices
    {
        public override sealed Task GetSynonymsFor(string adjective) => Task.Run(() =>
        {
            SetLookedUp(Adjective.SynonymsFor(adjective).Result.OrderBy(word => word.Score));
        });
    }
}
