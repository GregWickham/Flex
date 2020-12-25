using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexibleRealization;
using Flex.ElementSelectors;

namespace Flex.Database
{
    public partial class FlexDataContext
    {
        //public Task<IQueryable<WordElementBuilder>> LoadWordBuilders() => Task.Run(() => DB_WordBuilders.Select(dbBuilder => Load(dbBuilder.ID)));

        //private WordElementBuilder Load(int wordBuilder_ID)
        //{
        //    DB_WordBuilder dbBuilder = DB_WordBuilders
        //        .Where(builder => builder.ID.Equals(wordBuilder_ID))
        //        .Single();
        //    //WordElementBuilder result = new WordElementBuilder(dbBuilder.C)
        //}

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
                builderToSave = new DB_WordBuilder();
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
            // Now we have a WordElementBuilder in the database, and we can deal with any DB_WeightedWords that might refer to it.
            switch (word.WordSource)
            {
                case SingleWordSource sws:
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
                        // Now the newly inserted DB_WeightedWords have their IDs assigned, so we can figure out which ID to make the Builder's Default
                        builderToSave.DefaultForm = DB_WeightedWords
                            .Where(dbWord => dbWord.Builder == builderToSave.ID && dbWord.Word.Equals(ws.Default.Word))
                            .Single()
                            .ID;
                        SubmitChanges();
                    }
                    else
                    {
                        IEnumerable<DB_WeightedWord> existingWordsForThisBuilder = DB_WeightedWords
                            .Where(dbWord => dbWord.Builder == builderToSave.ID);
                        IEnumerable<DB_WeightedWord> existingWordsToDelete = existingWordsForThisBuilder
                            .Where(dbWord => !ws.GetWeightedWordVariations().Any(variation => variation.Word.Equals(dbWord.Word)));
                        DB_WeightedWords.DeleteAllOnSubmit(existingWordsToDelete);
                        IEnumerable<DB_WeightedWord> existingWordsToUpdate = existingWordsForThisBuilder
                            .Join(ws.GetWeightedWordVariations(),
                                existingDBWord => existingDBWord.Word,
                                wsVariation => wsVariation.Word,
                                (existingDBWord, wsVariation) => existingDBWord)
                            .Where(existingDBWord => existingDBWord.Weight != ws.WeightOf(existingDBWord.Word));
                        foreach (DB_WeightedWord eachWordToUpdate in existingWordsToUpdate)
                        {
                            eachWordToUpdate.Weight = ws.GetWeightedWordVariations()
                                .Where(wsVariation => wsVariation.Word.Equals(eachWordToUpdate.Word))
                                .Single()
                                .Weight;
                        }
                        IEnumerable<DB_WeightedWord> newWordsToInsert = ws.GetWeightedWordVariations()
                            .Where(wsVariation => !existingWordsForThisBuilder.Any(dbWord => dbWord.Word.Equals(wsVariation.Word)))
                            .Select(wsVariation => new DB_WeightedWord
                            {
                                Builder = builderToSave.ID,
                                Word = wsVariation.Word,
                                Weight = wsVariation.Weight
                            });
                        DB_WeightedWords.InsertAllOnSubmit(newWordsToInsert);
                        builderToSave.DefaultForm = existingWordsForThisBuilder.Concat(newWordsToInsert)
                            .Where(dbWord => dbWord.Builder == builderToSave.ID && dbWord.Word.Equals(ws.Default.Word))
                            .Single()
                            .ID;
                    }
                    break;
                default: break;
            }
            SubmitChanges();
        });

    }
}
