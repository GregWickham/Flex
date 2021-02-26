using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FlexibleRealization;
using Flex.Database;
using Flex.Database.UserInterface;
using Flex.UserInterface.ViewModels;
using WordNet.Linq;
using WordNet.UserInterface;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for FlexWindow.xaml</summary>
    public partial class FlexWindow : Window
    {
        public FlexWindow()
        {
            InitializeComponent();
            VariationsWindow.Closing += VariationsWindow_Closing;
        }

        #region TreeEditor

        private void TreeEditor_ModelSetFromDatabase(IElementTreeNode tree) => BoundSynsets.LoadBindingsFor(tree);

        /// <summary>The user has selected an IElementTreeNode in the tree editor.</summary>
        private void TreeEditor_SelectedNodeChanged()
        {
            BoundSynsets.Element = TreeEditor.SelectedNode;
            switch (TreeEditor.SelectedNode)
            {
                case WordElementBuilder word:
                    if (word.SupportsVariations)
                    {
                        WordBuilderViewModel viewModel = new WordBuilderViewModel(word);
                        if (viewModel != null)
                        {
                            WordEditor.ViewModel = viewModel;
                            ShowWordEditor();
                            viewModel.PopulatePotential();
                        }
                        else HideWordEditor();
                    }
                    else HideWordEditor();
                    break;
                default:
                    HideWordEditor();
                    break;
            }

            // Adjusting the Window Width prevents the GraphEditor from being re-laid out which can mess up vertex positions
            void HideWordEditor()
            {
                if (WordEditor.Visibility == Visibility.Visible)
                {
                    // Suspend layout while doing this stuff
                    using (var d = Dispatcher.DisableProcessing())
                    {
                        Width = Width - WordEditor.Width;
                        WordEditor.Visibility = Visibility.Collapsed;
                    }
                }
            }

            // Adjusting the Window Width prevents the GraphEditor from being re-laid out which can mess up vertex positions
            void ShowWordEditor()
            {
                if (WordEditor.Visibility == Visibility.Collapsed)
                {
                    // Suspend layout while doing this stuff
                    using (var d = Dispatcher.DisableProcessing())
                    {
                        Width = Width + WordEditor.Width;
                        WordEditor.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>This event handler is called when the TreeEditor has successfully realized some text.</summary>
        private void TreeEditor_TextRealized(string realizedText)
        {
            RealizedTextBox.Background = Brushes.WhiteSmoke;
            RealizedTextBox.Text = realizedText;
        }

        /// <summary>This event handler is called when the TreeEditor has tried to realize an IElementTreeNode, but failed.</summary>
        private void TreeEditor_RealizationFailed(IElementTreeNode failed)
        {
            RealizedTextBox.Background = RealizeFailedBrush;
            RealizedTextBox.Text = "";
        }

        private void ExpandCollapseTreeEditorPropertiesButton_Checked(object sender, RoutedEventArgs e) => TreeEditor.ShowProperties = false;
        private void ExpandCollapseTreeEditorPropertiesButton_Unchecked(object sender, RoutedEventArgs e) => TreeEditor.ShowProperties = true;

        #endregion TreeEditor

        #region Menu Bar and Toolbar

        /// <summary>When the user changes a setting for the CoreNLP server, save its settings.</summary>
        private void CoreNLP_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Stanford.CoreNLP.Properties.Settings.Default.Save();

        /// <summary>When the user changes a setting for the SimpleNLG server, save its settings.</summary>
        private void SimpleNLG_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SimpleNLG.Properties.Settings.Default.Save();

        /// <summary>When the user changes a setting for the WordNet server, save its settings.</summary>
        private void WordNet_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => WordNet.Linq.Properties.Settings.Default.Save();

        /// <summary>The user wants to browse WordNet synsets.</summary>
        private void BrowseWordNetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new WordNetBrowserWindow(
                TreeEditor.OnSynsetDragStarted,
                TreeEditor.OnSynsetDragCancelled,
                TreeEditor.OnSynsetDropCompleted,
                null, null, null)
                .SetDroppedWordConverter(e =>
                {
                    IElementTreeNode droppedNode = null;
                    // We could get a dropped IElementTree node in one of two forms:
                    // 1.  It's in the IDataObject as an IElementTreeNode, ready to use; or
                    // 2.  It's in the IDataObject as a Task<ElementBuilder> that we can run to get the IElementTreeNode
                    if (e.Data.GetDataPresent(typeof(IElementTreeNode)))
                    {
                        droppedNode = (IElementTreeNode)e.Data.GetData(typeof(IElementTreeNode));
                    }
                    else if (e.Data.GetDataPresent(typeof(Task)))
                    {
                        Task<IElementTreeNode> getNodeTask = (Task<IElementTreeNode>)e.Data.GetData(typeof(Task));
                        droppedNode = getNodeTask.Result;
                    }
                    return (droppedNode is WordElementBuilder wordBuilder)
                        ? new WordSpecification(wordBuilder.WordSource.DefaultWord, wordBuilder switch
                            {
                                NounBuilder nounBuilder => WordNetData.PartOfSpeech.Noun,
                                VerbBuilder verbBuilder => WordNetData.PartOfSpeech.Verb,
                                AdjectiveBuilder adjectiveBuilder => WordNetData.PartOfSpeech.Adjective,
                                AdverbBuilder adverbBuilder => WordNetData.PartOfSpeech.Adverb,
                                _ => WordNetData.PartOfSpeech.Unspecified
                            })
                        : null;
                })
            .Show();
        }

        /// <summary>The user wants to browse words in the Flex Database</summary>
        private void BrowseWordsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new FlexDB_WordsBrowserWindow(
                TreeEditor.OnElementDragStarted,
                TreeEditor.OnElementDragCancelled,
                TreeEditor.OnElementDropCompleted)
            .Show();
        }

        /// <summary>The user wants to browse words in the Flex Database</summary>
        private void BrowseParentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new FlexDB_ParentsBrowserWindow(
                TreeEditor.OnElementDragStarted,
                TreeEditor.OnElementDragCancelled,
                TreeEditor.OnElementDropCompleted)
            .Show();
        }

        /// <summary>When the user changes a setting for the WordNet server, save its settings</summary>
        private void FlexDB_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Flex.Database.Properties.Settings.Default.Save();

        /// <summary>Delete the selected ElementBuilder from the ElementBuilderGraphArea</summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            TreeEditor.DeleteSelection();
            RealizedTextBox.Clear();
            BoundSynsets.Clear();
        }

        /// <summary>Save the selected IElementTreeNode, and its synset-to-element bindings, to the Flex database.</summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            RealizationResult result = TreeEditor.Model.Realize();
            if (result.Outcome == RealizationOutcome.Success)
            {
                await FlexData.Context.SaveAsync(TreeEditor.Model);
                // We can't save synset bindings until all IElementTreeNodes are saved and we have valid element IDs for each one that's bound
                BoundSynsets.SaveBindingsFor(TreeEditor.Model);
            }           
        }

        #endregion Menu Bar and Toolbar

        #region Input Text

        /// <summary>The user wants to parse the text in the InputTextBox.</summary>
        private void ParseButton_Click(object sender, RoutedEventArgs e) { HandleTextInput(); }

        /// <summary>If there's text in the InputTextBox, parse it.</summary>
        private void HandleTextInput()
        {
            HideVariationsWindow();
            if (InputTextBox.Text.Length > 0) TreeEditor.ParseText(InputTextBox.Text);
        }

        /// <summary>The user has entered a keystroke in the InputTextBox.</summary>
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) HandleTextInput(); }

        #endregion Input Text

        #region Synset Bindings

        /// <summary>This event handler is called when the TreeEditor wants to bind a synset to an IElementTreeNode.</summary>
        /// <remarks>The actual creation of the binding is delegated to the BoundSynsetsControl.</remarks>
        private void TreeEditor_SynsetBoundToNode(IElementTreeNode boundNode, Synset boundSynset)
        {
            ExpandCollapseSynsetBindingsButton.IsChecked = false;
            BoundSynsets.BindSynsetToNode(boundNode, boundSynset);
        }

        #endregion Synset Bindings

        #region Variations

        private readonly VariationsListWindow VariationsWindow = new VariationsListWindow();

        private void ShowVariationsWindow()
        {
            if (!VariationsWindowIsShowing)
            {
                VariationsWindow.Show();
                VariationsWindowIsShowing = true;
            }
        }

        private void HideVariationsWindow()
        {
            if (VariationsWindowIsShowing)
            {
                VariationsWindow.Hide();
                VariationsWindowIsShowing = false;
            }
        }

        private bool VariationsWindowIsShowing { get; set; }

        private void VariationsWindow_Closing(object sender, CancelEventArgs e) => HideVariationsWindow();

        /// <summary>The user has clicked on the "Show Variations" button.</summary>
        private void ShowVariationsButton_Click(object sender, RoutedEventArgs e)
        {
            VariationsWindow.DefaultForm = RealizedTextBox.Text;
            VariationsWindow.Variations.Clear();
            ShowVariationsWindow();
            foreach (IElementTreeNode eachRealizableVariation in TreeEditor.SelectedNode.GetRealizableVariations())
            {
                TryToRealizeVariation(eachRealizableVariation);
            }
        }

        /// <summary>Try to transform <paramref name="editableTree"/> into realizable form and if successful, try to realize it.</summary>
        private void TryToRealizeVariation(IElementTreeNode editableTree)
        {
            RealizationResult result = editableTree.Realize();
            switch (result.Outcome)
            {
                case RealizationOutcome.Success:
                    VariationsWindow.Variations.Add(result.Text);
                    break;
                default: break;
            }
        }

        #endregion Variations

        /// <summary>A color representing that realization has failed.</summary>
        private static Brush RealizeFailedBrush = new SolidColorBrush(Color.FromArgb(100, 254, 0, 0));
    }
}
