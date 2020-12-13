using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Verb
    {
        public static async Task<IEnumerable<string>> SynonymsFor(string adjective) => (await Client.MeaningLike(adjective))
            .Where(word => word.Tags.Contains("v"))
            .OrderByDescending(word => word.Score)
            .Select(word => word.Text);
    }
}
