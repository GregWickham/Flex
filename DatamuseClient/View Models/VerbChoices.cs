using System.Linq;
using System.Threading.Tasks;

namespace Datamuse.ViewModels
{
    public class VerbChoices : WordChoices
    {
        public override sealed Task GetSynonymsFor(string verb) => Task.Run(() =>
        {
            SetLookedUp(Verb.SynonymsFor(verb).Result.OrderBy(word => word.Score));
        });
    }
}
