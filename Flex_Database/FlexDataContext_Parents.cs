using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleNLG;
using FlexibleRealization;

namespace Flex.Database
{
    public partial class FlexDataContext
    {
        public IQueryable<DB_ParentElement> LoadAllDB_ParentElements() => DB_Elements
            .Where(dbElement => dbElement.ElementType.Equals(FlexData.ElementType.DB_ParentElement))
            .Cast<DB_ParentElement>();

        public Task<IQueryable<DB_ParentElement>> LoadAllDB_ParentElementsAsync => Task.Run(() => LoadAllDB_ParentElements());

        public IQueryable<ParentElementBuilder> LoadAllParents() => LoadAllDB_ParentElements()
            .Select(dbElement => LoadParent(dbElement.ID));

        public Task<IQueryable<ParentElementBuilder>> LoadAllParentsAsync() => Task.Run(() => LoadAllParents());

        public ParentElementBuilder LoadParent(int parentBuilder_ID)
        {
            DB_ParentElement dbElement = DB_Elements
                .Where(builder => builder.ID.Equals(parentBuilder_ID))
                .Cast<DB_ParentElement>()
                .Single();
            ParentElementBuilder parent = FlexData.Parent.ElementOfType((byte)dbElement.ParentType);
            parent.FlexDB_ID = dbElement.ID;
            CopyDataFromDB_ParentElement(dbElement, parent);
            LoadChildrenOf(parent);
            return parent;
        }

        private void CopyDataFromDB_ParentElement(DB_ParentElement dbParentElement, ParentElementBuilder parent)
        {
            switch (parent)
            {
                case ClauseBuilder clause:
                    clause.DiscourseFunctionSpecified = dbParentElement.DiscourseFunction != null;
                    if (clause.DiscourseFunctionSpecified)
                        clause.DiscourseFunction = (discourseFunction)dbParentElement.DiscourseFunction;
                    clause.AppositiveSpecified = dbParentElement.Appositive != null;
                    if (clause.AppositiveSpecified)
                        clause.Appositive = (bool)dbParentElement.Appositive;
                    CopyClauseDataFromDB_ParentElement(dbParentElement, clause);
                    break;
                case PhraseBuilder phrase:
                    phrase.DiscourseFunctionSpecified = dbParentElement.DiscourseFunction != null;
                    if (phrase.DiscourseFunctionSpecified)
                        phrase.DiscourseFunction = (discourseFunction)dbParentElement.DiscourseFunction;
                    phrase.AppositiveSpecified = dbParentElement.Appositive != null;
                    if (phrase.AppositiveSpecified)
                        phrase.Appositive = (bool)dbParentElement.Appositive;
                    CopyPhraseDataFromDB_ParentElement(dbParentElement, phrase);
                    break;
                default: throw new InvalidOperationException("Can't populate data to this type of ParentElementBuilder from the Flex database");
            }
        }

        private void CopyClauseDataFromDB_ParentElement(DB_ParentElement dbClauseElement, ClauseBuilder clause)
        {
            clause.AggregateAuxiliarySpecified = dbClauseElement.AggregateAuxiliary != null;
            if (clause.AggregateAuxiliarySpecified)
                clause.AggregateAuxiliary = (bool)dbClauseElement.AggregateAuxiliary;
            if (dbClauseElement.Complementizer != null)
                clause.Complementiser = dbClauseElement.Complementizer;
            clause.FormSpecified = dbClauseElement.Form != null;
            if (clause.FormSpecified)
                clause.Form = (form)dbClauseElement.Form;
            clause.InterrogativeTypeSpecified = dbClauseElement.InterrogativeType != null;
            if (clause.InterrogativeTypeSpecified)
                clause.InterrogativeType = (interrogativeType)dbClauseElement.InterrogativeType;
            if (dbClauseElement.Modal != null)
                clause.Modal = dbClauseElement.Modal;
            clause.NegatedSpecified = dbClauseElement.Negated != null;
            if (clause.NegatedSpecified)
                clause.Negated = (bool)dbClauseElement.Negated;
            clause.PassiveSpecified = dbClauseElement.Passive != null;
            if (clause.PassiveSpecified)
                clause.Passive = (bool)dbClauseElement.Passive;
            clause.PerfectSpecified = dbClauseElement.Perfect != null;
            if (clause.PerfectSpecified)
                clause.Perfect = (bool)dbClauseElement.Perfect;
            clause.PersonSpecified = dbClauseElement.Person != null;
            if (clause.PersonSpecified)
                clause.Person = (person)dbClauseElement.Person;
            clause.ProgressiveSpecified = dbClauseElement.Progressive != null;
            if (clause.ProgressiveSpecified)
                clause.Progressive = (bool)dbClauseElement.Progressive;
            clause.SuppressGenitiveInGerundSpecified = dbClauseElement.SuppressGenitiveInGerund != null;
            if (clause.SuppressGenitiveInGerundSpecified)
                clause.SuppressGenitiveInGerund = (bool)dbClauseElement.SuppressGenitiveInGerund;
            clause.SuppressedComplementiserSpecified = dbClauseElement.SuppressedComplementizer != null;
            if (clause.SuppressedComplementiserSpecified)
                clause.SuppressedComplementiser = (bool)dbClauseElement.SuppressedComplementizer;
            clause.TenseSpecified = dbClauseElement.Tense != null;
            if (clause.TenseSpecified)
                clause.Tense = (tense)dbClauseElement.Tense;
        }

        private void CopyPhraseDataFromDB_ParentElement(DB_ParentElement dbPhraseElement, PhraseBuilder phrase)
        {
            switch (phrase)
            {
                case NounPhraseBuilder nounPhrase:
                    nounPhrase.AdjectiveOrderingSpecified = dbPhraseElement.AdjectiveOrdering != null;
                    if (nounPhrase.AdjectiveOrderingSpecified)
                        nounPhrase.AdjectiveOrdering = (bool)dbPhraseElement.AdjectiveOrdering;
                    nounPhrase.ElidedSpecified = dbPhraseElement.Elided != null;
                    if (nounPhrase.ElidedSpecified)
                        nounPhrase.Elided = (bool)dbPhraseElement.Elided;
                    nounPhrase.NumberSpecified = dbPhraseElement.Number != null;
                    if (nounPhrase.NumberSpecified)
                        nounPhrase.Number = (numberAgreement)dbPhraseElement.Number;
                    nounPhrase.GenderSpecified = dbPhraseElement.Gender != null;
                    if (nounPhrase.GenderSpecified)
                        nounPhrase.Gender = (gender)dbPhraseElement.Gender;
                    nounPhrase.PersonSpecified = dbPhraseElement.Person != null;
                    if (nounPhrase.PersonSpecified)
                        nounPhrase.Person = (person)dbPhraseElement.Person;
                    nounPhrase.PossessiveSpecified = dbPhraseElement.Possessive != null;
                    if (nounPhrase.PossessiveSpecified)
                        nounPhrase.Possessive = (bool)dbPhraseElement.Possessive;
                    nounPhrase.PronominalSpecified = dbPhraseElement.Pronominal != null;
                    if (nounPhrase.PronominalSpecified)
                        nounPhrase.Pronominal = (bool)dbPhraseElement.Pronominal;
                    break;
                case VerbPhraseBuilder verbPhrase:
                    verbPhrase.AggregateAuxiliarySpecified = dbPhraseElement.AggregateAuxiliary != null;
                    if (verbPhrase.AggregateAuxiliarySpecified)
                        verbPhrase.AggregateAuxiliary = (bool)dbPhraseElement.AggregateAuxiliary;
                    verbPhrase.FormSpecified = dbPhraseElement.Form != null;
                    if (verbPhrase.FormSpecified)
                        verbPhrase.Form = (form)dbPhraseElement.Form;
                    if (dbPhraseElement.Modal != null)
                        verbPhrase.Modal = dbPhraseElement.Modal;
                    verbPhrase.NegatedSpecified = dbPhraseElement.Negated != null;
                    if (verbPhrase.NegatedSpecified)
                        verbPhrase.Negated = (bool)dbPhraseElement.Negated;
                    verbPhrase.PassiveSpecified = dbPhraseElement.Passive != null;
                    if (verbPhrase.PassiveSpecified)
                        verbPhrase.Passive = (bool)dbPhraseElement.Passive;
                    verbPhrase.PerfectSpecified = dbPhraseElement.Perfect != null;
                    if (verbPhrase.PerfectSpecified)
                        verbPhrase.Perfect = (bool)dbPhraseElement.Perfect;
                    verbPhrase.PersonSpecified = dbPhraseElement.Person != null;
                    if (verbPhrase.PersonSpecified)
                        verbPhrase.Person = (person)dbPhraseElement.Person;
                    verbPhrase.ProgressiveSpecified = dbPhraseElement.Progressive != null;
                    if (verbPhrase.ProgressiveSpecified)
                        verbPhrase.Progressive = (bool)dbPhraseElement.Progressive;
                    verbPhrase.SuppressGenitiveInGerundSpecified = dbPhraseElement.SuppressGenitiveInGerund != null;
                    if (verbPhrase.SuppressGenitiveInGerundSpecified)
                        verbPhrase.SuppressGenitiveInGerund = (bool)dbPhraseElement.SuppressGenitiveInGerund;
                    verbPhrase.SuppressedComplementiserSpecified = dbPhraseElement.SuppressedComplementizer != null;
                    if (verbPhrase.SuppressedComplementiserSpecified)
                        verbPhrase.SuppressedComplementiser = (bool)dbPhraseElement.SuppressedComplementizer;
                    verbPhrase.TenseSpecified = dbPhraseElement.Tense != null;
                    if (verbPhrase.TenseSpecified)
                        verbPhrase.Tense = (tense)dbPhraseElement.Tense;
                    break;
                case AdjectivePhraseBuilder adjectivePhrase:
                    adjectivePhrase.ComparativeSpecified = dbPhraseElement.IsComparative != null;
                    if (adjectivePhrase.ComparativeSpecified)
                        adjectivePhrase.Comparative = (bool)dbPhraseElement.IsComparative;
                    adjectivePhrase.SuperlativeSpecified = dbPhraseElement.IsSuperlative != null;
                    if (adjectivePhrase.SuperlativeSpecified)
                        adjectivePhrase.Superlative = (bool)dbPhraseElement.IsSuperlative;
                    break;
                case AdverbPhraseBuilder adverbPhrase:
                    adverbPhrase.ComparativeSpecified = dbPhraseElement.IsComparative != null;
                    if (adverbPhrase.ComparativeSpecified)
                        adverbPhrase.Comparative = (bool)dbPhraseElement.IsComparative;
                    adverbPhrase.SuperlativeSpecified = dbPhraseElement.IsSuperlative != null;
                    if (adverbPhrase.SuperlativeSpecified)
                        adverbPhrase.Superlative = (bool)dbPhraseElement.IsSuperlative;
                    break;
                case PrepositionalPhraseBuilder prepositionalPhrase:
                    break;
                default: throw new InvalidOperationException("Can't populate data to this type of PhraseBuilder from the Flex database");
            }
        }

        private void LoadChildrenOf(ParentElementBuilder parent)
        {
            IEnumerable<DB_ParentChildRelation> childRelations = DB_ParentChildRelations
                .Where(relation => relation.Parent.Equals(parent.FlexDB_ID));
            foreach (DB_ParentChildRelation eachRelation in childRelations)
            {
                ElementBuilder child = Load(eachRelation.Child);
                parent.AddChildWithRole(child, (ParentElementBuilder.ChildRole)eachRelation.Role);
            }
        }

        private void SaveParent(ParentElementBuilder parentElement) 
        {
            RealizationResult realization = parentElement.AsRealizableTree().Realize();
            if (realization.Outcome == RealizationOutcome.Success)
            {
                DB_ParentElement dbParentElement;
                bool isNewDB_Record = parentElement.FlexDB_ID == 0;
                // First we need to make sure we have a ParentElementBuilder with a valid FlexDB_ID.
                // We'll need that FlexDB_ID so DB_ParentChildRelations can refer to it when we save them in the database.
                if (isNewDB_Record)        // The phrase builder does not already exist in the database.  We need to insert it so it has an ID.
                {
                    dbParentElement = new DB_ParentElement();
                    CopyDataToDB_ParentElement(dbParentElement, parentElement, realization.Text);
                    DB_Elements.InsertOnSubmit(dbParentElement);
                    SubmitChanges();
                    // Now we have the ID available from the database.  Assign the ID to the in-memory object, so we'll remember it's not new if we update it
                    parentElement.FlexDB_ID = dbParentElement.ID;
                }
                else    // The word builder already exists in the database
                {
                    dbParentElement = DB_Elements
                        .Where(dbElement => dbElement.ID.Equals(parentElement.FlexDB_ID))
                        .Cast<DB_ParentElement>()
                        .Single();
                    dbParentElement.ParentDefaultRealization = realization.Text;
                    CopyDataToDB_ParentElement(dbParentElement, parentElement, realization.Text);
                }
                SubmitChanges();
                DeleteChildRelationsFor(parentElement);
                CreateChildRelationsFor(parentElement);
            }
        }

        private void CopyDataToDB_ParentElement(DB_ParentElement dbParentElement, ParentElementBuilder parent, string defaultRealization)
        {
            dbParentElement.ParentType = FlexData.Parent.TypeOf(parent);
            dbParentElement.ParentDefaultRealization = defaultRealization;
            switch (parent)
            {
                case ClauseBuilder clause:
                    dbParentElement.DiscourseFunction = clause.DiscourseFunctionSpecified ? (byte)clause.DiscourseFunction : null;
                    dbParentElement.Appositive = clause.AppositiveSpecified ? clause.Appositive : null;
                    CopyClauseDataToDB_ParentElement(dbParentElement, clause);
                    break;
                case PhraseBuilder phrase:
                    dbParentElement.DiscourseFunction = phrase.DiscourseFunctionSpecified ? (byte)phrase.DiscourseFunction : null;
                    dbParentElement.Appositive = phrase.AppositiveSpecified ? phrase.Appositive : null;
                    CopyPhraseDataToDB_ParentElement(dbParentElement, phrase);
                    break;
                default: throw new InvalidOperationException("Can't save this type of ParentElementBuilder to the Flex database");
            }
        }

        private void CopyClauseDataToDB_ParentElement(DB_ParentElement dbClauseElement, ClauseBuilder clause)
        {
            dbClauseElement.AggregateAuxiliary = clause.AggregateAuxiliarySpecified ? clause.AggregateAuxiliary : null;
            dbClauseElement.Complementizer = clause.Complementiser;
            dbClauseElement.Form = clause.FormSpecified ? (byte)clause.Form : null;
            dbClauseElement.InterrogativeType = clause.InterrogativeTypeSpecified ? (byte)clause.InterrogativeType : null;
            dbClauseElement.Modal = clause.Modal;
            dbClauseElement.Negated = clause.NegatedSpecified ? clause.Negated : null;
            dbClauseElement.Passive = clause.PassiveSpecified ? clause.Passive : null;
            dbClauseElement.Perfect = clause.PerfectSpecified ? clause.Perfect : null;
            dbClauseElement.Person = clause.PersonSpecified ? (byte)clause.Person : null;
            dbClauseElement.Progressive = clause.ProgressiveSpecified ? clause.Progressive : null;
            dbClauseElement.SuppressGenitiveInGerund = clause.SuppressGenitiveInGerundSpecified ? clause.SuppressGenitiveInGerund : null;
            dbClauseElement.SuppressedComplementizer = clause.SuppressedComplementiserSpecified ? clause.SuppressedComplementiser : null;
            dbClauseElement.Tense = clause.TenseSpecified ? (byte)clause.Tense : null;
        }

        private void CopyPhraseDataToDB_ParentElement(DB_ParentElement dbPhraseElement, PhraseBuilder phrase)
        {
            switch (phrase)
            {
                case NounPhraseBuilder nounPhrase:
                    dbPhraseElement.AdjectiveOrdering = nounPhrase.AdjectiveOrderingSpecified ? nounPhrase.AdjectiveOrdering : null;
                    dbPhraseElement.Elided = nounPhrase.ElidedSpecified ? nounPhrase.Elided : null;
                    dbPhraseElement.Number = nounPhrase.NumberSpecified ? (byte)nounPhrase.Number : null;
                    dbPhraseElement.Gender = nounPhrase.GenderSpecified ? (byte)nounPhrase.Gender : null;
                    dbPhraseElement.Person = nounPhrase.PersonSpecified ? (byte)nounPhrase.Person : null;
                    dbPhraseElement.Possessive = nounPhrase.PossessiveSpecified ? nounPhrase.Possessive : null;
                    dbPhraseElement.Pronominal = nounPhrase.PronominalSpecified ? nounPhrase.Pronominal : null;
                    break;
                case VerbPhraseBuilder verbPhrase:
                    dbPhraseElement.AggregateAuxiliary = verbPhrase.AggregateAuxiliarySpecified ? verbPhrase.AggregateAuxiliary : null;
                    dbPhraseElement.Form = verbPhrase.FormSpecified ? (byte)verbPhrase.Form : null;
                    dbPhraseElement.Modal = verbPhrase.Modal;
                    dbPhraseElement.Negated = verbPhrase.NegatedSpecified ? verbPhrase.Negated : null;
                    dbPhraseElement.Passive = verbPhrase.PassiveSpecified ? verbPhrase.Passive : null;
                    dbPhraseElement.Perfect = verbPhrase.PerfectSpecified ? verbPhrase.Perfect : null;
                    dbPhraseElement.Person = verbPhrase.PersonSpecified ? (byte)verbPhrase.Person : null;
                    dbPhraseElement.Progressive = verbPhrase.ProgressiveSpecified ? verbPhrase.Progressive : null;
                    dbPhraseElement.SuppressGenitiveInGerund = verbPhrase.SuppressGenitiveInGerundSpecified ? verbPhrase.SuppressGenitiveInGerund : null;
                    dbPhraseElement.SuppressedComplementizer = verbPhrase.SuppressedComplementiserSpecified ? verbPhrase.SuppressedComplementiser : null;
                    break;
                case AdjectivePhraseBuilder adjectivePhrase:
                    dbPhraseElement.IsComparative = adjectivePhrase.ComparativeSpecified ? adjectivePhrase.Comparative : null;
                    dbPhraseElement.IsSuperlative = adjectivePhrase.SuperlativeSpecified ? adjectivePhrase.Superlative : null;
                    break;
                case AdverbPhraseBuilder adverbPhrase:
                    dbPhraseElement.IsComparative = adverbPhrase.ComparativeSpecified ? adverbPhrase.Comparative : null;
                    dbPhraseElement.IsSuperlative = adverbPhrase.SuperlativeSpecified ? adverbPhrase.Superlative : null;
                    break;
                case PrepositionalPhraseBuilder prepositionalPhrase:
                    break;
                default: throw new InvalidOperationException("Can't save this type of PhraseBuilder to the Flex database");
            }
        }

        private void DeleteChildRelationsFor(ParentElementBuilder parent)
        {
            IEnumerable<DB_ParentChildRelation> existingRelations = DB_ParentChildRelations
                .Where(relation => relation.Parent.Equals(parent.FlexDB_ID));
            DB_ParentChildRelations.DeleteAllOnSubmit(existingRelations);
            SubmitChanges();
        }

        private void CreateChildRelationsFor(ParentElementBuilder parent)
        {
            foreach (IElementTreeNode eachChild in parent.Children)
            {
                Save(eachChild);
                DB_ParentChildRelation childRelation = new DB_ParentChildRelation
                {
                    Parent = parent.FlexDB_ID,
                    Child = eachChild.FlexDB_ID,
                    Role = (byte)parent.RoleFor(eachChild)
                };
                DB_ParentChildRelations.InsertOnSubmit(childRelation);
            }
            SubmitChanges();
        }
    }
}
