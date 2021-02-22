using WordNet.Linq;

namespace WordNet.UserInterface
{
    public delegate void SynsetDragStarted_EventHandler(Synset synset);

    public delegate void SynsetDragCancelled_EventHandler(Synset synset);

    public delegate void SynsetDropCompleted_EventHandler(Synset synset);


    public delegate void WordSenseDragStarted_EventHandler(WordSense wordSense);

    public delegate void WordSenseDragCancelled_EventHandler(WordSense wordSense);

    public delegate void WordSenseDropCompleted_EventHandler(WordSense wordSense);
}
