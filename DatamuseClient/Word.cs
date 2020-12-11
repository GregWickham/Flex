using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datamuse
{
    public class Word
    {
        public string word { get; set; }

        public string Text => word;

        public int Score { get; set; }

        public IList<string> Tags { get; set; }
    }


}
