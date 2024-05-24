using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoLists.Data;

namespace TodoLists.Utils
{
    public class ElementCounter
    {
        private TreeSearcher _searcher;
        private ObservableCollection<ToDoElement> _toDoElements;

        public ElementCounter(ObservableCollection<ToDoElement> toDoElements, TreeSearcher _sercher)
        {
            _toDoElements = toDoElements;
            this._searcher = _sercher;
        }

        public void RecalculateElements()
        {
            foreach (var ele in _toDoElements)
            {
                this.CalculateElementDown(ele);
            }
        }

        public void CalculateElementDown(ToDoElement element)
        {
            foreach (var child in element.Children)
            {
                this.CalculateElementDown(child);
            }
            this.RecalculateNumbers(element);
        }

        public void CalculateParentUp(ToDoElement? element)
        {
            if (element != null)
            {
                var parent = this._searcher.FindParent(element);
                if (parent != null)
                {
                    this.CalculateElementUp(parent);
                }
            }
        }

        public void CalculateElementUp(ToDoElement? element)
        {
            if (element != null)
            {
                this.RecalculateNumbers(element);
                var parent = this._searcher.FindParent(element);
                if (parent != null)
                {
                    this.CalculateElementUp(parent);
                }
            }
        }

        private void RecalculateNumbers(ToDoElement element)
        {
            element.FinishedElementsNo = 0;
            element.InProgressElementsNo = 0;
            element.OpenElementsNo = 0;
            foreach (var child in element.Children)
            {
                if (child.IsFinished)
                {
                    element.FinishedElementsNo++;
                }
                else if (child.IsInProgress)
                {
                    element.InProgressElementsNo++;
                }
                else
                {
                    element.OpenElementsNo++;
                }
                element.FinishedElementsNo += child.FinishedElementsNo;
                element.InProgressElementsNo += child.InProgressElementsNo;
                element.OpenElementsNo += child.OpenElementsNo;
            }
        }
    }
}
