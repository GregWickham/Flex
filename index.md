![Flex logo](/images/FlexLogo.png)

Flex creates specifications of English grammatical elements, which can be permuted to generate text expressing an idea in a variety of ways.

Flex is built on top of [Echo](https://github.com/GregWickham/Echo), so you may want to review the [Echo documentation](https://gregwickham.github.io/Echo/) before proceeding.

Here's the [Flex repository on GitHub.](https://github.com/GregWickham/Flex)

### Prerequisites

Like Echo, Flex uses the servers for [CoreNLP](https://stanfordnlp.github.io/CoreNLP/) and [SimpleNLG](https://github.com/simplenlg/simplenlg).  The folder called `Server BAT files` contains two Windows batch files that I use to start these servers.  These batch files refer to specific locations in my file system, so you'll need to modify them to reflect the actual locations where you install the distribution `.jar` files.  The SimpleNLG server requires two `.jar` files that are not included in the SimpleNLG distribution -- `hsqldb_6148.jar` and `lexAccess2013dist.jar`.

In addition to CoreNLP and SimpleNLG, Flex uses the [Datamuse API](https://www.datamuse.com/api/), so you'll need an internet connection to use it.

### The User Interface

The Visual Studio startup project for the user interface is `Flex_UI_WPF`.  Let's start it, and parse a simple declarative clause:

![Flex window after parsing](/images/ParsedSDC.jpg)

So far it looks almost identical to the Echo window, with the exception of the *Show Variations* button in the lower right.

#### Specifying Word Variations

Nouns, verbs, adjectives, and adverbs have a special property not shared by other parts of speech:  They're often surrounded in the lexicon by a rich and varied constellation of other words that can replace them in a given syntactic structure.  If the replacement word is a true synonym the swap has little, if any, effect on overall meaning.  The replacement word can also be similar but not quite synonymous, adding shades to the overall meaning or changing it altogether.

Let's look at some examples.  First I'll select the noun "movie" by clicking on it:

![Flex window with "movie" selected](/images/MovieSelected.jpg)

A new control has appeared on the right side of the window.  This control will allow us to select alternate words that can be swapped in for "movie."

On the far right is a list of words with the header **Actual**.  This list is currently empty.

To the left of the "Actual" list is another list with the header **Potential**.  This list contains words that are candidates for the **Actual** list.  When a noun, verb, adjective, or adverb is selected, and the "Alternate Word Selection Control" opens, the **Potential** list is initially populated by querying the [Datamuse API](https://www.datamuse.com/api/) for words with similar meaning to the selected word.  Later, we'll see how to bring a wider variety of words into the **Potential** list, but for now let's select a few synonyms for "movie":

![Flex window with "movie" synonyms selected](/images/MovieSynonymsSelected.jpg)

When something is selected in the **Potential** list, pressing the *Insert* key moves the selection from **Potential** to **Actual**:

![Flex window with "movie" synonyms moved to actual](/images/MovieSynonymsMovedToActual.jpg)

Now the word "movie" has three alternate forms, in addition to its default form.  With "movie" still selected in the graph, pressing the *Show Variations* button opens another window to display the realized forms of the selected element:

![Variations of "movie"](/images/VariationsOfMovie.jpg)

Since we're only realizing a single word, it's not too interesting yet.

Each of the syntactic elements containing "movie" also has three alternate forms.  For example, let's select the noun phrase "a very good movie":

![Flex window with "a very good movie" selected](/images/AVeryGoodMovieSelected.jpg)

...and click on the *Show Variations* button to show the variations of the noun phrase:

![Variations of "a very good movie"](/images/VariationsOfAVeryGoodMovie.jpg)

That's a bit more interesting; but it's not really a **Combinatorial Explosion of Language**, is it?

Our sentence happens to contain a noun, a verb, and adjective, and an adverb.  What if we assign some alternates to *each* of those words?

Instead of the verb "saw," we could say "watched" or "caught."  Notice that we specify the lemma form of verbs, and [SimpleNLG](https://github.com/simplenlg/simplenlg) will inflect the verbs to past tense when it does the realization:

![Flex window with alternates for "saw"](/images/AlternatesForSaw.jpg)

Instead of the adverb "very," we could say "really," "truly," or "pretty":

![Flex window with alternates for "very"](/images/AlternatesForVery.jpg)

Instead of the adjective "good," we could say "great" or "cool":

![Flex window with alternates for "very"](/images/AlternatesForGood.jpg)

So we have:

* 3 variations of "saw"
* 4 variations of "very"
* 3 variations of "good"
* 4 variations of "movie"

The n-fold Cartesian product of these four sets has 3 * 4 * 3 * 4 = 144 elements, or 144 variations of the sentence "We saw a very good movie.":

![Variations of "we saw a very good movie"](/images/VariationsOfWeSawAVeryGoodMovie.jpg)

