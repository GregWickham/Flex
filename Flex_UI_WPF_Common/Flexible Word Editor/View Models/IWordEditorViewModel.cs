using System.Collections;
using System.Collections.Generic;
using FlexibleRealization;
using Datamuse;

namespace Flex.UserInterface.ViewModels
{
    public interface IWordEditorViewModel
    {
        string DefaultWord { get; }

        bool HasAlternates { get; }

        IEnumerable<string> PotentialAlternates { get; }

        IEnumerable<IWeightedWord> ActualAlternates { get; }

        IEnumerable<IWeightedWord> AllChoices { get; }

        void MoveFromPotentialToActual(IList wordsToMove);

        void MoveFromActualToPotential(IList dbWeightedWordsToMove);

        WordRelation RelationToPivot { get; }

        void SetRelationToPivot(WordRelation relation);

        string Pivot { get; }

        void SetPivot(string newPivot);
    }
}
