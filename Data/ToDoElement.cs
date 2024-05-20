using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace TodoLists.Data
{
    public class ToDoElement: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Guid ID { get; set; } = Guid.NewGuid();
        private string _description;
        public string Description { get { return _description; } set { _description = value; RefreshState(); OnPropertyChanged(); } }

        private bool _isInProgress;
        private bool _isFinished;
        public bool IsInProgress { get { return _isInProgress;  } set { _isInProgress = value; OnPropertyChanged(); } }
        public bool IsFinished { get { return _isFinished; } set { _isFinished = value; OnPropertyChanged(); } }

        private bool _isExpanded;
        public bool IsExpanded { get { return _isExpanded; }  set { _isExpanded = value; OnPropertyChanged(); } }

        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        private bool _isTextBoxFocused;
        public bool IsTextBoxFocused { get { return _isTextBoxFocused; } set { _isTextBoxFocused = value; OnPropertyChanged(); } }

        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }

        public ObservableCollection<ToDoElement> Children { get; set; } = new ObservableCollection<ToDoElement>();

        private void RefreshState()
        {
            string descriptionLowered = this._description.ToLowerInvariant();
            IEnumerable<char> filtered = descriptionLowered.ToCharArray().Where(x => x=='_' || Char.IsAsciiLetter(x) );
            var ready = new string(filtered.ToArray());

            bool startsWithDone = ready.StartsWith("done");
            bool endsWithDone = ready.EndsWith("done");

            bool startsWithWIP = ready.StartsWith("wip");
            bool endsWithWIP = ready.EndsWith("wip");

            if (startsWithDone || endsWithDone)
            {
                this.IsFinished = true;
                this.IsInProgress = false;
            }
            else if (startsWithWIP || endsWithWIP)
            {
                this.IsInProgress = true;
                this.IsFinished = false;
            }

            if (startsWithDone || endsWithDone || startsWithWIP || endsWithWIP)
            {
                int whereToStartDeleting = 0;
                int howMuchToDelete = 0;

                if (startsWithDone || startsWithWIP)
                {
                    if (startsWithDone) howMuchToDelete = descriptionLowered.IndexOf("done") + 3;
                    else if (startsWithWIP) howMuchToDelete = descriptionLowered.IndexOf("wip") + 2;
                    char c;
                    do
                    {
                        howMuchToDelete++;
                        c = descriptionLowered[howMuchToDelete];
                    }
                    while (!Char.IsLetterOrDigit(c));
                    this._description = this._description.Remove(0, howMuchToDelete);
                }
                if (endsWithDone || endsWithWIP)
                {
                    if (endsWithDone) whereToStartDeleting = descriptionLowered.LastIndexOf("done");
                    else if (endsWithWIP) whereToStartDeleting = descriptionLowered.LastIndexOf("wip");
                    
                    
                    howMuchToDelete = this._description.Length - whereToStartDeleting;
                    if (howMuchToDelete < 5)
                    {
                        this._description = this._description.Remove(whereToStartDeleting, howMuchToDelete);
                    }
                }
            }
            
            
        }

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

        public ToDoElement? GetElementByGuid(Guid id)
        {
            if (this.ID == id)
            {
                return this;
            }
            foreach (var c in this.Children)
            {
                var possible = c.GetElementByGuid(id);
                if (possible != null)
                {
                    return possible;
                }
            }
            return null;
        }
    }
}
