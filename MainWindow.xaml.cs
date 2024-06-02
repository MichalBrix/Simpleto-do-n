using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TodoLists.Data;
using TodoLists.Utils;
using TodoLists.Utils.Mutators;

namespace TodoLists
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ToDoElement? CurrentlyFocusedElement = null;
        private ElementCounter Counter;
        private TreeSearcher TreeSearcher;
        private TreeViewMutator TreeViewMutator;
        public ObservableCollection<Data.ToDoElement> ToDoElements { get; set; }

        public MainWindow()
        {
            UndoRedoTracker.I.SuppressTracking = true;
            this.ToDoElements = new ObservableCollection<ToDoElement>();
            

            for (var i = 0; i < 10; i++)
            {
                this.ToDoElements.Add(MakeToDoElement(0));
            }
            
            

            InitializeComponent();

            this.FontFamily = new FontFamily("Consolas");
            this.FontSize = 16;
            this.ToDoTree.ItemsSource = this.ToDoElements;
            this.TreeSearcher = new TreeSearcher(this.ToDoElements);
            var finalMutator = new FinalMutators();
            this.TreeViewMutator = new TreeViewMutator(this.TreeSearcher, finalMutator, this.ToDoTree, this.ToDoElements);
            this.Counter = new ElementCounter(this.ToDoElements, this.TreeSearcher, finalMutator);
            this.Counter.RecalculateElements();
            UndoRedoTracker.I.SuppressTracking = false;
        }

        private int counter = 0;

        public Data.ToDoElement MakeToDoElement(int depth)
        {
            counter++;
            var element = new Data.ToDoElement();
            element.SetDescription("Element " + depth + " (" + counter + ")");
            element.SetStatus(false, false);
            if (depth < 5)
            {
                for (var i = 0; i < 4; i++)
                {
                    element.Children.Add(MakeToDoElement(depth + 1));
                }
            }

            return element;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                e.Handled = false;
                return;
            }
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                //add new item here.
                e.Handled = true;
            }
            else if (e.Key == Key.Tab && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                //todo: handle moving to right
                e.Handled = true;
            }
            else if (e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                //todo: handle moving to left
                e.Handled = true;
            }
            
        }

        private void ToDoTree_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.Up && Keyboard.Modifiers == (/*ModifierKeys.Shift | ModifierKeys.Control |*/ ModifierKeys.Alt))
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.TreeViewMutator.SelectElementUp(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.SystemKey == Key.Down && Keyboard.Modifiers == (/*ModifierKeys.Shift | ModifierKeys.Control |*/ ModifierKeys.Alt))
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.TreeViewMutator.SelectElementDown(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
            else if (e.SystemKey == Key.Left && Keyboard.Modifiers == (/*ModifierKeys.Shift | ModifierKeys.Control |*/ ModifierKeys.Alt))
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.TreeViewMutator.CollapseElement(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
            else if (e.SystemKey == Key.Right && Keyboard.Modifiers == (/*ModifierKeys.Shift | ModifierKeys.Control |*/ ModifierKeys.Alt))
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.TreeViewMutator.ExpandElement(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
            else if ((e.SystemKey == Key.B && Keyboard.Modifiers == ModifierKeys.Alt)
                    || (e.SystemKey == Key.Enter && Keyboard.Modifiers == ModifierKeys.Alt))
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_AddChildBelow(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
            else if ((e.SystemKey == Key.N && Keyboard.Modifiers == ModifierKeys.Alt)
                    || (e.Key == Key.Enter))
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_AddSibling(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.SystemKey == Key.OemComma && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_MoveLeft(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.SystemKey == Key.OemPeriod && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_MoveRight(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_MarkWorkInProgress(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_MarkFinished(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_ClearMarking(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.SystemKey == Key.Delete && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    this.Action_Delete(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                if (this.CurrentlyFocusedElement != null)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        this.TreeViewMutator.SelectElementUp(this.CurrentlyFocusedElement);
                    }
                    else
                    {
                        this.TreeViewMutator.SelectElementDown(this.CurrentlyFocusedElement);
                    }
                }
            }
            
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                UndoRedoTracker.I.Undo();
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                UndoRedoTracker.I.Redo();
            }
            else if (e.OriginalSource is TextBox && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt))
            {
                e.Handled = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.CurrentlyFocusedElement = (((sender as TextBox)!.Parent as Grid)!.DataContext as ToDoElement)!;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CurrentlyFocusedElement = null;
        }

        private ToDoElement GetElementFromSender(object sender)
        {
            var button = sender as FrameworkElement;
            var element = (button!.DataContext as ToDoElement)!;
            return element;
        }

        private void ArrowUpButton_Click(object sender, RoutedEventArgs e)
        {
            var element = this.GetElementFromSender(sender);
            var parent = this.TreeSearcher.FindParent(element);
            this.TreeViewMutator.MoveElementUp(element);
            this.Counter.CalculateElementUp(parent);
            this.Counter.CalculateParentUp(element);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void ArrowDownButton_Click(object sender, RoutedEventArgs e)
        {
            var element = this.GetElementFromSender(sender);
            var parent = this.TreeSearcher.FindParent(element);
            this.TreeViewMutator.MoveElementDown(element);
            this.Counter.CalculateElementUp(parent);
            this.Counter.CalculateParentUp(element);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var ellipse = sender as Ellipse;
                var data = ellipse!.DataContext as ToDoElement;
                DragDrop.DoDragDrop(ellipse, "ToDoElement.GUID:" + data!.ID.ToString(), DragDropEffects.Move);
            }
        }

        private void ArrowDownButton_Drop(object sender, DragEventArgs e)
        {
            var contextElement = this.GetElementFromSender(sender);
            var dropString = e.Data.GetData(DataFormats.Text) as string;
            if (dropString != null && dropString.StartsWith("ToDoElement.GUID:"))
            {
                dropString = dropString.Split(':')[1];
                var element = this.TreeSearcher.GetElementByGuid(dropString);
                if (element != null)
                {
                    var parent = this.TreeSearcher.FindParent(element);
                    this.TreeViewMutator.DropElementDown(contextElement, element);
                    this.Counter.CalculateElementUp(parent);
                    this.Counter.CalculateParentUp(element);
                    UndoRedoTracker.I.FinishTransaction();
                }
            }
            
            return;
        }

        private void ArrowUpButton_Drop(object sender, DragEventArgs e)
        {
            var contextElement = this.GetElementFromSender(sender);
            var dropString = e.Data.GetData(DataFormats.Text) as string;
            if (dropString != null && dropString.StartsWith("ToDoElement.GUID:"))
            {
                dropString = dropString.Split(':')[1];
                var element = this.TreeSearcher.GetElementByGuid(dropString);
                if (element != null)
                {
                    var parent = this.TreeSearcher.FindParent(element);
                    this.TreeViewMutator.DropElementUp(contextElement, element);
                    this.Counter.CalculateElementUp(parent);
                    this.Counter.CalculateParentUp(element);
                    UndoRedoTracker.I.FinishTransaction();
                }
            }

            return;
        }

        private void MenuItem_AddChildBelow(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_AddChildBelow(ele);
        }

        private void Action_AddChildBelow(ToDoElement parent)
        {
            this.TreeViewMutator.ExpandElement(parent);
            var newEle = this.TreeViewMutator.AddChildToElement(parent);
            this.Counter.CalculateElementUp(parent);
            this.FocusOnElement(newEle);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void FocusOnElement(ToDoElement ele)
        {
            this.Dispatcher.Invoke(() => { ele.IsTextBoxFocused = true; }, DispatcherPriority.Input);
            this.CurrentlyFocusedElement = ele;
        }

        private void MenuItem_AddSibling(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_AddSibling(ele);
        }

        private void Action_AddSibling(ToDoElement element)
        {
            var newElement = this.TreeViewMutator.AddSibling(element);
            this.Counter.CalculateParentUp(element);
            this.FocusOnElement(newElement);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void MenuItem_MoveLeft(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_MoveLeft(ele);
        }

        private void MenuItem_MoveRight(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_MoveRight(ele);
        }

        private void Action_MoveLeft(ToDoElement ele)
        {
            var parent = this.TreeSearcher.FindParent(ele);
            if (parent != null) {
                this.TreeViewMutator.MoveAsParentSiblingBelow(ele, parent);
                this.Counter.CalculateElementUp(ele);
                this.Counter.CalculateElementUp(parent);
                this.FocusOnElement(ele);
            }
            UndoRedoTracker.I.FinishTransaction();
        }

        private void Action_MoveRight(ToDoElement ele)
        {
            var eleData = this.TreeSearcher.GetIndexAndParentCollectionFromElement(ele);
            if (eleData.Index > 0)
            {
                this.TreeViewMutator.MoveAsChildOfAbove(ele, eleData.Index, eleData.Parent, eleData.ParentList);
                this.Counter.CalculateElementUp(ele);
                this.Counter.CalculateElementUp(eleData.Parent);
                this.FocusOnElement(ele);
            }
            UndoRedoTracker.I.FinishTransaction();
        }

        private void MenuItem_MarkWorkInProgress(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_MarkWorkInProgress(ele);
        }

        private void Action_MarkWorkInProgress(ToDoElement ele)
        {
            var before = ele.Clone(false);
            this.TreeViewMutator.ChangeStatus(ele, true, false);
            this.Counter.CalculateParentUp(ele);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void MenuItem_MarkFinished(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_MarkFinished(ele);
        }

        private void Action_MarkFinished(ToDoElement ele)
        {
            var before = ele.Clone(false);
            this.TreeViewMutator.ChangeStatus(ele, false, true);
            this.Counter.CalculateParentUp(ele);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void MenuItem_ClearMarking(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_ClearMarking(ele);
        }

        private void Action_ClearMarking(ToDoElement ele)
        {
            this.TreeViewMutator.ChangeStatus(ele, false, false);
            this.Counter.CalculateParentUp(ele);
            UndoRedoTracker.I.FinishTransaction();
        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
            var ele = this.GetElementFromSender(sender);
            this.Action_Delete(ele);
            
        }

        private void Action_Delete(ToDoElement ele)
        {
            var parent = this.TreeSearcher.FindParent(ele);
            this.TreeViewMutator.DeleteElement(ele);
            if (parent != null)
            {
                this.Counter.CalculateElementUp(parent);
            }
            UndoRedoTracker.I.FinishTransaction();
        }
    }
}
