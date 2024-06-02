using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoLists.Data;

namespace TodoLists.Utils.Mutators.Actions
{
    internal class DescriptionChangeAction: IAction
    {
        public ActionType ActionType { get { return ActionType.DescriptionChange; } }

        private ToDoElement _element;
        private string oldDescription;
        private string newDescription;

        public DescriptionChangeAction(ToDoElement element, string oldDescription, string newDescription)
        {
            this._element = element;
            this.oldDescription = oldDescription;
            this.newDescription = newDescription;
        }

        public void Execute()
        {
            this._element.SetDescription(this.newDescription);
            this._element.ForceRefresh(nameof(ToDoElement.Description));
        }

        public void Undo()
        {
            this._element.SetDescription(this.oldDescription);
            this._element.ForceRefresh(nameof(ToDoElement.Description));
        }

        public static void Execute(ToDoElement element, string oldDescription, string newDescription)
        {
            var action = new DescriptionChangeAction(element, oldDescription, newDescription);
            action.Execute();
            UndoRedoTracker.I.AddAction(action);
        }
    }
}
