using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace TodoLists.Data
{
    public class ToDoElement: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Description { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsFinished { get; set; }

        private bool _isExpanded;
        public bool IsExpanded { get { return _isExpanded; }  set { _isExpanded = value; OnPropertyChanged(); } }

        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        private bool _isTextBoxFocused;
        public bool IsTextBoxFocused { get { return _isTextBoxFocused; } set { _isTextBoxFocused = value; OnPropertyChanged(); } }

        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }

        public ObservableCollection<ToDoElement> Children { get; set; } = new ObservableCollection<ToDoElement>();

        public ToDoElement GetParentOfElement(ToDoElement element)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (element == Children[i])
                {
                    return this;
                }
            }
            for (int i = 0; i < Children.Count; i++)
            {
                var possibility = Children[i].GetParentOfElement(element);
                if (possibility != null)
                {
                    return possibility;
                }
            }
            return null;
        }

        public int GetIndexOf(ToDoElement child)
        {
            for (int i = 0; i < this.Children.Count; i++) {
                if (this.Children[i] == child)
                {
                    return i;
                }
            }

            return -1;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
