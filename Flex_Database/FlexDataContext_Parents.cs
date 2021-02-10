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
        // public IEnumerable<DB_Parent> Parents => DB_Parents
        //.Where(dbElement => dbElement.ElementType.Equals(FlexData.ElementType.DB_Parent))
        //.Cast<DB_ParentElement>();

        //private IQueryable<DB_ParentElement> LoadAllDB_ParentElements() => DB_Elements
        //    .Where(dbElement => dbElement.ElementType.Equals(FlexData.ElementType.DB_ParentElement))
        //    .Cast<DB_ParentElement>();

        //public Task<IQueryable<DB_ParentElement>> LoadAllDB_ParentElementsAsync() => Task.Run(() => LoadAllDB_ParentElements());

        //private IQueryable<ParentElementBuilder> LoadAllParents() => LoadAllDB_ParentElements()
        //    .Select(dbElement => LoadParent(dbElement.ID));

        //public Task<IQueryable<ParentElementBuilder>> LoadAllParentsAsync() => Task.Run(() => LoadAllParents());

        //public Task<ParentElementBuilder> LoadParentAsync(int parentBuilder_ID) => Task.Run(() => LoadParent(parentBuilder_ID));

        private ParentElementBuilder LoadParent(int parentBuilder_ID)
        {
            ParentElementBuilder parentBuilder;
            DB_Parent dbParent = DB_Parents.Single(element => element.ID.Equals(parentBuilder_ID));
            parentBuilder = FlexData.Parent.BuilderOfType((FlexData.ParentType)dbParent.ParentType);
            parentBuilder.FlexDB_ID = dbParent.ID;
            LoadParentLayersOf(parentBuilder);
            LoadChildrenOf(parentBuilder);
            return parentBuilder;
        }

        private void LoadParentLayersOf(ParentElementBuilder parentBuilder)
        {
            switch (parentBuilder)
            {
                case IndependentClauseBuilder clauseBuilder:
                    CopyPhraseLayersOf(clauseBuilder, DB_Clauses.Single(clause => clause.ID.Equals(clauseBuilder.FlexDB_ID)));
                    break;
                case SubordinateClauseBuilder clauseBuilder:
                    CopyPhraseLayersOf(clauseBuilder, DB_Clauses.Single(clause => clause.ID.Equals(clauseBuilder.FlexDB_ID)));
                    break;
                case NounPhraseBuilder nounPhraseBuilder:
                    CopyPhraseLayersOf(nounPhraseBuilder, DB_NounPhrases.Single(nounPhrase => nounPhrase.ID.Equals(nounPhraseBuilder.FlexDB_ID)));
                    break;
                case VerbPhraseBuilder verbPhraseBuilder:
                    CopyPhraseLayersOf(verbPhraseBuilder, DB_VerbPhrases.Single(nounPhrase => nounPhrase.ID.Equals(verbPhraseBuilder.FlexDB_ID)));
                    break;
                case AdjectivePhraseBuilder adjectivePhraseBuilder:
                    CopyPhraseLayersOf(adjectivePhraseBuilder, DB_AdjectivePhrases.Single(nounPhrase => nounPhrase.ID.Equals(adjectivePhraseBuilder.FlexDB_ID)));
                    break;
                case AdverbPhraseBuilder adverbPhraseBuilder:
                    CopyPhraseLayersOf(adverbPhraseBuilder, DB_AdverbPhrases.Single(nounPhrase => nounPhrase.ID.Equals(adverbPhraseBuilder.FlexDB_ID)));
                    break;
                case PrepositionalPhraseBuilder prepositionalPhraseBuilder:
                    CopyPhraseLayersOf(prepositionalPhraseBuilder, DB_PrepositionalPhrases.Single(nounPhrase => nounPhrase.ID.Equals(prepositionalPhraseBuilder.FlexDB_ID)));
                    break;
            }
        }

        private void CopyPhraseLayersOf(ClauseBuilder clauseBuilder, IPhrase dbPhrase)
        {
            clauseBuilder.DiscourseFunctionSpecified = dbPhrase.DiscourseFunction != null;
            if (clauseBuilder.DiscourseFunctionSpecified) clauseBuilder.DiscourseFunction = (discourseFunction)dbPhrase.DiscourseFunction;
            clauseBuilder.AppositiveSpecified = dbPhrase.Appositive != null;
            if (clauseBuilder.AppositiveSpecified) clauseBuilder.Appositive = (bool)dbPhrase.Appositive;
            switch (dbPhrase)
            {
                case IClause dbClause:
                    CopyClauseLayerOf(clauseBuilder, dbClause);
                    break;
            }
        }

        private void CopyClauseLayerOf(ClauseBuilder clauseBuilder, IClause dbClause)
        {
            clauseBuilder.AggregateAuxiliarySpecified = dbClause.AggregateAuxiliary != null;
            if (clauseBuilder.AggregateAuxiliarySpecified) clauseBuilder.AggregateAuxiliary = (bool)dbClause.AggregateAuxiliary;
            clauseBuilder.Complementiser = dbClause.Complementizer;
            clauseBuilder.FormSpecified = dbClause.Form != null;
            if (clauseBuilder.FormSpecified) clauseBuilder.Form = (form)dbClause.Form;
            clauseBuilder.InterrogativeTypeSpecified = dbClause.InterrogativeType != null;
            if (clauseBuilder.InterrogativeTypeSpecified) clauseBuilder.InterrogativeType = (interrogativeType)dbClause.InterrogativeType;
            clauseBuilder.Modal = dbClause.Modal;
            clauseBuilder.NegatedSpecified = dbClause.Negated != null;
            if (clauseBuilder.NegatedSpecified) clauseBuilder.Negated = (bool)dbClause.Negated;
            clauseBuilder.PassiveSpecified = dbClause.Passive != null;
            if (clauseBuilder.PassiveSpecified) clauseBuilder.Passive = (bool)dbClause.Passive;
            clauseBuilder.PerfectSpecified = dbClause.Perfect != null;
            if (clauseBuilder.PerfectSpecified) clauseBuilder.Perfect = (bool)dbClause.Perfect;
            clauseBuilder.PersonSpecified = dbClause.Person != null;
            if (clauseBuilder.PersonSpecified) clauseBuilder.Person = (person)dbClause.Person;
            clauseBuilder.ProgressiveSpecified = dbClause.Progressive != null;
            if (clauseBuilder.ProgressiveSpecified) clauseBuilder.Progressive = (bool)dbClause.Progressive;
            clauseBuilder.SuppressGenitiveInGerundSpecified = dbClause.SuppressGenitiveInGerund != null;
            if (clauseBuilder.SuppressGenitiveInGerundSpecified) clauseBuilder.SuppressGenitiveInGerund = (bool)dbClause.SuppressGenitiveInGerund;
            clauseBuilder.SuppressedComplementiserSpecified = dbClause.SuppressedComplementizer != null;
            if (clauseBuilder.SuppressedComplementiserSpecified) clauseBuilder.SuppressedComplementiser = (bool)dbClause.SuppressedComplementizer;
            clauseBuilder.TenseSpecified = dbClause.Tense != null;
            if (clauseBuilder.TenseSpecified) clauseBuilder.Tense = (tense)dbClause.Tense;
        }

        private void CopyPhraseLayersOf(PhraseBuilder phraseBuilder, IPhrase dbPhrase)
        {
            phraseBuilder.DiscourseFunctionSpecified = dbPhrase.DiscourseFunction != null;
            if (phraseBuilder.DiscourseFunctionSpecified) phraseBuilder.DiscourseFunction = (discourseFunction)dbPhrase.DiscourseFunction;
            phraseBuilder.AppositiveSpecified = dbPhrase.Appositive != null;
            if (phraseBuilder.AppositiveSpecified) phraseBuilder.Appositive = (bool)dbPhrase.Appositive;
            switch (dbPhrase)
            {
                case INounPhrase dbNounPhrase:
                    CopyNounPhraseLayerOf((NounPhraseBuilder)phraseBuilder, dbNounPhrase);
                    break;
                case IVerbPhrase dbVerbPhrase:
                    CopyVerbPhraseLayerOf((VerbPhraseBuilder)phraseBuilder, dbVerbPhrase);
                    break;
                case IAdjectivePhrase dbAdjectivePhrase:
                    CopyAdjectivePhraseLayerOf((AdjectivePhraseBuilder)phraseBuilder, dbAdjectivePhrase);
                    break;
                case IAdverbPhrase dbAdverbPhrase:
                    CopyAdverbPhraseLayerOf((AdverbPhraseBuilder)phraseBuilder, dbAdverbPhrase);
                    break;
            }
        }

        private void CopyNounPhraseLayerOf(NounPhraseBuilder nounPhraseBuilder, INounPhrase dbNounPhrase)
        {
            nounPhraseBuilder.AdjectiveOrderingSpecified = dbNounPhrase.AdjectiveOrdering != null;
            if (nounPhraseBuilder.AdjectiveOrderingSpecified) nounPhraseBuilder.AdjectiveOrdering = (bool)dbNounPhrase.AdjectiveOrdering;
            nounPhraseBuilder.ElidedSpecified = dbNounPhrase.Elided != null;
            if (nounPhraseBuilder.ElidedSpecified) nounPhraseBuilder.Elided = (bool)dbNounPhrase.Elided;
            nounPhraseBuilder.NumberSpecified = dbNounPhrase.Number != null;
            if (nounPhraseBuilder.NumberSpecified) nounPhraseBuilder.Number = (numberAgreement)dbNounPhrase.Number;
            nounPhraseBuilder.GenderSpecified = dbNounPhrase.Gender != null;
            if (nounPhraseBuilder.GenderSpecified) nounPhraseBuilder.Gender = (gender)dbNounPhrase.Gender;
            nounPhraseBuilder.PersonSpecified = dbNounPhrase.Person != null;
            if (nounPhraseBuilder.PersonSpecified) nounPhraseBuilder.Person = (person)dbNounPhrase.Person;
            nounPhraseBuilder.PossessiveSpecified = dbNounPhrase.Possessive != null;
            if (nounPhraseBuilder.PossessiveSpecified) nounPhraseBuilder.Possessive = (bool)dbNounPhrase.Possessive;
            nounPhraseBuilder.PronominalSpecified = dbNounPhrase.Pronominal != null;
            if (nounPhraseBuilder.PronominalSpecified) nounPhraseBuilder.Pronominal = (bool)dbNounPhrase.Pronominal;
        }

        private void CopyVerbPhraseLayerOf(VerbPhraseBuilder verbPhraseBuilder, IVerbPhrase dbVerbPhrase)
        {
            verbPhraseBuilder.AggregateAuxiliarySpecified = dbVerbPhrase.AggregateAuxiliary != null;
            if (verbPhraseBuilder.AggregateAuxiliarySpecified) verbPhraseBuilder.AggregateAuxiliary = (bool)dbVerbPhrase.AggregateAuxiliary;
            verbPhraseBuilder.FormSpecified = dbVerbPhrase.Form != null;
            if (verbPhraseBuilder.FormSpecified) verbPhraseBuilder.Form = (form)dbVerbPhrase.Form;
            verbPhraseBuilder.Modal = dbVerbPhrase.Modal;
            verbPhraseBuilder.NegatedSpecified = dbVerbPhrase.Negated != null;
            if (verbPhraseBuilder.NegatedSpecified) verbPhraseBuilder.Negated = (bool)dbVerbPhrase.Negated;
            verbPhraseBuilder.PassiveSpecified = dbVerbPhrase.Passive != null;
            if (verbPhraseBuilder.PassiveSpecified) verbPhraseBuilder.Passive = (bool)dbVerbPhrase.Passive;
            verbPhraseBuilder.PerfectSpecified = dbVerbPhrase.Perfect != null;
            if (verbPhraseBuilder.PerfectSpecified) verbPhraseBuilder.Perfect = (bool)dbVerbPhrase.Perfect;
            verbPhraseBuilder.PersonSpecified = dbVerbPhrase.Person != null;
            if (verbPhraseBuilder.PersonSpecified) verbPhraseBuilder.Person = (person)dbVerbPhrase.Person;
            verbPhraseBuilder.ProgressiveSpecified = dbVerbPhrase.Progressive != null;
            if (verbPhraseBuilder.ProgressiveSpecified) verbPhraseBuilder.Progressive = (bool)dbVerbPhrase.Progressive;
            verbPhraseBuilder.SuppressGenitiveInGerundSpecified = dbVerbPhrase.SuppressGenitiveInGerund != null;
            if (verbPhraseBuilder.SuppressGenitiveInGerundSpecified) verbPhraseBuilder.SuppressGenitiveInGerund = (bool)dbVerbPhrase.SuppressGenitiveInGerund;
            verbPhraseBuilder.SuppressedComplementiserSpecified = dbVerbPhrase.SuppressedComplementizer != null;
            if (verbPhraseBuilder.SuppressedComplementiserSpecified) verbPhraseBuilder.SuppressedComplementiser = (bool)dbVerbPhrase.SuppressedComplementizer;
            verbPhraseBuilder.TenseSpecified = dbVerbPhrase.Tense != null;
            if (verbPhraseBuilder.TenseSpecified) verbPhraseBuilder.Tense = (tense)dbVerbPhrase.Tense;
        }

        private void CopyAdjectivePhraseLayerOf(AdjectivePhraseBuilder adjectivePhraseBuilder, IAdjectivePhrase dbAdjectivePhrase)
        {
            adjectivePhraseBuilder.ComparativeSpecified = dbAdjectivePhrase.Comparative != null;
            if (adjectivePhraseBuilder.ComparativeSpecified) adjectivePhraseBuilder.Comparative = (bool)dbAdjectivePhrase.Comparative;
            adjectivePhraseBuilder.SuperlativeSpecified = dbAdjectivePhrase.Superlative != null;
            if (adjectivePhraseBuilder.SuperlativeSpecified) adjectivePhraseBuilder.Superlative = (bool)dbAdjectivePhrase.Superlative;
        }

        private void CopyAdverbPhraseLayerOf(AdverbPhraseBuilder adverbPhraseBuilder, IAdverbPhrase dbAdverbPhrase)
        {
            adverbPhraseBuilder.ComparativeSpecified = dbAdverbPhrase.Comparative != null;
            if (adverbPhraseBuilder.ComparativeSpecified) adverbPhraseBuilder.Comparative = (bool)dbAdverbPhrase.Comparative;
            adverbPhraseBuilder.SuperlativeSpecified = dbAdverbPhrase.Superlative != null;
            if (adverbPhraseBuilder.SuperlativeSpecified) adverbPhraseBuilder.Superlative = (bool)dbAdverbPhrase.Superlative;
        }

        private void LoadChildrenOf(ParentElementBuilder parent)
        {
            foreach (DB_ParentChildRelation eachRelation in DB_ParentChildRelations.Where(relation => relation.Parent.Equals(parent.FlexDB_ID)))
            {
                ElementBuilder child = Load(eachRelation.Child);
                parent.AddChildWithRole(child, (ParentElementBuilder.ChildRole)eachRelation.Role);
            }
            foreach (DB_ChildOrdering eachDB_Ordering in DB_ChildOrderings.Where(ordering => ordering.Parent.Equals(parent.FlexDB_ID)))
            {
                parent.ChildOrderings.Add(new ParentElementBuilder.ChildOrdering
                {
                    Before = parent.Children.Single(child => child.FlexDB_ID.Equals(eachDB_Ordering.Child_Before)),
                    After = parent.Children.Single(child => child.FlexDB_ID.Equals(eachDB_Ordering.Child_After))
                });
            }
        }

        private void SaveParent(ParentElementBuilder parentBuilder)
        {
            RealizationResult realization = parentBuilder.AsRealizableTree().Realize();
            if (realization.Outcome == RealizationOutcome.Success)
            {
                DB_Element dbSavedElement;
                bool isNewDB_Element = parentBuilder.FlexDB_ID == 0;
                // First we need to make sure we have a ParentElementBuilder with a valid FlexDB_ID.
                // We'll need that FlexDB_ID so DB_ParentChildRelations can refer to it when we save them in the database.
                if (isNewDB_Element)        // The parent builder does not already exist in the database.  We need to insert it so it has an ID.
                {
                    dbSavedElement = new DB_Element(FlexData.ElementType.DB_Parent);
                    dbSavedElement.FormsCount = parentBuilder.CountForms();
                    DB_Elements.InsertOnSubmit(dbSavedElement);
                    SubmitChanges();
                    // Now we have the ID available from the database.  Assign the ID to the in-memory object, so we'll remember it's not new if we update it
                    parentBuilder.FlexDB_ID = dbSavedElement.ID;
                }
                else    // The parent builder already exists in the database
                {
                    dbSavedElement = DB_Elements.Single(dbElement => dbElement.ID.Equals(parentBuilder.FlexDB_ID));
                    dbSavedElement.FormsCount = parentBuilder.CountForms();
                }
                UpdateParentLayersFor(parentBuilder, realization);
                DeleteChildOrderingsFor(parentBuilder);
                DeleteChildRelationsFor(parentBuilder);
                CreateChildRelationsFor(parentBuilder);
                CreateChildOrderingsFor(parentBuilder);
                SubmitChanges();
                OnParentChanged(parentBuilder.FlexDB_ID);
            }
        }

        private void UpdateParentLayersFor(ParentElementBuilder parentBuilder, RealizationResult realization)
        {
            LayerParent dbSavedParentLayer;
            LayerParent existingParentLayer = LayerParents.FirstOrDefault(parentLayer => parentLayer.ID.Equals(parentBuilder.FlexDB_ID));
            if (existingParentLayer != null)
                dbSavedParentLayer = existingParentLayer;
            else
            {
                dbSavedParentLayer = new LayerParent(FlexData.Parent.TypeOf(parentBuilder));
                dbSavedParentLayer.ID = parentBuilder.FlexDB_ID;
                LayerParents.InsertOnSubmit(dbSavedParentLayer);
            }
            dbSavedParentLayer.DefaultRealization = realization.Text;
            switch (parentBuilder)
            {
                case ClauseBuilder clause:
                    UpdatePhraseLayerFor(clause);
                    UpdateClauseLayerFor(clause);
                    break;
                case PhraseBuilder phrase:
                    UpdatePhraseLayersFor(phrase);
                    break;
                default: break;
            }
        }

        private void UpdatePhraseLayerFor(ClauseBuilder clauseBuilder)
        {
            LayerPhrase dbSavedPhraseLayer;
            LayerPhrase existingPhraseLayer = LayerPhrases.FirstOrDefault(phraseLayer => phraseLayer.ID.Equals(clauseBuilder.FlexDB_ID));
            if (existingPhraseLayer != null)
                dbSavedPhraseLayer = existingPhraseLayer;
            else
            {
                dbSavedPhraseLayer = new LayerPhrase { ID = clauseBuilder.FlexDB_ID };
                LayerPhrases.InsertOnSubmit(dbSavedPhraseLayer);
            }
            dbSavedPhraseLayer.DiscourseFunction = clauseBuilder.DiscourseFunctionSpecified ? (byte)clauseBuilder.DiscourseFunction : null;
            dbSavedPhraseLayer.Appositive = clauseBuilder.AppositiveSpecified ? clauseBuilder.Appositive : null;
            UpdateLayerPhrase(dbSavedPhraseLayer);
        }

        private void UpdatePhraseLayersFor(PhraseBuilder phraseBuilder)
        {
            LayerPhrase dbSavedPhraseLayer;
            LayerPhrase existingPhraseLayer = LayerPhrases.FirstOrDefault(phraseLayer => phraseLayer.ID.Equals(phraseBuilder.FlexDB_ID));
            if (existingPhraseLayer != null)
                dbSavedPhraseLayer = existingPhraseLayer;
            else
            {
                dbSavedPhraseLayer = new LayerPhrase { ID = phraseBuilder.FlexDB_ID };
                LayerPhrases.InsertOnSubmit(dbSavedPhraseLayer);
            }
            dbSavedPhraseLayer.DiscourseFunction = phraseBuilder.DiscourseFunctionSpecified ? (byte)phraseBuilder.DiscourseFunction : null;
            dbSavedPhraseLayer.Appositive = phraseBuilder.AppositiveSpecified ? phraseBuilder.Appositive : null;
            UpdateLayerPhrase(dbSavedPhraseLayer);
            switch (phraseBuilder)
            {
                case NounPhraseBuilder nounPhrase:
                    UpdateNounPhraseLayerFor(nounPhrase);
                    break;
                case VerbPhraseBuilder verbPhrase:
                    UpdateVerbPhraseLayerFor(verbPhrase);
                    break;
                case AdjectivePhraseBuilder adjectivePhrase:
                    UpdateAdjectivePhraseLayerFor(adjectivePhrase);
                    break;
                case AdverbPhraseBuilder adverbPhrase:
                    UpdateAdverbPhraseLayerFor(adverbPhrase);
                    break;
                default: break;
            }
        }

        private void UpdateClauseLayerFor(ClauseBuilder clauseBuilder)
        {
            LayerClause dbSavedClauseLayer;
            LayerClause existingClauseLayer = LayerClauses.FirstOrDefault(clauseLayer => clauseLayer.ID.Equals(clauseBuilder.FlexDB_ID));
            if (existingClauseLayer != null)
                dbSavedClauseLayer = existingClauseLayer;
            else
            {
                dbSavedClauseLayer = new LayerClause { ID = clauseBuilder.FlexDB_ID };
                LayerClauses.InsertOnSubmit(dbSavedClauseLayer);
            }
            dbSavedClauseLayer.AggregateAuxiliary = clauseBuilder.AggregateAuxiliarySpecified ? clauseBuilder.AggregateAuxiliary : null;
            dbSavedClauseLayer.Complementizer = clauseBuilder.Complementiser;
            dbSavedClauseLayer.Form = clauseBuilder.FormSpecified ? (byte)clauseBuilder.Form : null;
            dbSavedClauseLayer.InterrogativeType = clauseBuilder.InterrogativeTypeSpecified ? (byte)clauseBuilder.InterrogativeType : null;
            dbSavedClauseLayer.Modal = clauseBuilder.Modal;
            dbSavedClauseLayer.Negated = clauseBuilder.NegatedSpecified ? clauseBuilder.Negated : null;
            dbSavedClauseLayer.Passive = clauseBuilder.PassiveSpecified ? clauseBuilder.Passive : null;
            dbSavedClauseLayer.Perfect = clauseBuilder.PerfectSpecified ? clauseBuilder.Perfect : null;
            dbSavedClauseLayer.Person = clauseBuilder.PersonSpecified ? (byte)clauseBuilder.Person : null;
            dbSavedClauseLayer.Progressive = clauseBuilder.ProgressiveSpecified ? clauseBuilder.Progressive : null;
            dbSavedClauseLayer.SuppressGenitiveInGerund = clauseBuilder.SuppressGenitiveInGerundSpecified ? clauseBuilder.SuppressGenitiveInGerund : null;
            dbSavedClauseLayer.SuppressedComplementizer = clauseBuilder.SuppressedComplementiserSpecified ? clauseBuilder.SuppressedComplementiser : null;
            UpdateLayerClause(dbSavedClauseLayer);
        }

        private void UpdateNounPhraseLayerFor(NounPhraseBuilder nounPhraseBuilder)
        {
            LayerNounPhrase dbSavedNounPhraseLayer;
            LayerNounPhrase existingNounPhraseLayer = LayerNounPhrases.FirstOrDefault(nounPhraseLayer => nounPhraseLayer.ID.Equals(nounPhraseBuilder.FlexDB_ID));
            if (existingNounPhraseLayer != null)
                dbSavedNounPhraseLayer = existingNounPhraseLayer;
            else
            {
                dbSavedNounPhraseLayer = new LayerNounPhrase { ID = nounPhraseBuilder.FlexDB_ID };
                LayerNounPhrases.InsertOnSubmit(dbSavedNounPhraseLayer);
            }
            dbSavedNounPhraseLayer.AdjectiveOrdering = nounPhraseBuilder.AdjectiveOrderingSpecified ? nounPhraseBuilder.AdjectiveOrdering : null;
            dbSavedNounPhraseLayer.Elided = nounPhraseBuilder.ElidedSpecified ? nounPhraseBuilder.Elided : null;
            dbSavedNounPhraseLayer.Number = nounPhraseBuilder.NumberSpecified ? (byte)nounPhraseBuilder.Number : null;
            dbSavedNounPhraseLayer.Gender = nounPhraseBuilder.GenderSpecified ? (byte)nounPhraseBuilder.Gender : null;
            dbSavedNounPhraseLayer.Person = nounPhraseBuilder.PersonSpecified ? (byte)nounPhraseBuilder.Person : null;
            dbSavedNounPhraseLayer.Possessive = nounPhraseBuilder.PossessiveSpecified ? nounPhraseBuilder.Possessive : null;
            dbSavedNounPhraseLayer.Pronominal = nounPhraseBuilder.PronominalSpecified ? nounPhraseBuilder.Pronominal : null;
            UpdateLayerNounPhrase(dbSavedNounPhraseLayer);
        }

        private void UpdateVerbPhraseLayerFor(VerbPhraseBuilder verbPhraseBuilder)
        {
            LayerVerbPhrase dbSavedVerbPhraseLayer;
            LayerVerbPhrase existingVerbPhraseLayer = LayerVerbPhrases.FirstOrDefault(verbPhraseLayer => verbPhraseLayer.ID.Equals(verbPhraseBuilder.FlexDB_ID));
            if (existingVerbPhraseLayer != null)
                dbSavedVerbPhraseLayer = existingVerbPhraseLayer;
            else
            {
                dbSavedVerbPhraseLayer = new LayerVerbPhrase { ID = verbPhraseBuilder.FlexDB_ID };
                LayerVerbPhrases.InsertOnSubmit(dbSavedVerbPhraseLayer);
            }
            dbSavedVerbPhraseLayer.AggregateAuxiliary = verbPhraseBuilder.AggregateAuxiliarySpecified ? verbPhraseBuilder.AggregateAuxiliary : null;
            dbSavedVerbPhraseLayer.Form = verbPhraseBuilder.FormSpecified ? (byte)verbPhraseBuilder.Form : null;
            dbSavedVerbPhraseLayer.Modal = verbPhraseBuilder.Modal;
            dbSavedVerbPhraseLayer.Negated = verbPhraseBuilder.NegatedSpecified ? verbPhraseBuilder.Negated : null;
            dbSavedVerbPhraseLayer.Passive = verbPhraseBuilder.PassiveSpecified ? verbPhraseBuilder.Passive : null;
            dbSavedVerbPhraseLayer.Perfect = verbPhraseBuilder.PerfectSpecified ? verbPhraseBuilder.Perfect : null;
            dbSavedVerbPhraseLayer.Person = verbPhraseBuilder.PersonSpecified ? (byte)verbPhraseBuilder.Person : null;
            dbSavedVerbPhraseLayer.Progressive = verbPhraseBuilder.ProgressiveSpecified ? verbPhraseBuilder.Progressive : null;
            dbSavedVerbPhraseLayer.SuppressGenitiveInGerund = verbPhraseBuilder.SuppressGenitiveInGerundSpecified ? verbPhraseBuilder.SuppressGenitiveInGerund : null;
            dbSavedVerbPhraseLayer.SuppressedComplementizer = verbPhraseBuilder.SuppressedComplementiserSpecified ? verbPhraseBuilder.SuppressedComplementiser : null;
            UpdateLayerVerbPhrase(dbSavedVerbPhraseLayer);
        }

        private void UpdateAdjectivePhraseLayerFor(AdjectivePhraseBuilder adjectivePhraseBuilder)
        {
            LayerAdjectivePhrase dbSavedAdjectivePhraseLayer;
            LayerAdjectivePhrase existingAdjectivePhraseLayer = LayerAdjectivePhrases.FirstOrDefault(adjectivePhraseLayer => adjectivePhraseLayer.ID.Equals(adjectivePhraseBuilder.FlexDB_ID));
            if (existingAdjectivePhraseLayer != null)
                dbSavedAdjectivePhraseLayer = existingAdjectivePhraseLayer;
            else
            {
                dbSavedAdjectivePhraseLayer = new LayerAdjectivePhrase { ID = adjectivePhraseBuilder.FlexDB_ID };
                LayerAdjectivePhrases.InsertOnSubmit(dbSavedAdjectivePhraseLayer);
            }
            dbSavedAdjectivePhraseLayer.Comparative = adjectivePhraseBuilder.ComparativeSpecified ? adjectivePhraseBuilder.Comparative : null;
            dbSavedAdjectivePhraseLayer.Superlative = adjectivePhraseBuilder.SuperlativeSpecified ? adjectivePhraseBuilder.Superlative : null;
            UpdateLayerAdjectivePhrase(dbSavedAdjectivePhraseLayer);
        }

        private void UpdateAdverbPhraseLayerFor(AdverbPhraseBuilder adverbPhraseBuilder)
        {
            LayerAdverbPhrase dbSavedAdverbPhraseLayer;
            LayerAdverbPhrase existingAdverbPhraseLayer = LayerAdverbPhrases.FirstOrDefault(adverbPhraseLayer => adverbPhraseLayer.ID.Equals(adverbPhraseBuilder.FlexDB_ID));
            if (existingAdverbPhraseLayer != null)
                dbSavedAdverbPhraseLayer = existingAdverbPhraseLayer;
            else
            {
                dbSavedAdverbPhraseLayer = new LayerAdverbPhrase { ID = adverbPhraseBuilder.FlexDB_ID };
                LayerAdverbPhrases.InsertOnSubmit(dbSavedAdverbPhraseLayer);
            }
            dbSavedAdverbPhraseLayer.Comparative = adverbPhraseBuilder.ComparativeSpecified ? adverbPhraseBuilder.Comparative : null;
            dbSavedAdverbPhraseLayer.Superlative = adverbPhraseBuilder.SuperlativeSpecified ? adverbPhraseBuilder.Superlative : null;
            UpdateLayerAdverbPhrase(dbSavedAdverbPhraseLayer);
        }

        private void CopyDataToDB_ParentElement(DB_Parent dbParentElement, ParentElementBuilder parent, string defaultRealization)
        {
            //dbParentElement.ParentType = (byte)FlexData.Parent.TypeOf(parent);
            //dbParentElement.ParentDefaultRealization = defaultRealization;
            //switch (parent)
            //{
            //    case ClauseBuilder clause:
            //        dbParentElement.DiscourseFunction = clause.DiscourseFunctionSpecified ? (byte)clause.DiscourseFunction : null;
            //        dbParentElement.Appositive = clause.AppositiveSpecified ? clause.Appositive : null;
            //        CopyClauseDataToDB_ParentElement(dbParentElement, clause);
            //        break;
            //    case PhraseBuilder phrase:
            //        dbParentElement.DiscourseFunction = phrase.DiscourseFunctionSpecified ? (byte)phrase.DiscourseFunction : null;
            //        dbParentElement.Appositive = phrase.AppositiveSpecified ? phrase.Appositive : null;
            //        CopyPhraseDataToDB_ParentElement(dbParentElement, phrase);
            //        break;
            //    case CompoundBuilder:
            //    case NominalModifierBuilder:
            //        break;
            //    default: throw new InvalidOperationException("Can't save this type of ParentElementBuilder to the Flex database");
            //}
        }

        private void DeleteChildRelationsFor(ParentElementBuilder parent)
        {
            DB_ParentChildRelations.DeleteAllOnSubmit(DB_ParentChildRelations
                .Where(relation => relation.Parent.Equals(parent.FlexDB_ID)));
            //SubmitChanges();
        }

        private void DeleteChildOrderingsFor(ParentElementBuilder parent)
        {
            DB_ChildOrderings.DeleteAllOnSubmit(DB_ChildOrderings
                .Where(ordering => ordering.Parent.Equals(parent.FlexDB_ID)));
            //SubmitChanges();
        }

        private void CreateChildOrderingsFor(ParentElementBuilder parent)
        {
            foreach (ParentElementBuilder.ChildOrdering eachOrdering in parent.ChildOrderings)
            {
                DB_ChildOrdering ordering = new DB_ChildOrdering
                {
                    Parent = parent.FlexDB_ID,
                    Child_Before = eachOrdering.Before.FlexDB_ID,
                    Child_After = eachOrdering.After.FlexDB_ID
                };
                DB_ChildOrderings.InsertOnSubmit(ordering);
            }
            //SubmitChanges();
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
            //SubmitChanges();
        }
    }
}
