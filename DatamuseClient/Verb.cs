using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Verb
    {
        public static async Task<IEnumerable<Word>> SynonymsFor(string verb) => (await Client.MeaningLike(verb))
            .Where(word => word.Tags != null && word.Tags.Contains("v"))
            .OrderByDescending(word => word.Score);
    }
}
