using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TodoLists.Data;

namespace TodoLists.Utils
{
    public class TreeSearcher
    {
        private ObservableCollection<ToDoElement> _toDoElements;
        public TreeSearcher(ObservableCollection<ToDoElement> toDoElements)
        {
            _toDoElements = toDoElements;
        }

        public ToDoElement? GetElementByGuid(string guidStr)
        {
            var guid = Guid.Parse(guidStr);
            ToDoElement? ele = null;
            for (int i = 0; i < _toDoElements.Count && ele == null; i++)
            {
                ele = this._toDoElements[i].GetElementByGuid(guid);
            }
            return ele;
        }

        public int GetTopLevelIndenx(ToDoElement element)
        {
            for (int i = 0; i < _toDoElements.Count; i++)
            {
                if (element == _toDoElements[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public ToDoElement FindParent(ToDoElement element)
        {
            var parentCollectionIndex = this.IsPartOfParentCollection(element);
            if (parentCollectionIndex != -1)
            {
                return null;
            }
            for (int i = 0; i < this._toDoElements.Count; i++)
            {
                ToDoElement possibility = this.FindParent(this._toDoElements[i], element);
                if (possibility != null)
                {
                    return possibility;
                }
            }
            return null;
        }

        private ToDoElement FindParent(ToDoElement elementToCheck, ToDoElement childToFind)
        {
            for (int i = 0; i < elementToCheck.Children.Count; i++)
            {
                if (elementToCheck.Children[i] == childToFind)
                {
                    return elementToCheck;
                }
                else
                {
                    var possibility = FindParent(elementToCheck.Children[i], childToFind);
                    if (possibility != null)
                    {
                        return possibility;
                    }
                }
            }
            return null;
        }

        public int IsPartOfParentCollection(ToDoElement ele) { 
            for (int i = 0; i < _toDoElements.Count; i++)
            {
                if (ele == _toDoElements[i])
                    return i;
            }
            return -1;
        }

        public ToDoElement FindDeepestLastExpandedElement(ToDoElement eleToCheck)
        {
            if (eleToCheck.Children.Count == 0)
            {
                return eleToCheck;
            }
            ToDoElement lastChild = eleToCheck.Children.Last();
            if (!lastChild.IsExpanded)
            {
                return eleToCheck;
            }
            return FindDeepestLastExpandedElement(lastChild);
        }

        public (int Index, ObservableCollection<ToDoElement> ParentList, ToDoElement? Parent) GetIndexAndParentCollectionFromElement(ToDoElement element)
        {
            int elementIndex = this._toDoElements.IndexOf(element);
            if (elementIndex > -1)
            {
                return (elementIndex, this._toDoElements, null);
            }

            ToDoElement parent = this.FindParent(element);
            return (parent.Children.IndexOf(element), parent.Children, parent);
        }
    }
}
