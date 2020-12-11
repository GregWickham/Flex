using System.Collections.Generic;

namespace NHunspell
{
    public static class Thesaurus
    {
        public static IEnumerable<string> SynonymsFor(string word)
        {
            MyThes thes = new MyThes();
            {
                using (Hunspell hunspell = new Hunspell("D:\\Natural Language\\Hunspell\\en_US.aff", "D:\\Natural Language\\Hunspell\\en_US.dic"))
                {
                    ThesResult tr = thes.Lookup("cars", hunspell);
                    return tr.GetSynonyms().Keys;
                }
            }
            
        }
    }
}
