using System.Linq;
using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class AdverbChoices : WordChoices
    {
        public override sealed Task GetSynonymsFor(string adverb) => Task.Run(() =>
        {
            SetLookedUp(Adverb.SynonymsFor(adverb).Result.OrderBy(word => word.Score));
        });
    }
}
