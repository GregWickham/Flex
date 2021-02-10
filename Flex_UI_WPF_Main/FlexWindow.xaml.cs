using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FlexibleRealization;
using Flex.Database;
using Flex.Database.UserInterface;
using Flex.UserInterface.ViewModels;

namespace Flex.UserInterface
{
    /// <summary>Interaction logic for ParseAndRealizeWindow.xaml</summary>
    public partial class FlexWindow : Window
    {
        public FlexWindow()
        {
            InitializeComponent();
            VariationsWindow.Closing += VariationsWindow_Closing;
        }

        private void TreeEditor_SelectedNodeChanged()
        {
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
                    SynsetsControl.WordBuilder = word;
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

        /// <summary>This event handler is called when the TreeEditor has successfully realized some text</summary>
        private void TreeEditor_TextRealized(string realizedText)
        {
            realizedTextBox.Background = Brushes.WhiteSmoke;
            realizedTextBox.Text = realizedText;
        }

        /// <summary>This event handler is called when the TreeEditor has tried to realize an IElementTreeNode, but failed</summary>
        private void TreeEditor_RealizationFailed(IElementTreeNode failed)
        {
            realizedTextBox.Background = RealizeFailedBrush;
            realizedTextBox.Text = "";
        }

        /// <summary>A color representing that realization has failed</summary>
        private static Brush RealizeFailedBrush = new SolidColorBrush(Color.FromArgb(100, 254, 0, 0));

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

        /// <summary>When the user changes a setting for the CoreNLP server, save its settings</summary>
        private void CoreNLP_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Stanford.CoreNLP.Properties.Settings.Default.Save();

        /// <summary>When the user changes a setting for the SimpleNLG server, save its settings</summary>
        private void SimpleNLG_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SimpleNLG.Properties.Settings.Default.Save();

        /// <summary>When the user changes a setting for the WordNet server, save its settings</summary>
        private void WordNet_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => WordNet.Linq.Properties.Settings.Default.Save();

        /// <summary>The user wants to browse words in the Flex Database</summary>
        private void BrowseWordsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FlexDB_WordsBrowserWindow wordBrowserWindow = new FlexDB_WordsBrowserWindow();
            wordBrowserWindow.Closing += WordBrowserWindow_Closing;
            wordBrowserWindow.ElementDragStarted += TreeEditor.OnElementDragStarted;
            wordBrowserWindow.ElementDragCancelled += TreeEditor.OnElementDragCancelled;
            wordBrowserWindow.ElementDropCompleted += TreeEditor.OnElementDropCompleted;
            wordBrowserWindow.Show();
        }

        private void WordBrowserWindow_Closing(object sender, CancelEventArgs e)
        {
            FlexDB_WordsBrowserWindow wordBrowserWindow = (FlexDB_WordsBrowserWindow)sender;
            wordBrowserWindow.Closing -= WordBrowserWindow_Closing;
            wordBrowserWindow.ElementDragStarted -= TreeEditor.OnElementDragStarted;
            wordBrowserWindow.ElementDragCancelled -= TreeEditor.OnElementDragCancelled;
            wordBrowserWindow.ElementDropCompleted -= TreeEditor.OnElementDropCompleted;
        }

        /// <summary>The user wants to browse words in the Flex Database</summary>
        private void BrowseParentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FlexDB_ParentsBrowserWindow parentBrowserWindow = new FlexDB_ParentsBrowserWindow();
            parentBrowserWindow.Closing += ParentBrowserWindow_Closing;
            parentBrowserWindow.ElementDragStarted += TreeEditor.OnElementDragStarted;
            parentBrowserWindow.ElementDragCancelled += TreeEditor.OnElementDragCancelled;
            parentBrowserWindow.ElementDropCompleted += TreeEditor.OnElementDropCompleted;
            parentBrowserWindow.Show();
        }

        private void ParentBrowserWindow_Closing(object sender, CancelEventArgs e)
        {
            FlexDB_ParentsBrowserWindow parentBrowserWindow = (FlexDB_ParentsBrowserWindow)sender;
            parentBrowserWindow.Closing -= ParentBrowserWindow_Closing;
            parentBrowserWindow.ElementDragStarted -= TreeEditor.OnElementDragStarted;
            parentBrowserWindow.ElementDragCancelled -= TreeEditor.OnElementDragCancelled;
            parentBrowserWindow.ElementDropCompleted -= TreeEditor.OnElementDropCompleted;
        }

        /// <summary>When the user changes a setting for the WordNet server, save its settings</summary>
        private void FlexDB_SettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Flex.Database.Properties.Settings.Default.Save();

        /// <summary>Delete the selected ElementBuilder from the ElementBuilderGraphArea</summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBox.Clear();
            TreeEditor.DeleteSelection();
            realizedTextBox.Clear();
        }

        /// <summary>Save the selected ElementBuilder to the Flex database</summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            RealizationResult result = TreeEditor.Model.Realize();
            if (result.Outcome == RealizationOutcome.Success)
            {
                FlexData.Context.SaveAsync(TreeEditor.Model);
            }           
        }

        /// <summary>If there's text in the inputTextBox, parse it</summary>
        private void parseButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputTextBox.Text.Length > 0) HandleTextInput(inputTextBox.Text);
        }

        /// <summary>Send <paramref name="text"/> to the GraphEditor</summary>
        private void HandleTextInput(string text)
        {
            HideVariationsWindow();
            TreeEditor.ParseText(text);
        }

        /// <summary>The user has entered some text in the inputTextBox</summary>
        private void inputTextBox_TextInput(object sender, TextCompositionEventArgs e) => HandleTextInput(e.Text);

        /// <summary>The user has clicked on the "Show Variations" button</summary>
        private void showVariationsButton_Click(object sender, RoutedEventArgs e)
        {
            VariationsWindow.DefaultForm = realizedTextBox.Text;
            VariationsWindow.Variations.Clear();
            ShowVariationsWindow();
            foreach (IElementTreeNode eachRealizableVariation in TreeEditor.SelectedNode.GetRealizableVariations())
            {
                TryToRealizeVariation(eachRealizableVariation);
            }
        }

        /// <summary>Try to transform <paramref name="editableTree"/> into realizable form and if successful, try to realize it</summary>
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
    }
}
