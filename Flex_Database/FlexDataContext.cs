using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleNLG;
using FlexibleRealization;
using Flex.ElementSelectors;

namespace Flex.Database
{
    public partial class FlexDataContext
    {
        public Task<IQueryable<WordElementBuilder>> LoadAllWordBuilders() => Task.Run(() => DB_WordBuilders.Select(dbBuilder => LoadWordBuilder(dbBuilder.ID)));

        private WordElementBuilder LoadWordBuilder(int wordBuilder_ID)
        {
            DB_WordBuilder dbBuilder = DB_WordBuilders
                .Where(builder => builder.ID.Equals(wordBuilder_ID))
                .Single();
            WordElementBuilder result = WordElementBuilder.OfCategory((lexicalCategory)dbBuilder.Lexical_Category);
            if (dbBuilder.SingleWord != null)
            {
                result.WordSource = new SingleWordSource(dbBuilder.SingleWord);
            }
            else
            {
                WeightedWord defaultWord = DB_WeightedWords
                    .Where(dbWeightedWord => dbWeightedWord.Builder == wordBuilder_ID && dbBuilder.DefaultForm == dbWeightedWord.ID)
                    .Select(dbWeightedWord => new WeightedWord(dbWeightedWord.Word, dbWeightedWord.Weight))
                    .Single();
                IEnumerable <WeightedWord> alternates = DB_WeightedWords
                    .Where(dbWeightedWord => dbWeightedWord.Builder == wordBuilder_ID && dbBuilder.DefaultForm != dbWeightedWord.ID)
                    .Select(dbWeightedWord => new WeightedWord(dbWeightedWord.Word, dbWeightedWord.Weight));
                result.WordSource = new WordSelector(defaultWord, alternates);
            }
            return result;
        }

        public void Save(ElementBuilder builder)
        {
            switch (builder)
            {
                case WordElementBuilder wb:
                    Save(wb);
                    break;
                default: break;
            }
        }

        private Task Save(WordElementBuilder word) => Task.Run(() =>
        {
            DB_WordBuilder builderToSave;
            bool isNewBuilder;
            // First we need to make sure we have a WordElementBuilder with a valid FlexDB_ID.
            // We'll need that FlexDB_ID so DB_WeightedWords can refer to it when we save them in the database.
            if (word.FlexDB_ID == 0)        // The word builder does not already exist in the database.  We need toinsert it so it has an ID.
            {
                isNewBuilder = true;
                builderToSave = new DB_WordBuilder
                {
                    Lexical_Category = (byte)word.LexicalCategory
                };
                if (word.WordSource is SingleWordSource sws)
                    builderToSave.SingleWord = sws.GetWord();
                DB_WordBuilders.InsertOnSubmit(builderToSave);
                SubmitChanges();
                // Now we have the ID available from the database.  Assign the ID to the in-memory object, so we'll remember it's not new if we update it
                word.FlexDB_ID = builderToSave.ID;
            }
            else                            // The word builder already exists in the database
            {
                isNewBuilder = false;
                builderToSave = DB_WordBuilders
                    .Where(dbWordBuilder => dbWordBuilder.ID == word.FlexDB_ID)
                    .Single();
            }
            // Regardless of whether the DB_WordBuilder is single-form or multiple-form, set its Lexical_Category
            builderToSave.Lexical_Category = (byte)word.LexicalCategory;
            UpdateDB_WordBuilder(builderToSave);
            // Now we have a WordElementBuilder in the database, and we can deal with any DB_WeightedWords that might refer to it.
            switch (word.WordSource)
            {
                case SingleWordSource sws:
                    // If a WordElementBuilder has only a single word form, then in-memory its WordSource is a SingleWordSource.
                    // In this case we store it to the database with the field SingleWord holding that single word form.
                    // The DB_WordBuilder has its DefaultForm set to null because it has a single word form.
                    builderToSave.SingleWord = sws.GetWord();
                    builderToSave.DefaultForm = null;
                    UpdateDB_WordBuilder(builderToSave);
                    break;
                case WordSelector ws:
                    if (isNewBuilder)
                    {
                        IEnumerable<DB_WeightedWord> newWordsToInsert = ws.GetWeightedWordVariations()
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                Builder = builderToSave.ID,
                                Word = wsVariation.Word,
                                Weight = wsVariation.Weight
                            });
                        DB_WeightedWords.InsertAllOnSubmit(newWordsToInsert);
                        SubmitChanges();
                        // Now that the newly inserted DB_WeightedWords have their IDs assigned, we can figure out which ID to make the Builder's DefaultForm.
                        // If a WordElementBuilder has multiple word forms, then in-memory its WordSource is a WordSelector.
                        // In this case we store it to the database as a collection of DB_WeightedWord that refer to the DB_WordBuilder, and the DB_WordBuilder
                        // has a reference to ONE of those.  The DB_WordBuilder has its SingleWord set to null because it has multiple word forms.
                        builderToSave.DefaultForm = DB_WeightedWords
                            .Where(dbWord => dbWord.Builder == builderToSave.ID && dbWord.Word.Equals(ws.Default.Word))
                            .Single()
                            .ID;
                        builderToSave.SingleWord = null;
                        SubmitChanges();
                    }
                    else  // We're updating a DB_WordBuilder that's already in the database
                    {
                        // The DB_WeightedWords that are already in the database belonging to this DB_WordBuilder
                        IEnumerable<DB_WeightedWord> existingWordsForThisBuilder = DB_WeightedWords
                            .Where(dbWord => dbWord.Builder == builderToSave.ID);
                        // The DB_WeightedWords that are already in the database, but are NOT present in the in-memory WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> existingWordsToDelete = existingWordsForThisBuilder
                            .Where(dbWord => !ws.GetWeightedWordVariations().Any(variation => variation.Word.Equals(dbWord.Word)));
                        DB_WeightedWords.DeleteAllOnSubmit(existingWordsToDelete);
                        // The DB_WeightedWords that are already in the database, AND present in the WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> existingWordsToUpdate = existingWordsForThisBuilder
                            .Join(ws.GetWeightedWordVariations(),
                                existingDBWord => existingDBWord.Word,
                                wsVariation => wsVariation.Word,
                                (existingDBWord, wsVariation) => existingDBWord)
                            .Where(existingDBWord => existingDBWord.Weight != ws.WeightOf(existingDBWord.Word));
                        // Update weights for existing DB_WeightedWords, in case the user changed them in the UI
                        foreach (DB_WeightedWord eachWordToUpdate in existingWordsToUpdate)
                        {
                            eachWordToUpdate.Weight = ws.GetWeightedWordVariations()
                                .Where(wsVariation => wsVariation.Word.Equals(eachWordToUpdate.Word))
                                .Single()
                                .Weight;
                        }
                        // New DB_WeightedWords corresponding to words that are NOT already in the database, but ARE present in the WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> newWordsToInsert = ws.GetWeightedWordVariations()
                            .Where(wsVariation => !existingWordsForThisBuilder.Any(dbWord => dbWord.Word.Equals(wsVariation.Word)))
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                Builder = builderToSave.ID,
                                Word = wsVariation.Word,
                                Weight = wsVariation.Weight
                            });
                        DB_WeightedWords.InsertAllOnSubmit(newWordsToInsert);
                        // Now update the DB_WordBuilder.  Since the in-memory WordElementBuilder has a WordSelector as its WordSource, the database form needs
                        // to have DefaultForm set, and SingleWord NOT set.
                        builderToSave.DefaultForm = existingWordsForThisBuilder.Concat(newWordsToInsert)
                            .Where(dbWord => dbWord.Builder == builderToSave.ID && dbWord.Word.Equals(ws.Default.Word))
                            .Single()
                            .ID;
                        builderToSave.SingleWord = null;
                    }
                    break;
                default: break;
            }
            SubmitChanges();
        });

    }
}
