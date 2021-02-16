using System.Collections;
using System.Collections.Generic;
using FlexibleRealization;

namespace Flex.UserInterface.ViewModels
{
    public interface IWordEditorViewModel
    {
        IEnumerable<string> Potential { get; }

        IEnumerable<IWeightedWord> Actual { get; }

        void MoveFromPotentialToActual(IList wordsToMove);

        void MoveFromActualToPotential(IList dbWeightedWordsToMove);

        void SetPivot(string newPivot);
    }
}
