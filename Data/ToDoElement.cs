using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using TodoLists.Utils;

namespace TodoLists.Data
{
    public class ToDoElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid ID { get; set; } = Guid.NewGuid();

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                string oldDescription = this._description;
                _description = value;
                RefreshState();
                OnPropertyChanged();
                Utils.Mutators.Actions.DescriptionChangeAction.Execute(this, oldDescription, this._description);
                UndoRedoTracker.I.FinishTransaction();
            }
        }

        public void SetDescription(string description)
        {
            this._description = description;
            OnPropertyChanged();
        }

        public void SetStatus(bool isInProgress, bool isFinished)
        {
            this._isInProgress = isInProgress;
            this._isFinished = isFinished;
            OnPropertyChanged();
        }
        private bool _isInProgress;
        private bool _isFinished;
        public bool IsInProgress
        {
            get
            { return _isInProgress; }
            set
            {
                if (value != _isInProgress)
                {
                    Utils.Mutators.Actions.ChangeStatusAction.Execute(this, value, this.IsFinished);
                    OnPropertyChanged();
                    UndoRedoTracker.I.FinishTransaction();
                }
                
            }
        }
        public bool IsFinished
        {
            get
            {
                return _isFinished;
            }
            set
            {
                if (_isFinished != value)
                {
                    Utils.Mutators.Actions.ChangeStatusAction.Execute(this, this.IsInProgress, value);
                    OnPropertyChanged();
                    UndoRedoTracker.I.FinishTransaction();
                }
                
            }
        }


        public void SetIsExpanded(bool value)
        {
            this._isExpanded = value;
            OnPropertyChanged();
        }
        private bool _isExpanded;
        public bool IsExpanded { get { return _isExpanded; } set { _isExpanded = value; OnPropertyChanged(); } }

        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; OnPropertyChanged(); } }

        private bool _isTextBoxFocused;
        public bool IsTextBoxFocused { get { return _isTextBoxFocused; } set { _isTextBoxFocused = value; OnPropertyChanged(); } }

        private int _opentElementsNo;
        private int _inProgressElementsNo;
        private int _finishedElementsNo;

        public int OpenElementsNo { get { return _opentElementsNo; } set { _opentElementsNo = value; OnPropertyChanged(); } }
        public int InProgressElementsNo { get { return _inProgressElementsNo; } set { _inProgressElementsNo = value; OnPropertyChanged(); } }
        public int FinishedElementsNo { get { return _finishedElementsNo; } set { _finishedElementsNo = value; OnPropertyChanged(); } }

        //public DateTime? Started { get; set; }
        //public DateTime? Finished { get; set; }

        public ObservableCollection<ToDoElement> Children { get; set; } = new ObservableCollection<ToDoElement>();

        public void ForceRefresh()
        {
            this.OnPropertyChanged();
        }

        private void RefreshState()
        {
            string descriptionLowered = this._description.ToLowerInvariant();
            IEnumerable<char> filtered = descriptionLowered.ToCharArray().Where(x => x == '_' || Char.IsAsciiLetter(x));
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
                    if (howMuchToDelete <= 4)
                    {
                        this._description = this._description.Remove(whereToStartDeleting, howMuchToDelete);
                        this._description.TrimEnd();
                    }
                }
            }
        }

        public ToDoElement? GetParentOfElement(ToDoElement element)
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
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i] == child)
                {
                    return i;
                }
            }

            return -1;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
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

        public ToDoElement Clone(bool deepClone)
        {
            var clone = new ToDoElement()
            {
                ID = this.ID,
                _description = this.Description,
                _isInProgress = this.IsInProgress,
                _isFinished = this.IsFinished,
                _isExpanded = this.IsExpanded,
                _isSelected = this.IsSelected,
                IsTextBoxFocused = this.IsTextBoxFocused,
                OpenElementsNo = this.OpenElementsNo,
                InProgressElementsNo = this.InProgressElementsNo,
                FinishedElementsNo = this.FinishedElementsNo
                //Started = this.Started,
                //Finished = this.Finished
            };

            if (deepClone)
            {
                foreach (var c in this.Children)
                {
                    clone.Children.Add(c.Clone(true));
                }
            }

            return clone;
        }
    }
}
