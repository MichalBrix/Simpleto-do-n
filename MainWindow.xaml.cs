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
using TodoLists.Data;
using TodoLists.Utils;

namespace TodoLists
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ToDoElement CurrentlyFocusedElement = null;
        private TreeSearcher TreeSearcher;
        private TreeViewMutator TreeViewMutator;
        public ObservableCollection<Data.ToDoElement> ToDoElements { get; set; }

        public MainWindow()
        {
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
            this.TreeViewMutator = new TreeViewMutator(this.TreeSearcher, this.ToDoTree, this.ToDoElements);
            
        }

        private int counter = 0;

        public Data.ToDoElement MakeToDoElement(int depth)
        {
            counter++;
            var element = new Data.ToDoElement();
            element.Description = "Element " + depth + " (" + counter + ")";
            element.IsInProgress = false;
            element.IsFinished = false;
            element.Started = null;
            element.Finished = null;
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
            if (e.Key == Key.Up && Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt))
            {
                var treeView = sender as TreeView;

                if (this.CurrentlyFocusedElement != null)
                {
                    TreeViewMutator.SelectElementUp(this.CurrentlyFocusedElement);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down && Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt))
            {
                var treeView = sender as TreeView;

                if (this.CurrentlyFocusedElement != null)
                {
                    TreeViewMutator.SelectElementDown(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Left && Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt))
            {
                var treeView = sender as TreeView;

                if (this.CurrentlyFocusedElement != null)
                {
                    TreeViewMutator.CollapseElement(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Right && Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt))
            {
                var treeView = sender as TreeView;

                if (this.CurrentlyFocusedElement != null)
                {
                    TreeViewMutator.ExpandElement(this.CurrentlyFocusedElement);
                }

                e.Handled = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.CurrentlyFocusedElement = ((sender as TextBox).Parent as Grid).DataContext as ToDoElement;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CurrentlyFocusedElement = null;
        }

        private void ArrowUpButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var element = button.DataContext as ToDoElement;
            this.TreeViewMutator.MoveElementUp(element);
        }

        private void ArrowDownButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var element = button.DataContext as ToDoElement;
            this.TreeViewMutator.MoveElementDown(element);
        }

        private DateTime? LastPress = null;

        private void TextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.LastPress != null) {
                    var ct = DateTime.Now;
                    var dt = ct.Subtract(this.LastPress.Value);
                    if (dt.TotalSeconds > 1)
                    {
                        var textBox = sender as TextBox;
                        var data = textBox.DataContext as ToDoElement;
                        DragDrop.DoDragDrop(textBox, data, DragDropEffects.Move);

                    }
                }
            }
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.LastPress = DateTime.Now;
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
