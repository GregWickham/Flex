### Prerequisites

Like Echo, Flex also uses the servers for CoreNLP and SimpleNLG.  The folder called `Server BAT files` contains two Windows batch files that I use to start these servers.  These batch files refer to specific locations in my file system, so you'll need to modify them to reflect the actual locations where you install the distribution `.jar` files.  The SimpleNLG server requires two `.jar` files that are not included in the SimpleNLG distribution -- `hsqldb_6148.jar` and `lexAccess2013dist.jar`.

### The User Interface

The Visual Studio startup project for the user interface is `Flex_UI_WPF`:
