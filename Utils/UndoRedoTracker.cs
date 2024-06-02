using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoLists.Utils.Mutators.Actions;

namespace TodoLists.Utils
{
    public class UndoRedoTracker
    {
        public static UndoRedoTracker I { get; } = new UndoRedoTracker();

        public bool SuppressTracking = false;

        private List<IAction> CurrentStack = new List<IAction>();    

        private List<List<IAction>> historyForUndo = new List<List<IAction>>();
        private List<List<IAction>> historyForRedo = new List<List<IAction>>();

        public void AddAction(IAction action)
        {
            if (!this.SuppressTracking)
            {
                this.CurrentStack.Add(action);
            }
        }

        public void FinishTransaction()
        {
            if (this.CurrentStack.Count > 0)
            {
                this.historyForUndo.Add(this.CurrentStack);
                this.CurrentStack = new List<IAction>();
                this.historyForRedo.Clear();
            }
        }

        public void Undo()
        {
            if (this.historyForUndo.Count == 0)
            {
                return;
            }
            FinishTransaction();
            var lastTransaction = this.historyForUndo.TakeLast(1).FirstOrDefault();
            this.historyForUndo.RemoveAt(this.historyForUndo.Count - 1);
            if (lastTransaction == null)
            {
                return;
            }
            UndoActions(lastTransaction);
            this.historyForRedo.Add(lastTransaction);
        }

        private void UndoActions(List<IAction> actions)
        {
            for (int i = actions.Count() - 1; i >= 0; i--)
            {
                var action = actions[i];
                action.Undo();
            }
        }

        public void Redo()
        {
            if (this.historyForRedo.Count == 0)
            {
                return;
            }
            this.FinishTransaction();
            var lastTransaction = this.historyForRedo.TakeLast(1).FirstOrDefault();
            this.historyForRedo.RemoveAt(this.historyForRedo.Count - 1);
            if (lastTransaction == null)
            {
                return;
            }
            foreach (var action in lastTransaction)
            {
                action.Execute();
            }
            this.historyForUndo.Add(lastTransaction);
        }
    }
}
