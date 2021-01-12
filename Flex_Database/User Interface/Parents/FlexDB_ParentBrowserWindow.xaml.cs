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
    public partial class FlexDB_ParentBrowserWindow : Window
    {
        public FlexDB_ParentBrowserWindow()
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
            DataContext = new ParentBuildersViewModel();
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

        private ParentElementBuilder SelectedParent { get; set; }

        private void ParentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParentViewModel viewModel = (ParentViewModel)ParentsListBox.SelectedItem;
            SelectedParent = viewModel.Model;
        }

        private Point mouseDownPosition;
        private void ParentsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownPosition = e.GetPosition(this);
        }

        private void ParentsListBox_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);
            Vector mouseDownDistanceMoved = mouseDownPosition - currentMousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(mouseDownDistanceMoved.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(mouseDownDistanceMoved.Y) > SystemParameters.MinimumVerticalDragDistance) &&
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (SelectedParent != null)
                {
                    OnElementDragStarted(SelectedParent);
                    DragDropEffects dragResult = DragDrop.DoDragDrop(this, SelectedParent, DragDropEffects.Copy | DragDropEffects.None);
                    switch (dragResult)
                    {
                        case DragDropEffects.None:
                            OnElementDragCancelled(SelectedParent);
                            break;
                        case DragDropEffects.Copy:
                            OnElementDropCompleted(SelectedParent);
                            break;
                        default: break;
                    }
                }
            }
        }
    }
}
