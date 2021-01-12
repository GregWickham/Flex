using System;
using System.Linq;
using System.Threading.Tasks;
using FlexibleRealization;

namespace Flex.Database
{
    public partial class FlexDataContext
    {
        /// <summary>Load the ElementBuilder with  the specified <paramref name="elementID"/> from the Flex database, and return it</summary>
        public ElementBuilder Load(int elementID) 
        {
            DB_Element element = DB_Elements
                .Where(dbElement => dbElement.ID.Equals(elementID))
                .Single();
            ElementBuilder result;
            switch ((FlexData.ElementType)element.ElementType)
            {
                case FlexData.ElementType.DB_WordElement:
                    result = LoadWord(elementID);
                    break;
                case FlexData.ElementType.DB_ParentElement:
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
        public void Save(IElementTreeNode element)
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
        public Task SaveAsync(IElementTreeNode element) => Task.Run(() => Save(element));

    }
}
