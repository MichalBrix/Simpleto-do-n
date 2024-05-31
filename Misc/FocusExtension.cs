using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;

namespace TodoLists.Misc
{
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
             "IsFocused", typeof(bool), typeof(FocusExtension),
             new UIPropertyMetadata(false, null, OnCoerceValue));

        private static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            if ((bool)baseValue)
            {
                Debug.WriteLine("Focusing");
                ((UIElement)d).Focus();
            }
            else if (((UIElement)d).IsFocused) {
                Debug.WriteLine("ClearFocus");
                Keyboard.ClearFocus();
            }
            return ((bool)baseValue);
        }
    }

}
