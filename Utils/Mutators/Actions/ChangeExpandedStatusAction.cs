using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpletodon.Data;

namespace Simpletodon.Utils.Mutators.Actions
{
    internal class ChangeExpandedStatusAction : IAction
    {
        public ActionType ActionType { get { return ActionType.AddElementToList; } }

        private bool oldValue;
        private bool newValue;
        private ToDoElement _element;


        public ChangeExpandedStatusAction(ToDoElement element, bool value)
        { 
            this._element = element;
            this.oldValue = element.IsExpanded;
            this.newValue = value;
        }

        public void Execute()
        {
            this._element.SetIsExpanded(this.newValue);
            this._element.ForceRefresh(nameof(ToDoElement.IsExpanded));

        }
        public void Undo()
        {
            this._element.SetIsExpanded(this.oldValue);
            this._element.ForceRefresh(nameof(ToDoElement.IsExpanded));
        }

        public static void Execute(ToDoElement element, bool value)
        {
            var action = new ChangeExpandedStatusAction(element, value);
            action.Execute();
            UndoRedoTracker.I.AddAction(action);
        }
    }
}
