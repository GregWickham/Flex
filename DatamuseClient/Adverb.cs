using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Adverb
    {
        public static async Task<IEnumerable<Word>> SynonymsFor(string adverb) => (await Client.MeaningLike(adverb))
            .Where(word => word.Tags != null && word.Tags.Contains("adv"))
            .OrderByDescending(word => word.Score);
    }
}
