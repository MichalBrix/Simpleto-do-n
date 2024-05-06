using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoLists.Data;

namespace TodoLists.Utils.Mutators
{
    internal class Selector
    {
        public void SelectLastVisibleElement(ToDoElement source, ToDoElement lastEle)
        {
            if (source.IsExpanded == false || source.Children.Count == 0)
            {
                SelectElement(source, lastEle);
                return;
            }
            SelectLastVisibleElement(source.Children.Last(), lastEle);
        }

        public void SelectElement(ToDoElement ele, ToDoElement lastEle)
        {
            lastEle.IsTextBoxFocused = false;
            ele.IsSelected = true;
            ele.IsTextBoxFocused = true;

        }
    }
}
