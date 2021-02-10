using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FlexibleRealization;
using Flex.ElementSelectors;
using SimpleNLG;

namespace Flex.Database
{
    public partial class FlexDataContext
    {
        //public IEnumerable<DB_Word> Words => DB_Elements
        //    .Where(dbElement => dbElement.ElementType.Equals(FlexData.ElementType.DB_Word))
        //    .Cast<DB_WordElement>();

        //public Task<WordElementBuilder> LoadWordAsync(int wordBuilder_ID) => Task.Run(() => LoadWord(wordBuilder_ID));

        //public Task<IQueryable<WordElementBuilder>> LoadAllWordsAsync() => Task.Run(() => LoadAllWords());

        public Task SaveWordAsync(DB_Word dbWord, DB_WeightedWord defaultWeightedWord, IEnumerable<DB_WeightedWord> alternateWeightedWords) => Task.Run(() =>
        {
            //using (TransactionScope transaction = new TransactionScope())
            //{
            //    SaveWord(dbWord, defaultWeightedWord, alternateWeightedWords);
            //}
            //OnWordChanged(dbWord.ID);
        });

        //private IQueryable<WordElementBuilder> LoadAllWords() => DB_Elements
        //    .Where(dbElement => dbElement.ElementType.Equals(FlexData.ElementType.DB_Word))
        //    .Select(dbBuilder => LoadWord(dbBuilder.ID));

        private WordElementBuilder LoadWord(int wordBuilder_ID)
        {
            WordElementBuilder wordBuilder;
            DB_Word dbWord = DB_Words.Single(element => element.ID.Equals(wordBuilder_ID));
            wordBuilder = FlexData.Word.BuilderOfType((FlexData.WordType)dbWord.WordType);
            wordBuilder.FlexDB_ID = wordBuilder_ID;
            LoadWordLayersOf(wordBuilder);
            Task.Run(() =>
            {
                WeightedWord defaultWeightedWord = DB_WeightedWords
                    .Where(dbWeightedWord => dbWeightedWord.WordElement.Equals(wordBuilder_ID) && dbWord.DefaultWeightedWord.Equals(dbWeightedWord.ID))
                    .Select(dbWeightedWord => new WeightedWord(dbWeightedWord.Text, dbWeightedWord.Weight))
                    .Single();
                IEnumerable<WeightedWord> alternates = DB_WeightedWords
                    .Where(dbWeightedWord => dbWeightedWord.WordElement.Equals(wordBuilder_ID) && !dbWord.DefaultWeightedWord.Equals(dbWeightedWord.ID))
                    .Select(dbWeightedWord => new WeightedWord(dbWeightedWord.Text, dbWeightedWord.Weight));
                if (alternates.Count().Equals(0))
                    wordBuilder.WordSource = new SingleWordSource(defaultWeightedWord.Text);
                else
                    wordBuilder.WordSource = new WordSelector(defaultWeightedWord, alternates);
            });
            return wordBuilder;
        }

        private void LoadWordLayersOf(WordElementBuilder wordBuilder)
        {
            LayerWord wordLayer = LayerWords.Single(wordLayer => wordLayer.ID.Equals(wordBuilder.FlexDB_ID));
            wordBuilder.ExpletiveSubjectSpecified = wordLayer.ExpletiveSubject != null;
            if (wordBuilder.ExpletiveSubjectSpecified) wordBuilder.ExpletiveSubject = (bool)wordLayer.ExpletiveSubject;
            wordBuilder.ProperSpecified = wordLayer.Proper != null;
            if (wordBuilder.ProperSpecified) wordBuilder.Proper = (bool)wordLayer.Proper;
            wordBuilder.InflectionSpecified = wordLayer.Inflection != null;
            if (wordBuilder.InflectionSpecified) wordBuilder.Inflection = (inflection)wordLayer.Inflection;
            wordBuilder.CannedSpecified = wordLayer.Canned != null;
            if (wordBuilder.CannedSpecified) wordBuilder.Canned = (bool)wordLayer.Canned;
            switch (wordBuilder)
            {
                case PronounBuilder pronounBuilder:
                    LoadPronounLayerOf(pronounBuilder);
                    break;
            }
        }

        private void LoadPronounLayerOf(PronounBuilder pronounBuilder)
        {
            LayerPronoun pronounLayer = LayerPronouns.Single(pronounLayer => pronounLayer.ID.Equals(pronounBuilder.FlexDB_ID));
            pronounBuilder.CaseSpecified = pronounLayer.PronounCase != null;
            if (pronounBuilder.CaseSpecified) pronounBuilder.Case = (PronounCase)pronounLayer.PronounCase;
            pronounBuilder.PersonSpecified = pronounLayer.Person != null;
            if (pronounBuilder.PersonSpecified) pronounBuilder.Person = (person)pronounLayer.Person;
            pronounBuilder.NumberSpecified = pronounLayer.Number != null;
            if (pronounBuilder.NumberSpecified) pronounBuilder.Number = (numberAgreement)pronounLayer.Number;
            pronounBuilder.GenderSpecified = pronounLayer.Gender != null;
            if (pronounBuilder.GenderSpecified) pronounBuilder.Gender = (gender)pronounLayer.Gender;
        }

        private void SaveWord(WordElementBuilder wordBuilder)
        {
            DB_Element dbSavedElement;
            bool isNewDB_Element = wordBuilder.FlexDB_ID == 0;
            // If the word doesn't support variations, and we've already saved a matching DB_Word in the database, don't save another one
            if (!wordBuilder.SupportsVariations)
            {
                DB_Word existingDB_WordThatMatches = DB_Words
                    .Where(dbWord => dbWord.WordType.Equals(FlexData.Word.TypeOf(wordBuilder)))
                    .FirstOrDefault(wordElement => wordElement.DefaultForm.Equals(wordBuilder.WordSource.DefaultWord));
                if (existingDB_WordThatMatches != null)
                {
                    wordBuilder.FlexDB_ID = existingDB_WordThatMatches.ID;
                    return;
                }
            }
            // Make sure we have a WordElementBuilder with a valid FlexDB_ID.
            // We'll need that FlexDB_ID so DB_WeightedWords can refer to it when we save them in the database.
            if (isNewDB_Element)        // The word builder does not already exist in the database.  We need to insert it so it has an ID.
            {
                dbSavedElement = new DB_Element(FlexData.ElementType.DB_Word);
                dbSavedElement.FormsCount = wordBuilder.CountForms();
                DB_Elements.InsertOnSubmit(dbSavedElement);
                SubmitChanges();
                // Now we have the ID available from the database.  Assign the ID to the in-memory object, so we'll remember it's not new if we update it
                wordBuilder.FlexDB_ID = dbSavedElement.ID;
            }
            else  // The word builder already exists in the database
            {
                dbSavedElement = DB_Elements.Single(dbElement => dbElement.ID.Equals(wordBuilder.FlexDB_ID));
                dbSavedElement.FormsCount = wordBuilder.CountForms();
            }
            UpdateWordLayersFor(wordBuilder);
            SubmitChanges();
            OnWordChanged(wordBuilder.FlexDB_ID);
        }

        private void UpdateWordLayersFor(WordElementBuilder wordBuilder)
        {
            LayerWord dbSavedWordLayer;
            bool isNewLayerWord;
            LayerWord existingWordLayer = LayerWords.FirstOrDefault(wordLayer => wordLayer.ID.Equals(wordBuilder.FlexDB_ID));
            if (existingWordLayer != null)
            {
                isNewLayerWord = false;
                dbSavedWordLayer = existingWordLayer;
            }
            else
            {
                isNewLayerWord = true;
                dbSavedWordLayer = new LayerWord(wordBuilder.FlexDB_ID, FlexData.Word.TypeOf(wordBuilder));
                LayerWords.InsertOnSubmit(dbSavedWordLayer);
            }                
            dbSavedWordLayer.ExpletiveSubject = wordBuilder.ExpletiveSubjectSpecified ? wordBuilder.ExpletiveSubject : null;
            dbSavedWordLayer.Proper = wordBuilder.ProperSpecified ? wordBuilder.Proper : null;
            dbSavedWordLayer.Inflection = wordBuilder.InflectionSpecified ? (byte)wordBuilder.Inflection : null;
            dbSavedWordLayer.Canned = wordBuilder.CannedSpecified ? wordBuilder.Canned : null;
            UpdateWeightedWordsFor(wordBuilder, dbSavedWordLayer, isNewLayerWord);
            UpdateLayerWord(dbSavedWordLayer);
            switch (wordBuilder)
            {
                case PronounBuilder pronoun:
                    UpdatePronounLayerFor(pronoun);
                    break;
                default: break;
            }
        }

        private void UpdatePronounLayerFor(PronounBuilder pronounBuilder)
        {
            LayerPronoun dbSavedPronounLayer;
            LayerPronoun existingPronounLayer = LayerPronouns.FirstOrDefault(pronounLayer => pronounLayer.ID.Equals(pronounBuilder.FlexDB_ID));
            if (existingPronounLayer != null)
                dbSavedPronounLayer = existingPronounLayer;
            else
            {
                dbSavedPronounLayer = new LayerPronoun { ID = pronounBuilder.FlexDB_ID };
                LayerPronouns.InsertOnSubmit(dbSavedPronounLayer);
            }
            dbSavedPronounLayer.PronounCase = pronounBuilder.CaseSpecified ? (byte)pronounBuilder.Case : null;
            dbSavedPronounLayer.Person = pronounBuilder.PersonSpecified ? (byte)pronounBuilder.Person : null;
            dbSavedPronounLayer.Number = pronounBuilder.NumberSpecified ? (byte)pronounBuilder.Number : null;
            dbSavedPronounLayer.Gender = pronounBuilder.GenderSpecified ? (byte)pronounBuilder.Gender : null;
        }

        private void UpdateWeightedWordsFor(WordElementBuilder wordBuilder, LayerWord wordLayer, bool isNewLayerWord)
        {
            // The DB_WeightedWords that are already in the database belonging to this word
            IEnumerable<DB_WeightedWord> existingWeightedWordsForThisBuilder = DB_WeightedWords
                .Where(dbWeightedWord => dbWeightedWord.WordElement.Equals(wordBuilder.FlexDB_ID));

            switch (wordBuilder.WordSource)
            {
                case SingleWordSource sws:
                    // If a WordElementBuilder has only a single word form, then in-memory its WordSource is a SingleWordSource.
                    DB_WeightedWord existingDefaultWeightedWord = existingWeightedWordsForThisBuilder
                        .FirstOrDefault(dbWeightedWord => dbWeightedWord.Text.Equals(sws.GetWord()));
                    DB_WeightedWord dbSavedDefaultWeightedWord;
                    if (existingDefaultWeightedWord == null)
                    {
                        dbSavedDefaultWeightedWord = new DB_WeightedWord
                        {
                            WordElement = wordBuilder.FlexDB_ID,
                            Text = sws.GetWord()
                        };
                        DB_WeightedWords.InsertOnSubmit(dbSavedDefaultWeightedWord);
                        SubmitChanges();
                    }
                    else
                    {
                        dbSavedDefaultWeightedWord = existingDefaultWeightedWord;
                        dbSavedDefaultWeightedWord.Text = sws.GetWord();
                        UpdateDB_WeightedWord(dbSavedDefaultWeightedWord);
                    }
                    wordLayer.DefaultWeightedWord = dbSavedDefaultWeightedWord.ID;
                    DB_WeightedWords.DeleteAllOnSubmit(existingWeightedWordsForThisBuilder
                        .Where(dbWeightedWord => dbWeightedWord != dbSavedDefaultWeightedWord));
                    break;
                case WordSelector ws:
                    if (isNewLayerWord)
                    {
                        DB_WeightedWords.InsertAllOnSubmit(ws.GetWeightedWordVariations()
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                WordElement = wordBuilder.FlexDB_ID,
                                Text = wsVariation.Text,
                                Weight = wsVariation.Weight
                            }));
                        SubmitChanges();
                        // Now that the newly inserted DB_WeightedWords have their IDs assigned, we can figure out which ID to make the Builder's DefaultForm.
                        // If a WordElementBuilder has multiple word forms, then in-memory its WordSource is a WordSelector.
                        // In this case we store it to the database as a collection of DB_WeightedWord that refer to the DB_WordBuilder, and the DB_WordBuilder
                        // has a reference to ONE of those.  The DB_WordBuilder has its SingleWord set to null because it has multiple word forms.
                        wordLayer.DefaultWeightedWord = DB_WeightedWords
                            .Single(dbWeightedWord => dbWeightedWord.WordElement.Equals(wordBuilder.FlexDB_ID) && dbWeightedWord.Text.Equals(ws.Default.Text))
                            .ID;
                    }
                    else  // We're updating a DB_WordBuilder that's already in the database
                    {
                        DB_WeightedWords.DeleteAllOnSubmit(existingWeightedWordsForThisBuilder
                            .Where(dbWeightedWord => !ws.GetWeightedWordVariations().Any(variation => variation.Text.Equals(dbWeightedWord.Text))));
                        IEnumerable<DB_WeightedWord> existingWeightedWordsToUpdate = existingWeightedWordsForThisBuilder
                            .Join(ws.GetWeightedWordVariations(),
                                existingDBWeightedWord => existingDBWeightedWord.Text,
                                wsVariation => wsVariation.Text,
                                (existingDBWeightedWord, wsVariation) => existingDBWeightedWord)
                            .Where(existingDBWeightedWord => existingDBWeightedWord.Weight != ws.WeightOf(existingDBWeightedWord.Text));
                        // Update weights for existing DB_WeightedWords, in case the user changed them in the UI
                        foreach (DB_WeightedWord eachWeightedWordToUpdate in existingWeightedWordsToUpdate)
                        {
                            eachWeightedWordToUpdate.Weight = ws.GetWeightedWordVariations()
                                .Single(wsVariation => wsVariation.Text.Equals(eachWeightedWordToUpdate.Text))
                                .Weight;
                        }
                        // New DB_WeightedWords corresponding to words that are NOT already in the database, but ARE present in the WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> newWeightedWordsToInsert = ws.GetWeightedWordVariations()
                            .Where(wsVariation => !existingWeightedWordsForThisBuilder.Any(dbWeightedWord => dbWeightedWord.Text.Equals(wsVariation.Text)))
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                WordElement = wordBuilder.FlexDB_ID,
                                Text = wsVariation.Text,
                                Weight = wsVariation.Weight
                            });
                        DB_WeightedWords.InsertAllOnSubmit(newWeightedWordsToInsert);
                        SubmitChanges();  // After doing this, we have valid IDs for the DB_WeightedWords
                        // Now update the DB_WordBuilder.  Since the in-memory WordElementBuilder has a WordSelector as its WordSource, the database form needs
                        // to have DefaultForm set, and SingleWord NOT set.
                        wordLayer.DefaultWeightedWord = existingWeightedWordsForThisBuilder.Concat(newWeightedWordsToInsert)
                            .Single(dbWeightedWord => dbWeightedWord.WordElement == wordBuilder.FlexDB_ID && dbWeightedWord.Text.Equals(ws.Default.Text))
                            .ID;
                    }
                    break;
                default: break;
            }
        }

        //private void SaveWord(DB_Word dbWord, DB_WeightedWord defaultWeightedWord, IEnumerable<DB_WeightedWord> alternateWeightedWords)
        //{
        //    if (!FlexData.Word.SupportsVariations(dbWordElement)) // If the word doesn't support variations, and we've already saved a matching DB_WordElement in the database, don't save another one
        //    {
        //        DB_WordElement existingDB_WordElementThatMatches = DB_Elements
        //            .Where(element => element.ElementType.Equals(FlexData.ElementType.DB_Word))
        //            .Cast<DB_WordElement>()
        //            .Where(wordElement => wordElement.WordType.Equals(dbWordElement.WordType))
        //            .FirstOrDefault(WordElement => WordElement.SingleWord.Equals(dbWordElement.SingleWord));
        //        if (existingDB_WordElementThatMatches != null) return;
        //    }
        //    // Make sure we have a DB_WordElement with a valid FlexDB_ID.
        //    // We'll need that ID so DB_WeightedWords can refer to it when we save them in the database.
        //    if (dbWordElement.ID == 0)        // The word builder does not already exist in the database.  We need to insert it so it has an ID.
        //    {
        //        DB_Elements.InsertOnSubmit(dbWordElement);
        //        SubmitChanges();
        //    }
        //    else UpdateDB_Element(dbWordElement);
        //    // Now we have the DB_WordElement in the database, and we can deal with any DB_WeightedWords that might refer to it.
        //    if (defaultWeightedWord != null)
        //    {
        //        defaultWeightedWord.WordElement = dbWordElement.ID;
        //        if (defaultWeightedWord.ID == 0)
        //        {
        //            DB_WeightedWords.InsertOnSubmit(defaultWeightedWord);
        //            SubmitChanges();
        //        }
        //        else UpdateDB_WeightedWord(defaultWeightedWord);
        //        dbWordElement.DefaultWeightedWord = defaultWeightedWord.ID;
        //        UpdateDB_Element(dbWordElement);
        //    }
        //    // The DB_WeightedWords that are already in the database belonging to this DB_WordBuilder
        //    IEnumerable<DB_WeightedWord> existingWeightedWordsForThisWordElement = DB_WeightedWords
        //        .Where(dbWeightedWord => dbWeightedWord.WordElement == dbWordElement.ID);
        //    // The DB_WeightedWords that are already in the database, but are NOT in the alternates collection
        //    IEnumerable<DB_WeightedWord> existingWeightedWordsToDelete = existingWeightedWordsForThisWordElement
        //        .Where(dbWeightedWord => defaultWeightedWord != dbWeightedWord && !alternateWeightedWords.Contains(dbWeightedWord));
        //    DB_WeightedWords.DeleteAllOnSubmit(existingWeightedWordsToDelete);
        //    // The DB_WeightedWords that are already in the database, AND present in the alternates collection
        //    IEnumerable<DB_WeightedWord> existingWeightedWordsToUpdate = existingWeightedWordsForThisWordElement
        //        .Intersect(alternateWeightedWords);
        //    foreach (DB_WeightedWord eachWeightedWordToUpdate in existingWeightedWordsToUpdate)
        //        UpdateDB_WeightedWord(eachWeightedWordToUpdate);
        //    // New DB_WeightedWords that are NOT already in the database, but ARE present in the alternates collection
        //    IEnumerable<DB_WeightedWord> newWeightedWordsToInsert = alternateWeightedWords
        //        .Where(dbWeightedWord => dbWeightedWord.ID == 0);
        //    foreach (DB_WeightedWord eachWeightedWordToInsert in newWeightedWordsToInsert)
        //        eachWeightedWordToInsert.WordElement = dbWordElement.ID;
        //    DB_WeightedWords.InsertAllOnSubmit(newWeightedWordsToInsert);
        //    SubmitChanges();
        //    OnWordChanged(dbWordElement.ID);
        //}

    }
}
