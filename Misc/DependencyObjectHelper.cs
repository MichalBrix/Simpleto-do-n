using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Simpletodon.Misc
{
    internal class DependencyObjectHelper
    {
        public static T? FindChildOfType<T>(DependencyObject parent) where T : System.Windows.DependencyObject
        {
            var queue = new Queue<DependencyObject>(new[] { parent });

            while (queue.Count > 0)
            {
                var possibleChild = queue.Dequeue();
                if (possibleChild is T foundChild)
                {
                    return foundChild;
                }
                var count = VisualTreeHelper.GetChildrenCount(possibleChild);

                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(possibleChild, i);
                    if (child is T foundChild2)
                        return foundChild2;

                    queue.Enqueue(child);
                }
            }
            return null;
        }
    }
}
