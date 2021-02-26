using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Verb
    {
        public static async Task<IEnumerable<Word>> SynonymsOf(string verb) => (await Client.SynonymsOf(verb))
            .Where(word => word.Tags != null && word.Tags.Contains("v"))
            .OrderByDescending(word => word.Score);

        public static async Task<IEnumerable<Word>> MeaningLike(string verb) => (await Client.MeaningLike(verb))
            .Where(word => word.Tags != null && word.Tags.Contains("v"))
            .OrderByDescending(word => word.Score);
    }
}
