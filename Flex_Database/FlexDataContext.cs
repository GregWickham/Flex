using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FlexibleRealization;

namespace Flex.Database
{
    public delegate void ElementChanged_EventHandler(int elementID);
    public delegate void WordChanged_EventHandler(int wordID);
    public delegate void ParentChanged_EventHandler(int parentID);
    public delegate void DatabaseSaveCompleted_EventHandler(IElementTreeNode saved);

    public partial class FlexDataContext
    {
        public event ElementChanged_EventHandler ElementChanged;
        private void OnElementChanged(int elementID) => ElementChanged?.Invoke(elementID);

        public event WordChanged_EventHandler WordChanged;
        private void OnWordChanged(int wordID)
        {
            ElementChanged?.Invoke(wordID);
            WordChanged?.Invoke(wordID);
        }

        public event ParentChanged_EventHandler ParentChanged;
        private void OnParentChanged(int parentID)
        {
            ElementChanged?.Invoke(parentID);
            ParentChanged?.Invoke(parentID);
        }

        public event DatabaseSaveCompleted_EventHandler SaveCompleted;
        private void OnSaveCompleted(IElementTreeNode saved) => SaveCompleted?.Invoke(saved);

        /// <summary>Load the ElementBuilder with  the specified <paramref name="elementID"/> from the Flex database, and return it</summary>
        private ElementBuilder Load(int elementID)
        {
            DB_Element element = DB_Elements.Single(dbElement => dbElement.ID.Equals(elementID));
            ElementBuilder result;
            switch ((FlexData.ElementType)element.ElementType)
            {
                case FlexData.ElementType.DB_Word:
                    result = LoadWord(elementID);
                    break;
                case FlexData.ElementType.DB_Parent:
                    result = LoadParent(elementID);
                    break;
                default:
                    throw new InvalidOperationException("");
            }
            return result;
        }

        /// <summary>Async version of <see cref="Load(int)"/></summary>
        public Task<ElementBuilder> LoadAsync(int elementID) => Task.Run(() => Load(elementID));

        /// <summary>Save <paramref name="element"/> to the Flex database</summary>
        private void Save(IElementTreeNode element)
        {
            switch (element)
            {
                case WordElementBuilder wb:
                    SaveWord(wb);
                    break;
                case ParentElementBuilder pb:
                    SaveParent(pb);
                    break;
                default: break;
            }
        }

        /// <summary>Async version of <see cref="Save(IElementTreeNode)"/></summary>
        public Task SaveAsync(IElementTreeNode element) => Task.Run(() =>
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                Save(element);
                transaction.Complete();
            }
            OnSaveCompleted(element);
        });

    }
}
