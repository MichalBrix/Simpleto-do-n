using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpletodon.Data;

namespace Simpletodon.Utils.Mutators.Actions
{
    internal class RemoveElementFromListAction : IAction
    {
        public ActionType ActionType { get { return ActionType.AddElementToList; } }

        private ObservableCollection<ToDoElement> _collection;
        private int _index;
        private ToDoElement _element;


        public RemoveElementFromListAction(ObservableCollection<ToDoElement> collection, int index, ToDoElement element)
        {
            this._collection = collection;
            this._index = index;
            this._element = element;
        }

        public void Execute()
        {
            this._collection.RemoveAt(this._index);
        }
        public void Undo()
        {
            this._collection.Insert(this._index, this._element);
        }

        public static void Execute(ObservableCollection<ToDoElement> collection, int index, ToDoElement element)
        {
            var action = new RemoveElementFromListAction(collection, index, element);
            action.Execute();
            UndoRedoTracker.I.AddAction(action);
        }
    }
}
