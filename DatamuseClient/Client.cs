using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Datamuse
{
    public static class Client
    {
        private static readonly HttpClient HTTP_Client = new HttpClient();

        private static readonly string WordsEndpoint = "https://api.datamuse.com/words";
        private static async Task<IEnumerable<Word>> ResponseTo(string request)
        {
            try
            {
                string responseBody = await HTTP_Client.GetStringAsync(request);
                return JsonConvert.DeserializeObject<IList<Word>>(responseBody)
                    .OrderBy(result => result.Score);
            }
            catch (HttpRequestException e)
            {
                return new List<Word>();
            }
        }

        public static async Task<IEnumerable<Word>> MeaningLike(string word) => await ResponseTo($"{WordsEndpoint}?ml={word}");
    }
}
