using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Adjective
    {
        public static async Task<IEnumerable<Word>> SynonymsOf(string adjective) => (await Client.SynonymsOf(adjective))
            .Where(word => word.Tags != null && word.Tags.Contains("adj"))
            .OrderByDescending(word => word.Score);

        public static async Task<IEnumerable<Word>> MeaningLike(string adjective) => (await Client.MeaningLike(adjective))
            .Where(word => word.Tags != null && word.Tags.Contains("adj"))
            .OrderByDescending(word => word.Score);
    }
}
