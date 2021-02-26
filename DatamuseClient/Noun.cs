using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Noun
    {
        public static async Task<IEnumerable<Word>> SynonymsOf(string noun) => (await Client.SynonymsOf(noun))
            .Where(word => word.Tags != null && word.Tags.Contains("n"))
            .OrderByDescending(word => word.Score);

        public static async Task<IEnumerable<Word>> MeaningLike(string noun) => (await Client.MeaningLike(noun))
            .Where(word => word.Tags != null && word.Tags.Contains("n"))
            .OrderByDescending(word => word.Score);
    }
}
