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
        public IQueryable<WordElementBuilder> LoadAllWords() => DB_Elements
            .Where(dbElement => dbElement.ElementType.Equals(FlexData.ElementType.DB_WordElement))
            .Select(dbBuilder => LoadWord(dbBuilder.ID));

        public Task<IQueryable<WordElementBuilder>> LoadAllWordsAsync() => Task.Run(() => LoadAllWords());

        private WordElementBuilder LoadWord(int wordBuilder_ID)
        {
            WordElementBuilder result;
            // Before loading from the database, see if it's already in the cache
            if (!FlexData.Word.Cache.TryGetValue(wordBuilder_ID, out result))
            {
                DB_WordElement dbElement = DB_Elements
                    .Where(builder => builder.ID.Equals(wordBuilder_ID))
                    .Cast<DB_WordElement>()
                    .Single();
                result = FlexData.Word.ElementOfType((byte)dbElement.WordType);
                if (dbElement.SingleWord != null)
                {
                    result.WordSource = new SingleWordSource(dbElement.SingleWord);
                }
                else
                {
                    WeightedWord defaultWord = DB_WeightedWords
                        .Where(dbWeightedWord => dbWeightedWord.WordElement == wordBuilder_ID && dbElement.DefaultWeightedWord == dbWeightedWord.ID)
                        .Select(dbWeightedWord => new WeightedWord(dbWeightedWord.WordText, dbWeightedWord.Weight))
                        .Single();
                    IEnumerable<WeightedWord> alternates = DB_WeightedWords
                        .Where(dbWeightedWord => dbWeightedWord.WordElement == wordBuilder_ID && dbElement.DefaultWeightedWord != dbWeightedWord.ID)
                        .Select(dbWeightedWord => new WeightedWord(dbWeightedWord.WordText, dbWeightedWord.Weight));
                    result.WordSource = new WordSelector(defaultWord, alternates);
                }
            }
            return result;
        }

        private void SaveWord(WordElementBuilder word) 
        {
            DB_WordElement dbWordElement;
            bool isNewDB_Record;
            // First we need to make sure we have a WordElementBuilder with a valid FlexDB_ID.
            // We'll need that FlexDB_ID so DB_WeightedWords can refer to it when we save them in the database.
            if (word.FlexDB_ID == 0)        // The word builder does not already exist in the database.  We need to insert it so it has an ID.
            {
                isNewDB_Record = true;
                dbWordElement = new DB_WordElement
                {
                    WordType = FlexData.Word.TypeOf(word)
                };
                if (word.WordSource is SingleWordSource sws)
                    dbWordElement.SingleWord = sws.GetWord();
                DB_Elements.InsertOnSubmit(dbWordElement);
                SubmitChanges();
                // Now we have the ID available from the database.  Assign the ID to the in-memory object, so we'll remember it's not new if we update it
                word.FlexDB_ID = dbWordElement.ID;
            }
            else  // The word builder already exists in the database
            {
                isNewDB_Record = false;
                dbWordElement = DB_Elements
                    .Where(dbWordBuilder => dbWordBuilder.ID == word.FlexDB_ID)
                    .Cast<DB_WordElement>()
                    .Single();
            }
            // Store it in the cache
            FlexData.Word.Cache[word.FlexDB_ID] = word;
            UpdateDB_Element(dbWordElement);
            // Now we have a WordElementBuilder in the database, and we can deal with any DB_WeightedWords that might refer to it.
            switch (word.WordSource)
            {
                case SingleWordSource sws:
                    // If a WordElementBuilder has only a single word form, then in-memory its WordSource is a SingleWordSource.
                    // In this case we store it to the database with the field SingleWord holding that single word form.
                    // The DB_WordBuilder has its DefaultForm set to null because it has a single word form.
                    dbWordElement.SingleWord = sws.GetWord();
                    dbWordElement.DefaultWeightedWord = null;
                    UpdateDB_Element(dbWordElement);
                    break;
                case WordSelector ws:
                    if (isNewDB_Record)
                    {
                        IEnumerable<DB_WeightedWord> newWordsToInsert = ws.GetWeightedWordVariations()
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                WordElement = dbWordElement.ID,
                                WordText = wsVariation.Text,
                                Weight = wsVariation.Weight
                            });
                        DB_WeightedWords.InsertAllOnSubmit(newWordsToInsert);
                        SubmitChanges();
                        // Now that the newly inserted DB_WeightedWords have their IDs assigned, we can figure out which ID to make the Builder's DefaultForm.
                        // If a WordElementBuilder has multiple word forms, then in-memory its WordSource is a WordSelector.
                        // In this case we store it to the database as a collection of DB_WeightedWord that refer to the DB_WordBuilder, and the DB_WordBuilder
                        // has a reference to ONE of those.  The DB_WordBuilder has its SingleWord set to null because it has multiple word forms.
                        dbWordElement.DefaultWeightedWord = DB_WeightedWords
                            .Where(dbWord => dbWord.WordElement == dbWordElement.ID && dbWord.WordText.Equals(ws.Default.Text))
                            .Single()
                            .ID;
                        dbWordElement.SingleWord = null;
                        SubmitChanges();
                    }
                    else  // We're updating a DB_WordBuilder that's already in the database
                    {
                        // The DB_WeightedWords that are already in the database belonging to this DB_WordBuilder
                        IEnumerable<DB_WeightedWord> existingWordsForThisBuilder = DB_WeightedWords
                            .Where(dbWord => dbWord.WordElement == dbWordElement.ID);
                        // The DB_WeightedWords that are already in the database, but are NOT present in the in-memory WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> existingWordsToDelete = existingWordsForThisBuilder
                            .Where(dbWord => !ws.GetWeightedWordVariations().Any(variation => variation.Text.Equals(dbWord.WordText)));
                        DB_WeightedWords.DeleteAllOnSubmit(existingWordsToDelete);
                        // The DB_WeightedWords that are already in the database, AND present in the WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> existingWordsToUpdate = existingWordsForThisBuilder
                            .Join(ws.GetWeightedWordVariations(),
                                existingDBWord => existingDBWord.WordText,
                                wsVariation => wsVariation.Text,
                                (existingDBWord, wsVariation) => existingDBWord)
                            .Where(existingDBWord => existingDBWord.Weight != ws.WeightOf(existingDBWord.WordText));
                        // Update weights for existing DB_WeightedWords, in case the user changed them in the UI
                        foreach (DB_WeightedWord eachWordToUpdate in existingWordsToUpdate)
                        {
                            eachWordToUpdate.Weight = ws.GetWeightedWordVariations()
                                .Where(wsVariation => wsVariation.Text.Equals(eachWordToUpdate.WordText))
                                .Single()
                                .Weight;
                        }
                        // New DB_WeightedWords corresponding to words that are NOT already in the database, but ARE present in the WordSelector that we're saving
                        IEnumerable<DB_WeightedWord> newWordsToInsert = ws.GetWeightedWordVariations()
                            .Where(wsVariation => !existingWordsForThisBuilder.Any(dbWord => dbWord.WordText.Equals(wsVariation.Text)))
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                WordElement = dbWordElement.ID,
                                WordText = wsVariation.Text,
                                Weight = wsVariation.Weight
                            });
                        DB_WeightedWords.InsertAllOnSubmit(newWordsToInsert);
                        // Now update the DB_WordBuilder.  Since the in-memory WordElementBuilder has a WordSelector as its WordSource, the database form needs
                        // to have DefaultForm set, and SingleWord NOT set.
                        dbWordElement.DefaultWeightedWord = existingWordsForThisBuilder.Concat(newWordsToInsert)
                            .Where(dbWord => dbWord.WordElement == dbWordElement.ID && dbWord.WordText.Equals(ws.Default.Text))
                            .Single()
                            .ID;
                        dbWordElement.SingleWord = null;
                    }
                    break;
                default: break;
            }
            SubmitChanges();
        }
    }
}
