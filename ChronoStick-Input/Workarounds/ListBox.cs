﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChronoStick_Input.Workarounds
{
    public static class ListBox
    {

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(System.Windows.Controls.ListBox),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AutoScrollHandlerProperty =
            DependencyProperty.RegisterAttached("AutoScrollHandler", typeof(AutoScrollHandler), typeof(System.Windows.Controls.ListBox));

        public static bool GetAutoScroll(System.Windows.Controls.ListBox instance)
        {
            return (bool)instance.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(System.Windows.Controls.ListBox instance, bool value)
        {
            AutoScrollHandler OldHandler = (AutoScrollHandler)instance.GetValue(AutoScrollHandlerProperty);
            if (OldHandler != null)
            {
                OldHandler.Dispose();
                instance.SetValue(AutoScrollHandlerProperty, null);
            }
            instance.SetValue(AutoScrollProperty, value);
            if (value)
                instance.SetValue(AutoScrollHandlerProperty, new AutoScrollHandler(instance));
        }

    }  
}
