using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpletodon.Data;

namespace Simpletodon.Utils.Mutators.Actions
{
    internal class ChangeStatusAction : IAction
    {
        public ActionType ActionType { get { return ActionType.ChangeStatus; } }
        
        private ToDoElement _element;
        private bool _previousIsFinished;
        private bool _newIsFinished;
        private bool _previousIsInProgress;
        private bool _newIsInProgress;


        public ChangeStatusAction(ToDoElement element, bool isInProgress, bool isFinished)
        {
            this._element = element;
            this._previousIsFinished = element.IsFinished;
            this._previousIsInProgress = element.IsInProgress;
            this._newIsInProgress = isInProgress;
            this._newIsFinished = isFinished;

        }

        public void Execute()
        {
            this._element.SetStatus(this._newIsInProgress, this._newIsFinished);
            this._element.ForceRefresh(nameof (ToDoElement.IsFinished));
            this._element.ForceRefresh(nameof(ToDoElement.IsInProgress));

        }
        public void Undo()
        {
            this._element.SetStatus(this._previousIsInProgress, this._previousIsFinished);
            this._element.ForceRefresh(nameof(ToDoElement.IsFinished));
            this._element.ForceRefresh(nameof(ToDoElement.IsInProgress));
        }

        public static void Execute(ToDoElement element, bool isInProgress, bool isFinished)
        {
            var action = new ChangeStatusAction(element, isInProgress, isFinished);
            action.Execute();
            UndoRedoTracker.I.AddAction(action);
        }
    }
}
