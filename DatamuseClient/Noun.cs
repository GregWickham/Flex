using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datamuse
{
    public static class Noun
    {
        public static async Task<IEnumerable<string>> SynonymsFor(string noun) => (await Client.MeaningLike(noun))
            .Where(word => word.Tags.Contains("n"))
            .OrderByDescending(word => word.Score)
            .Select(word => word.Text);
    }
}
