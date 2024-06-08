using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Simpletodon.Data;
using Simpletodon.Utils.Mutators.Actions;

namespace Simpletodon.Utils.Mutators
{
    public class FinalMutators
    {
        public void RebaseAsLast(ToDoElement element, ObservableCollection<ToDoElement> newParentCollection, ObservableCollection<ToDoElement> oldParentCollection)
        {
            RemoveElementFromListAction.Execute(oldParentCollection, oldParentCollection.IndexOf(element), element);
            AddElementToListAction.Execute(newParentCollection, newParentCollection.Count, element);
        }
        public void RebaseAsFirst(ToDoElement element, ObservableCollection<ToDoElement> newParentCollection, ObservableCollection<ToDoElement> oldParentCollection)
        {
            RemoveElementFromListAction.Execute(oldParentCollection, oldParentCollection.IndexOf(element), element);
            AddElementToListAction.Execute(newParentCollection, 0, element);
        }

        public void RebaseOnIndex(ToDoElement element, int newIndex, ObservableCollection<ToDoElement> newParentCollection, ObservableCollection<ToDoElement> oldParentCollection)
        {
            var oldIndex = oldParentCollection.IndexOf(element);
            if (oldParentCollection == newParentCollection)
            {
                if (oldIndex == newIndex)
                {
                    return;
                }
                if (oldIndex > newIndex)
                {
                    AddElementToListAction.Execute(newParentCollection, newIndex, element);
                    RemoveElementFromListAction.Execute(oldParentCollection, oldIndex + 1, element);
                }
                else
                {
                    AddElementToListAction.Execute(newParentCollection, newIndex, element);
                    RemoveElementFromListAction.Execute(oldParentCollection, oldIndex, element);
                }
            }
            else
            {
                RemoveElementFromListAction.Execute(oldParentCollection, oldIndex, element);
                AddElementToListAction.Execute(newParentCollection, newIndex, element);
            }
        }

        public void RemoveElement(ToDoElement element, ObservableCollection<ToDoElement> parentCollection)
        {
            RemoveElementFromListAction.Execute(parentCollection, parentCollection.IndexOf(element), element);
        }

        public ToDoElement AddNewChildToElement(ToDoElement parent)
        {
            var newEle = new ToDoElement();
            AddElementToListAction.Execute(parent.Children, parent.Children.Count, newEle);
            return newEle;
        }

        public ToDoElement AddNewChildToElement(int index, ObservableCollection<ToDoElement> list)
        {
            var newEle = new ToDoElement();
            AddElementToListAction.Execute(list, index, newEle);
            return newEle;
        }

        public void ChangeStatus(ToDoElement element, bool isWorkInProgress, bool isFinished)
        {
            ChangeStatusAction.Execute(element, isWorkInProgress, isFinished);
        }

        public void ChangeExpandedStatus(ToDoElement element, bool value)
        {
            ChangeExpandedStatusAction.Execute(element, value);
        }

        public void SetCount(ToDoElement element, int openElementsNo, int inProgressElementsNo, int finishedElementsNo)
        {
            ChangeCountAction.Execute(element, openElementsNo, inProgressElementsNo, finishedElementsNo);
        }
    }
}
