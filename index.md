![Flex logo](/images/FlexLogo.png)

Flex creates specifications of English grammatical elements, which can be permuted to generate text that expresses an idea in a variety of ways.

Flex is built on top of [Echo](https://github.com/GregWickham/Echo), so you may want to review the [Echo documentation](https://gregwickham.github.io/Echo/) before proceeding.

### Prerequisites

Like Echo, Flex uses the servers for [CoreNLP](https://stanfordnlp.github.io/CoreNLP/) and [SimpleNLG](https://github.com/simplenlg/simplenlg).  The folder called `Server BAT files` contains two Windows batch files that I use to start these servers.  These batch files refer to specific locations in my file system, so you'll need to modify them to reflect the actual locations where you install the distribution `.jar` files.  The SimpleNLG server requires two `.jar` files that are not included in the SimpleNLG distribution -- `hsqldb_6148.jar` and `lexAccess2013dist.jar`.

In addition to CoreNLP and SimpleNLG, Flex uses the [Datamuse API](https://www.datamuse.com/api/), so you'll need an internet connection to use it.

### The User Interface

The Visual Studio startup project for the user interface is `Flex_UI_WPF`.  Let's start it, and parse a simple declarative clause:

![Flex window after parsing](/images/ParsedSDC.jpg)

So far it looks almost identical to the Echo window, with the exception of the "Show Variations" button in the lower right.

Nouns, verbs, adjectives, and adverbs have a special property not shared by other parts of speech:  They're often surrounded in the lexicon by a rich and varied constellation of other words that can replace them in a given syntactic structure.  If the replacement word is a true synonym the swap has little, if any, effect on overall meaning.  The replacement word can also be similar but not quite synonymous, adding shades to the overall meaning or changing it altogether.

Let's look at some examples.  First I'll select the noun "movie" by clicking on it:

![Flex window with movie selected](/images/MovieSelected.jpg)

A new control has appeared on the right side of the window.  This control will allow us to select alternate words that can be swapped in for "movie."

On the far right is a list of words with the header **Actual**.  This list is currently empty.

To the left of the "Actual" list is another list with the header **Potential**.  This list contains words that are candidates for the **Actual** list.  When a noun, verb, adjective, or adverb is selected, and the "Alternate Word Selection Control" opens, the **Potential** list is initially populated by querying the [Datamuse API](https://www.datamuse.com/api/) for words with similar meaning to the selected word.  Later, we'll see how to bring a wider variety of words into the **Potential** list, but for now let's select a few synonyms for "movie":

![Flex window with movie synonyms selected](/images/MovieSynonymsSelected.jpg)

When something is selected in the **Potential** list, pressing the *Insert* key moves the selection from **Potential** to **Actual**:

![Flex window with movie synonyms moved to actual](/images/MovieSynonymsMovedToActual.jpg)

