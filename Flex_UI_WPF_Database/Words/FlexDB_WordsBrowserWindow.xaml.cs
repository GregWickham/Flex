using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlexibleRealization;
using FlexibleRealization.UserInterface;
using Flex.Database.UserInterface.ViewModels;
using System.ComponentModel;

namespace Flex.Database.UserInterface
{
    /// <summary>Interaction logic for FlexDB_BrowserWindow.xaml</summary>
    public partial class FlexDB_WordsBrowserWindow : Window
    {
        public FlexDB_WordsBrowserWindow()
        {
            InitializeComponent();
        }

        public FlexDB_WordsBrowserWindow(
            ElementDragStarted_EventHandler elementDragStartedHandler,
            ElementDragCancelled_EventHandler elementDragCancelledHandler,
            ElementDropCompleted_EventHandler elementDropCompletedHandler)
        {
            InitializeComponent();
            // Hook up external event handlers supplied to the constructor, and keep track of them
            if (elementDragStartedHandler != null)
            {
                ElementDragStarted += elementDragStartedHandler;
                External_ElementDragStarted_EventHandler = elementDragStartedHandler;
            }
            if (elementDragCancelledHandler != null)
            {
                ElementDragCancelled += elementDragCancelledHandler;
                External_ElementDragCancelled_EventHandler = elementDragCancelledHandler;
            }
            if (elementDragStartedHandler != null)
            {
                ElementDropCompleted += elementDropCompletedHandler;
                External_ElementDropCompleted_EventHandler = elementDropCompletedHandler;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => DataContext = new DB_WordElementsViewModel();

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            ViewModel.Detach();
            if (External_ElementDragStarted_EventHandler != null) ElementDragStarted -= External_ElementDragStarted_EventHandler;
            if (External_ElementDragCancelled_EventHandler != null) ElementDragCancelled -= External_ElementDragCancelled_EventHandler;
            if (External_ElementDropCompleted_EventHandler != null) ElementDropCompleted -= External_ElementDropCompleted_EventHandler;
        }

        #region External event handlers that can optionally be attached on construction of this Window

        private ElementDragStarted_EventHandler External_ElementDragStarted_EventHandler;
        private ElementDragCancelled_EventHandler External_ElementDragCancelled_EventHandler;
        private ElementDropCompleted_EventHandler External_ElementDropCompleted_EventHandler;

        #endregion External event handlers that can optionally be attached on construction of this Window


        #region Events

        public event ElementDragStarted_EventHandler ElementDragStarted;
        private void OnElementDragStarted(Type draggedType) => ElementDragStarted?.Invoke(draggedType);


        public event ElementDragCancelled_EventHandler ElementDragCancelled;
        private void OnElementDragCancelled(Type draggedType) => ElementDragCancelled?.Invoke(draggedType);


        public event ElementDropCompleted_EventHandler ElementDropCompleted;
        private void OnElementDropCompleted(Type droppedType) => ElementDropCompleted?.Invoke(droppedType);

        #endregion Events


        /// <summary>The view model for all DB_WordElements in the Flex database.</summary>
        private DB_WordElementsViewModel ViewModel => (DB_WordElementsViewModel)DataContext;

        private void NewButton_Click(object sender, RoutedEventArgs e) { }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) { }

        private void SaveButton_Click(object sender, RoutedEventArgs e) => SelectedViewModel?.Save();

        /// <summary>The view model for the ONE selected DB_WordElement.</summary>
        private DB_WordElementViewModel SelectedViewModel => (DB_WordElementViewModel)WordsListBox.SelectedItem;

        private DB_Word SelectedWord => SelectedViewModel?.DB_Word;

        /// <summary>The user has selected a word from the list of words in the Flex database.</summary>
        private void WordsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedWord != null && SelectedWord.SupportsVariations)
            {
                WordEditor.ViewModel = SelectedViewModel;
                ShowWordEditor();
                SelectedViewModel.PopulatePotential();
            }
            else HideWordEditor();
        }

        private void HideWordEditor()
        {
            if (WordEditor != null && WordEditor.Visibility == Visibility.Visible)
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
        private void ShowWordEditor()
        {
            if (WordEditor != null && WordEditor.Visibility == Visibility.Collapsed)
            {
                // Suspend layout while doing this stuff
                using (var d = Dispatcher.DisableProcessing())
                {
                    Width = Width + WordEditor.Width;
                    WordEditor.Visibility = Visibility.Visible;
                }
            }
        }

        #region Drag / Drop

        private Point mouseDownPosition;
        private void WordsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownPosition = e.GetPosition(this);
        }

        private void WordsListBox_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);
            Vector mouseDownDistanceMoved = mouseDownPosition - currentMousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (SelectedWord != null)
                {
                    Type draggedType = FlexData.Word.BuilderTypeFrom((byte)SelectedWord.WordType);
                    Task<IElementTreeNode> loadWordTask = FlexData.Context.LoadTreeAsync(SelectedWord.ID);
                    OnElementDragStarted(draggedType);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(Task), loadWordTask);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnElementDragCancelled(draggedType);
                            break;
                        case DragDropEffects.Copy:
                            OnElementDropCompleted(draggedType);
                            break;
                        default: break;
                    }
                }
            }
        }

        #endregion Drag / Drop

        #region Part of Speech Radio Buttons

        private void SetWordTypeFilter(FlexData.WordType wordType)
        {
            ViewModel?.SetWordTypeFilter(wordType);
            if (FlexData.Word.SupportsVariations(wordType)) ShowWordEditor();
            else HideWordEditor();
        }

        private void AllWordTypesButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Unspecified);
        private void NounButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Noun);
        private void VerbButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Verb);
        private void AdjectiveButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Adjective);
        private void AdverbButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Adverb);
        private void PronounButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Pronoun);
        private void PrepositionButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Preposition);
        private void ConjunctionButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Conjunction);
        private void DeterminerButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Determiner);
        private void ModalButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Modal);
        private void ParticleButton_Checked(object sender, RoutedEventArgs e) => SetWordTypeFilter(FlexData.WordType.Particle);

        #endregion Part of Speech Radio Buttons

        #region Alphabetic Filter Radio Buttons

        private void AllLetters_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter(null);
        private void A_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("a");
        private void B_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("b");
        private void C_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("c");
        private void D_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("d");
        private void E_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("e");
        private void F_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("f");
        private void G_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("g");
        private void H_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("h");
        private void I_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("i");
        private void J_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("j");
        private void K_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("k");
        private void L_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("l");
        private void M_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("m");
        private void N_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("n");
        private void O_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("o");
        private void P_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("p");
        private void Q_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("q");
        private void R_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("r");
        private void S_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("s");
        private void T_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("t");
        private void U_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("u");
        private void V_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("v");
        private void W_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("w");
        private void X_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("x");
        private void Y_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("y");
        private void Z_Button_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetAlphabeticFilter("z");

        #endregion Alphabetic Filter Radio Buttons

    }
}
