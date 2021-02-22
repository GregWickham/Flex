using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FlexibleRealization;
using FlexibleRealization.UserInterface;
using Flex.Database.UserInterface.ViewModels;
namespace Flex.Database.UserInterface
{
    /// <summary>Interaction logic for FlexDB_BrowserWindow.xaml</summary>
    public partial class FlexDB_ParentsBrowserWindow : Window
    {
        public FlexDB_ParentsBrowserWindow()
        {
            InitializeComponent();
        }

        public FlexDB_ParentsBrowserWindow(
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

        private void Window_Loaded(object sender, RoutedEventArgs e) => DataContext = new DB_ParentElementsViewModel();

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

        public event ElementDragStarted_EventHandler ElementDragStarted;
        private void OnElementDragStarted(Type draggedType) => ElementDragStarted?.Invoke(draggedType);


        public event ElementDragCancelled_EventHandler ElementDragCancelled;
        private void OnElementDragCancelled(Type draggedType) => ElementDragCancelled?.Invoke(draggedType);


        public event ElementDropCompleted_EventHandler ElementDropCompleted;
        private void OnElementDropCompleted(Type droppedType) => ElementDropCompleted?.Invoke(droppedType);



        private void DeleteButton_Click(object sender, RoutedEventArgs e) { }

        private DB_ParentElementsViewModel ViewModel => (DB_ParentElementsViewModel)DataContext;

        private DB_ParentElementViewModel SelectedViewModel => (DB_ParentElementViewModel)ParentsGrid.SelectedItem;

        private DB_Parent SelectedParent => SelectedViewModel.DB_Parent;


        #region Drag / Drop

        private Point mouseDownPosition;
        private void ParentsGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownPosition = e.GetPosition(this);
        }

        private void ParentsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);
            Vector mouseDownDistanceMoved = mouseDownPosition - currentMousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (SelectedParent != null)
                {
                    Type draggedType = FlexData.Parent.BuilderTypeFrom((byte)SelectedParent.ParentType);
                    Task<IElementTreeNode> loadParentTask = FlexData.Context.LoadTreeAsync(SelectedParent.ID);
                    OnElementDragStarted(draggedType);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(Task), loadParentTask);
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

        #region Syntax Element Type Radio Buttons

        private void AllButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.Unspecified);
        private void IndependentClauseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.IndependentClause);
        private void SubordinateClauseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.SubordinateClause);
        private void NounPhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.NounPhrase);
        private void VerbPhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.VerbPhrase);
        private void AdjectivePhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.AdjectivePhrase);
        private void AdverbPhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.AdverbPhrase);
        private void PrepositionalPhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.PrepositionalPhrase);
        private void WhNounPhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.WhNounPhrase);
        private void WhAdverbPhraseButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.WhAdverbPhrase);
        private void CompoundNounButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.CompoundNoun);
        private void NominalModifierButton_Checked(object sender, RoutedEventArgs e) => ViewModel?.SetParentTypeFilter(FlexData.ParentType.NominalModifier);

        #endregion Syntax Element Type Radio Buttons

    }
}
