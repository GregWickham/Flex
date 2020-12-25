﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GraphX.Measure;
using GraphX.Common.Interfaces;
using GraphX.Logic.Algorithms.OverlapRemoval;
using FlexibleRealization.UserInterface.ViewModels;

namespace FlexibleRealization.UserInterface
{
    public class ElementBuilderOverlapRemovalAlgorithm : IExternalOverlapRemoval<ElementVertex>
    {
        public IDictionary<ElementVertex, Rect> Rectangles { get; set; }

        public void Compute(CancellationToken cancellationToken)
        {
            RunFSA(cancellationToken);
            RealignPartsOfSpeechWithTokens();
            //RecenterSyntaxElementsOverSpannedPartsOfSpeech();
        }

        private void RunFSA(CancellationToken cancellationToken)
        {
            OverlapRemovalParameters parameters = new OverlapRemovalParameters
            {
                HorizontalGap = 20,
                VerticalGap = 30
            };
            var oneWayFSA = new FSAAlgorithm<ElementVertex>(Rectangles, parameters);
            oneWayFSA.Compute(cancellationToken);
            Rectangles = oneWayFSA.Rectangles;
        }

        private IEnumerable<ParentElementVertex> ParentElements => Rectangles
            .Where(kvp => kvp.Key is ParentElementVertex)
            .Select(kvp => kvp.Key)
            .Cast<ParentElementVertex>();

        private IEnumerable<WordPartOfSpeechVertex> PartsOfSpeech => Rectangles
            .Where(kvp => kvp.Key is WordPartOfSpeechVertex)
            .Select(kvp => kvp.Key)
            .Cast<WordPartOfSpeechVertex>();

        private IEnumerable<WordContentVertex> WordContents => Rectangles
            .Where(kvp => kvp.Key is WordContentVertex)
            .Select(kvp => kvp.Key)
            .Cast<WordContentVertex>();

        private WordPartOfSpeechVertex PartOfSpeechCorrespondingTo(WordContentVertex contentVertex) => PartsOfSpeech
            .Where(partOfSpeechVertex => partOfSpeechVertex.Model.WordSource == contentVertex.Model)
            .First();

        private IEnumerable<WordPartOfSpeechVertex> PartsOfSpeechSpannedBy(ParentElementBuilder parentElement)
        {
            IEnumerable<PartOfSpeechBuilder> partsOfSpeechInSubtree = parentElement.GetElementsOfTypeInSubtree<PartOfSpeechBuilder>();
            return PartsOfSpeech.Where(partOfSpeechVertex => partsOfSpeechInSubtree.Contains(partOfSpeechVertex.Model));
        }

        private void RealignPartsOfSpeechWithTokens()
        {
            IDictionary<ElementVertex, Rect> newRectangles = new Dictionary<ElementVertex, Rect>();
            foreach (ElementVertex eachElementVertex in Rectangles.Keys)
            {
                newRectangles.Add(eachElementVertex, Rectangles[eachElementVertex]);
            }
            foreach (WordContentVertex eachContentVertex in WordContents)
            {
                Rect tokenRectangle = Rectangles[eachContentVertex];
                WordPartOfSpeechVertex correspondingPartOfSpeech = PartOfSpeechCorrespondingTo(eachContentVertex);
                Rect oldPartOfSpeechRectangle = Rectangles[correspondingPartOfSpeech];
                double newPartOfSpeechX = tokenRectangle.X + ((tokenRectangle.Width - oldPartOfSpeechRectangle.Width) / 2);
                newRectangles[correspondingPartOfSpeech] = new Rect(new Point(newPartOfSpeechX, oldPartOfSpeechRectangle.Y), oldPartOfSpeechRectangle.Size);
            }
            Rectangles = newRectangles;
        }

        private void RecenterSyntaxElementsOverSpannedPartsOfSpeech()
        {
            foreach (ParentElementVertex eachParentVertex in ParentElements.ToList())
            {
                double horizontalCenterOfSpannedPartsOfSpeech = PartsOfSpeechSpannedBy(eachParentVertex.Model)
                    .Average(posv => Rectangles[posv].GetCenter().X);  // Average the CENTERS of the spanned part of speech vertices
                Rect oldRectForThisSyntaxElement = Rectangles[eachParentVertex];
                double newLeftEdgeOfThisSyntaxElement = horizontalCenterOfSpannedPartsOfSpeech - (Rectangles[eachParentVertex].Width / 2);
                Point newTopLeft = new Point(newLeftEdgeOfThisSyntaxElement, Rectangles[eachParentVertex].Y);
                Rectangles[eachParentVertex] = new Rect(newTopLeft, oldRectForThisSyntaxElement.Size);
            }
        }
    }
}
