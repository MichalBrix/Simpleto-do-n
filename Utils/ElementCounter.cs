using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpletodon.Data;
using Simpletodon.Utils.Mutators;

namespace Simpletodon.Utils
{
    public class ElementCounter
    {
        private TreeSearcher _searcher;
        private ObservableCollection<ToDoElement> _toDoElements;
        private FinalMutators _mutator;

        public ElementCounter(ObservableCollection<ToDoElement> toDoElements, TreeSearcher _sercher, FinalMutators mutator)
        {
            _toDoElements = toDoElements;
            this._searcher = _sercher;
            this._mutator= mutator;
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
            int finishedElementsNo = 0;
            int inProgressElementsNo = 0;
            int openElementsNo = 0;
            foreach (var child in element.Children)
            {
                if (child.IsFinished)
                {
                    finishedElementsNo++;
                }
                else if (child.IsInProgress)
                {
                    inProgressElementsNo++;
                }
                else
                {
                    openElementsNo++;
                }
                finishedElementsNo += child.FinishedElementsNo;
                inProgressElementsNo += child.InProgressElementsNo;
                openElementsNo += child.OpenElementsNo;
            }

            this._mutator.SetCount(element, openElementsNo, inProgressElementsNo, finishedElementsNo);
        }
    }
}
