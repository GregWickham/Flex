using System.Windows;
using WordNet.Linq;

namespace WordNet.UserInterface
{
    public delegate void WordSenseSelected_EventHandler(WordSense wordSense);

    public delegate void SynsetSelected_EventHandler(Synset synset);

    public delegate void EditingEnabledChanged_EventHandler(bool editingEnabled);

    public delegate WordSpecification DroppedWordConverter(DragEventArgs e);
}
