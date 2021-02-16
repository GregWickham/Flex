namespace WordNet.UserInterface
{
    public delegate void SynsetDragStarted_EventHandler(int draggedSynsetID);

    public delegate void SynsetDragCancelled_EventHandler(int draggedSynsetID);

    public delegate void SynsetDropCompleted_EventHandler(int draggedSynsetID);
}
