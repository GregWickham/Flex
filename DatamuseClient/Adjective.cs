using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Adjective
    {
        public static async Task<IEnumerable<string>> SynonymsFor(string adjective) => (await Client.MeaningLike(adjective))
            .Where(word => word.Tags.Contains("adj"))
            .OrderByDescending(word => word.Score)
            .Select(word => word.Text);
    }
}
