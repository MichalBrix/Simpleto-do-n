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
            if (oldParentCollection == newParentCollection)
            {
                var oldIndex = oldParentCollection.IndexOf(element);
                if (oldIndex == newIndex)
                {
                    return;
                }
                if (oldIndex > newIndex)
                {
                    newParentCollection.Insert(newIndex, element);
                    oldParentCollection.RemoveAt(oldIndex + 1);
                }
                else
                {
                    newParentCollection.Insert(newIndex, element);
                    oldParentCollection.RemoveAt(oldIndex);
                }
            }
            else
            {
                oldParentCollection.Remove(element);
                newParentCollection.Insert(newIndex, element);
            }
        }

        public void RemoveElement(ToDoElement element, ObservableCollection<ToDoElement> parentCollection)
        {
            parentCollection.Remove(element);
        }

        public ToDoElement AddNewChildToElement( ToDoElement parent)
        {
            var newEle = new ToDoElement();
            parent.Children.Add(newEle);

            return newEle;
        }

        public ToDoElement AddNewChildToElement(ToDoElement parent, int index, ObservableCollection<ToDoElement> list)
        {
            var newEle = new ToDoElement();
            list.Insert(index, newEle);
            return newEle;
        }

        public void ChangeStatus(ToDoElement element, bool isWorkInProgress, bool isFinished)
        {
            element.IsFinished = isFinished;
            element.IsInProgress = isWorkInProgress;
        }
    }
}
