using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpletodon.Data;

namespace Simpletodon.Utils.Mutators.Actions
{
    internal class ChangeCountAction: IAction
    {
        public ActionType ActionType { get { return ActionType.ChangeCount; } }

        private ToDoElement _element;
        private int _previousOpenNo;
        private int _previousInProgressNo;
        private int _previousFinishedNo;
        private int _newOpenNo;
        private int _newInProgressNo;
        private int _newFinishedNo;

        public ChangeCountAction(ToDoElement element, int openElementsNo, int inProgressElementsNo, int finishedElementsNo)
        {
            this._element = element;
            this._previousOpenNo = element.OpenElementsNo;
            this._previousInProgressNo = element.InProgressElementsNo;
            this._previousFinishedNo = element.FinishedElementsNo;
            this._newOpenNo = openElementsNo;
            this._newInProgressNo = inProgressElementsNo;
            this._newFinishedNo = finishedElementsNo;
        }

        public void Execute()
        {
            this._element.OpenElementsNo = this._newOpenNo;
            this._element.InProgressElementsNo = this._newInProgressNo;
            this._element.FinishedElementsNo = this._newFinishedNo;
            this._element.ForceRefresh(nameof(ToDoElement.OpenElementsNo));
            this._element.ForceRefresh(nameof(ToDoElement.InProgressElementsNo));
            this._element.ForceRefresh(nameof(ToDoElement.FinishedElementsNo));

        }
        public void Undo()
        {
            this._element.OpenElementsNo = this._previousOpenNo;
            this._element.InProgressElementsNo = this._previousInProgressNo;
            this._element.FinishedElementsNo = this._previousFinishedNo;
            this._element.ForceRefresh(nameof(ToDoElement.OpenElementsNo));
            this._element.ForceRefresh(nameof(ToDoElement.InProgressElementsNo));
            this._element.ForceRefresh(nameof(ToDoElement.FinishedElementsNo));
        }

        public static void Execute(ToDoElement element, int openElementsNo, int inProgressElementsNo, int finishedElementsNo)
        {
            var action = new ChangeCountAction(element, openElementsNo, inProgressElementsNo, finishedElementsNo);
            action.Execute();
            UndoRedoTracker.I.AddAction(action);
        }
    }
}
