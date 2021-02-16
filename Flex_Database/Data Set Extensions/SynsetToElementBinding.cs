using WordNet.Linq;

namespace Flex.Database
{
    public partial class SynsetToElementBinding
    {
        public string Gloss => Synset.WithID(SynsetID).GlossWithoutExamples;
    }
}
