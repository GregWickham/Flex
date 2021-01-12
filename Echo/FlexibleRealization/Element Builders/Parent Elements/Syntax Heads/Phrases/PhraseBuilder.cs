﻿using System.Collections.Generic;
using System.Linq;
using SimpleNLG;

namespace FlexibleRealization
{
    /// <summary>The base class of all PhraseElement builders.</summary>
    /// <remarks>The inheritance hierarchy of PhraseBuilder roughly corresponds to the hierarchy of <see cref="PhraseElement"/>s defined by the SimpleNLG Realizer Schema.</remarks>
    public abstract class PhraseBuilder : SyntaxHeadBuilder
    {
        public abstract phraseCategory PhraseCategory { get; }

        /// <summary>Since this is already a phrase, return this</summary>
        public override PhraseBuilder AsPhrase() => this;

        /// <summary>Return the heads of this phrase</summary>
        internal IEnumerable<IElementTreeNode> Heads => ChildrenWithRole(ChildRole.Head);

        /// <summary>Return the modifiers of this phrase</summary>
        internal IEnumerable<IElementTreeNode> Modifiers => ChildrenWithRole(ChildRole.Modifier);

        /// <summary>Return the complements of this phrase</summary>
        internal IEnumerable<IElementTreeNode> Complements => ChildrenWithRole(ChildRole.Complement);

        internal void AddHead(IElementTreeNode head) => AddChildWithRole(head, ChildRole.Head);
        private protected void AddHeads(IEnumerable<IElementTreeNode> newHeads) => newHeads.ToList().ForEach(newHead => AddHead(newHead));

        private protected void AddModifier(IElementTreeNode modifier) => AddChildWithRole(modifier, ChildRole.Modifier);
        private protected void AddModifiers(IEnumerable<IElementTreeNode> newModifiers) => newModifiers.ToList().ForEach(newModifier => AddModifier(newModifier));

        private protected void AddComplement(IElementTreeNode complement) => AddChildWithRole(complement, ChildRole.Complement);
        private protected void AddComplements(IEnumerable<IElementTreeNode> newComplements) => newComplements.ToList().ForEach(newComplement => AddComplement(newComplement));

        /// <summary>Default override of Consolidate for PhraseBuilders.</summary>
        /// <remarks>When a PhraseBuilder has exactly one child, we do NOT eliminate the PhraseBuilder and pass through that child, because this is
        /// the case when a phrase is used to define inflection features of its headword.</remarks>
        public override void Consolidate()
        {
            if (Children.Count() == 0) Become(null);
        }

        #region Phrase features

        public abstract bool DiscourseFunctionSpecified { get; set; }
        public abstract discourseFunction DiscourseFunction { get; set; }
        public abstract bool AppositiveSpecified { get; set; }
        public abstract bool Appositive { get; set; }

        #endregion Phrase features
    }
}
