using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlexibleRealization;
using FlexibleRealization.UserInterface;
using Flex.Database.UserInterface.ViewModels;

namespace Flex.Database.UserInterface
{
    /// <summary>Interaction logic for FlexDB_BrowserWindow.xaml</summary>
    public partial class FlexDB_WordBrowserWindow : Window
    {
        public FlexDB_WordBrowserWindow()
        {
            InitializeComponent();
        }

        public event ElementDragStarted_EventHandler ElementDragStarted;
        private void OnElementDragStarted(ElementBuilder dragged) => ElementDragStarted?.Invoke(dragged);


        public event ElementDragCancelled_EventHandler ElementDragCancelled;
        private void OnElementDragCancelled(ElementBuilder dragged) => ElementDragCancelled?.Invoke(dragged);


        public event ElementDropCompleted_EventHandler ElementDropCompleted;
        private void OnElementDropCompleted(ElementBuilder dropped) => ElementDropCompleted?.Invoke(dropped);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new WordBuildersViewModel();
        }


        private void NewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private WordElementBuilder SelectedWord { get; set; }

        private void WordsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedWord = (WordElementBuilder)WordsListBox.SelectedItem;
            WordEditorViewModel viewModel = WordEditorViewModel.For(SelectedWord);
            WordEditor.ViewModel = viewModel;
        }

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
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance) &&
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (SelectedWord != null)
                {
                    OnElementDragStarted(SelectedWord);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, SelectedWord, DragDropEffects.Copy | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnElementDragCancelled(SelectedWord);
                            break;
                        case DragDropEffects.Copy:
                            OnElementDropCompleted(SelectedWord);
                            break;
                        default: break;
                    }
                }
            }
        }
    }
}
