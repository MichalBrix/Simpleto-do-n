using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoLists.Data;

namespace TodoLists.Utils.Mutators
{
    internal class FinalMutators
    {
        public void RebaseAsLast(ToDoElement element, ObservableCollection<ToDoElement> newParentCollection, ObservableCollection<ToDoElement> oldParentCollection)
        {
            oldParentCollection.Remove(element);
            newParentCollection.Add(element);
        }
        public void RebaseAsFirst(ToDoElement element, ObservableCollection<ToDoElement> newParentCollection, ObservableCollection<ToDoElement> oldParentCollection)
        {
            oldParentCollection.Remove(element);
            newParentCollection.Insert(0, element);
        }

        public void RebaseOnIndex(ToDoElement element, int newIndex, ObservableCollection<ToDoElement> newParentCollection, ObservableCollection<ToDoElement> oldParentCollection)
        {
            oldParentCollection.Remove(element);
            newParentCollection.Insert(newIndex, element);
        }
    }
}
