using System.Collections.Generic;
using FlexibleRealization;

namespace Flex.Database.UserInterface.ViewModels
{
    internal class WordBuildersViewModel
    {
        internal WordBuildersViewModel()
        {
            //VisibleWords = FlexData.Context.
        }

        public IEnumerable<WordElementBuilder> VisibleWords { get; private set; } 
    }
}
