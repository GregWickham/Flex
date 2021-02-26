using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Adverb
    {
        public static async Task<IEnumerable<Word>> SynonymsOf(string adverb) => (await Client.SynonymsOf(adverb))
            .Where(word => word.Tags != null && word.Tags.Contains("adv"))
            .OrderByDescending(word => word.Score);

        public static async Task<IEnumerable<Word>> MeaningLike(string adverb) => (await Client.MeaningLike(adverb))
            .Where(word => word.Tags != null && word.Tags.Contains("adv"))
            .OrderByDescending(word => word.Score);
    }
}
