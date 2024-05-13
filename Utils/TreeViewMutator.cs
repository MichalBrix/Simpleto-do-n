using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TodoLists.Data;

namespace TodoLists.Utils
{
    public class TreeViewMutator
    {
        private ObservableCollection<ToDoElement> _toDoElements;
        private TreeSearcher _treeSearcher;
        private TreeView _treeView;
        private Mutators.FinalMutators _finalMutators;
        private Mutators.Selector _selector;
        
        public TreeViewMutator(TreeSearcher treeSearcher, TreeView treeView, ObservableCollection<ToDoElement> toDoElements)
        {
            _treeSearcher = treeSearcher;
            _treeView = treeView;
            this._selector = new Mutators.Selector();
            _toDoElements = toDoElements;
            this._finalMutators = new Mutators.FinalMutators();
        }

        public void ExpandElement (ToDoElement element)
        {
            element.IsExpanded = true;
        }

        public void CollapseElement (ToDoElement element) {
            element.IsExpanded = false;
        }

        public void SelectElementDown(ToDoElement source, bool omitSourceExpandable = false, ToDoElement lastEle = null)
        {
            if (source.IsExpanded == true && !omitSourceExpandable) { 
                if (source.Children.Count > 0)
                {
                    this._selector.SelectElement(source.Children[0], lastEle != null ? lastEle : source);
                    return;
                }
            }

            var parent = this._treeSearcher.FindParent(source);
            if (parent == null)
            {
                int i = this._treeSearcher.GetTopLevelIndenx(source);
                if (i < this._toDoElements.Count - 1)
                {
                    this._selector.SelectElement(this._toDoElements[i + 1], lastEle != null ? lastEle : source);
                    return;
                }
            }
            if (parent != null)
            {
                var i = parent.GetIndexOf(source);
                if (i < parent.Children.Count - 1)
                {
                    this._selector.SelectElement(parent.Children[i + 1], lastEle != null ? lastEle : source);
                    return;
                }
                else
                {
                    SelectElementDown(parent, true, lastEle != null ? lastEle : source);
                }

            }
            return;
        }

        public void SelectElementUp(ToDoElement source)
        {
            var parent = this._treeSearcher.FindParent(source);
            if (parent == null)
            {
                int i = this._treeSearcher.GetTopLevelIndenx(source);
                if (i > 0)
                {
                    this._selector.SelectLastVisibleElement(this._toDoElements[i - 1], source);
                    return;
                }
            }
            if (parent != null)
            {
                var i = parent.GetIndexOf(source);
                if (i > 0)
                {
                    this._selector.SelectLastVisibleElement(parent.Children[i - 1], source);
                    return;
                }
                if (i == 0)
                {
                    this._selector.SelectElement(parent, source);
                    return;
                }

            }
            return;
        }

        public void DropElementUp(ToDoElement dropContext, ToDoElement droppedElement)
        {
            var dropData = this._treeSearcher.GetIndexAndParentCollectionFromElement(droppedElement);
            var eleData = this._treeSearcher.GetIndexAndParentCollectionFromElement(dropContext);
            if (eleData.Index == 0)
            {
                if (eleData.Parent != null)
                {
                    var parentData = this._treeSearcher.GetIndexAndParentCollectionFromElement(eleData.Parent);
                    if (parentData.Index == 0)
                    {
                        this._finalMutators.RebaseAsFirst(droppedElement, parentData.ParentList, dropData.ParentList);
                    }
                    else
                    {
                        this._finalMutators.RebaseOnIndex(droppedElement, parentData.Index, parentData.ParentList, dropData.ParentList);
                    }
                }
            }
            else
            {
                if (!eleData.ParentList[eleData.Index - 1].IsExpanded)
                {
                    this._finalMutators.RebaseOnIndex(droppedElement, eleData.Index, eleData.ParentList, dropData.ParentList);
                }
                else
                {
                    var finalParent = this._treeSearcher.FindDeepestLastExpandedElement(eleData.ParentList[eleData.Index - 1]);
                    this._finalMutators.RebaseAsLast(droppedElement, finalParent.Children, dropData.ParentList);
                }
            }
        }

        public void MoveElementUp(ToDoElement ele)
        {
            var eleData = this._treeSearcher.GetIndexAndParentCollectionFromElement(ele);
            if (eleData.Index == 0)
            {
                if (eleData.Parent != null)
                {
                    var parentData = this._treeSearcher.GetIndexAndParentCollectionFromElement(eleData.Parent);
                    if (parentData.Index == 0)
                    {
                        this._finalMutators.RebaseAsFirst(ele, parentData.ParentList, eleData.ParentList);
                    }
                    else
                    {
                        this._finalMutators.RebaseOnIndex(ele, parentData.Index, parentData.ParentList, eleData.ParentList);
                    }
                }
            }
            else
            {
                if (!eleData.ParentList[eleData.Index - 1].IsExpanded)
                {
                    this._finalMutators.RebaseOnIndex(ele, eleData.Index - 1, eleData.ParentList, eleData.ParentList);
                }
                else
                {
                    var finalParent = this._treeSearcher.FindDeepestLastExpandedElement(eleData.ParentList[eleData.Index - 1]);
                    this._finalMutators.RebaseAsLast(ele, finalParent.Children, eleData.ParentList);
                }
            }
        }

        public void DropElementDown(ToDoElement dropContext, ToDoElement droppedElement)
        {
            var dropData = this._treeSearcher.GetIndexAndParentCollectionFromElement(droppedElement);
            var eleData = this._treeSearcher.GetIndexAndParentCollectionFromElement(dropContext);
            if (eleData.Index != eleData.ParentList.Count - 1)
            {
                var possibleNewParent = eleData.ParentList[eleData.Index + 1];

                if (!possibleNewParent.IsExpanded)
                {
                    this._finalMutators.RebaseOnIndex(droppedElement, eleData.Index + 1, eleData.ParentList, dropData.ParentList);
                }
                else
                {
                    this._finalMutators.RebaseAsFirst(droppedElement, possibleNewParent.Children, dropData.ParentList);
                }
            }
            else
            {
                if (eleData.Parent == null)
                {
                    return;
                }
                var parentOfParent = this._treeSearcher.GetIndexAndParentCollectionFromElement(eleData.Parent);
                this._finalMutators.RebaseOnIndex(droppedElement, parentOfParent.Index + 1, parentOfParent.ParentList, dropData.ParentList);
            }
        }

        public void MoveElementDown(ToDoElement ele)
        {
            var eleData = this._treeSearcher.GetIndexAndParentCollectionFromElement(ele);
            if (eleData.Index != eleData.ParentList.Count - 1)
            {
                var possibleNewParent = eleData.ParentList[eleData.Index + 1];

                if (!possibleNewParent.IsExpanded)
                {
                    this._finalMutators.RebaseOnIndex(ele, eleData.Index + 2, eleData.ParentList, eleData.ParentList);
                }
                else
                {
                    this._finalMutators.RebaseAsFirst(ele, possibleNewParent.Children, eleData.ParentList);
                }
            }
            else
            {
                if (eleData.Parent == null)
                {
                    return;
                }
                var parentOfParent = this._treeSearcher.GetIndexAndParentCollectionFromElement(eleData.Parent);
                this._finalMutators.RebaseOnIndex(ele, parentOfParent.Index+1, parentOfParent.ParentList, eleData.ParentList);
            }
        }


        public void DeleteElement(ToDoElement ele)
        {
            var eleData = this._treeSearcher.GetIndexAndParentCollectionFromElement(ele);
            this._finalMutators.RemoveElement(ele, eleData.ParentList);
        }

        public void AddChildToElement(ToDoElement ele)
        {
            ToDoElement newElement = this._finalMutators.AddNewChildToElement(ele);
            newElement.IsTextBoxFocused = true;
        }
    }
}
