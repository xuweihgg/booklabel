using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookLabel.LabelModule
{
    public static class TreeViewBehavior
    {
        public static readonly DependencyProperty SelectionChangedActionProperty =
         DependencyProperty.RegisterAttached("SelectionChangedAction", typeof(Action<object>), typeof(TreeViewBehavior), new PropertyMetadata(default(Action), OnSelectionChangedActionChanged));

        private static void OnSelectionChangedActionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var treeView = sender as TreeView;
            if (treeView == null) return;

            var action = GetSelectionChangedAction(treeView);

            if (action != null)
            {
                // Remove the next line if you don't want to invoke immediately. 
                InvokeSelectionChangedAction(treeView);
                treeView.SelectedItemChanged += TreeViewOnSelectedItemChanged;
            }
            else
            {
                treeView.SelectedItemChanged -= TreeViewOnSelectedItemChanged;
            }
        }

        private static void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            if (treeView == null) return;

            InvokeSelectionChangedAction(treeView);

        }

        private static void InvokeSelectionChangedAction(TreeView treeView)
        {
            var action = GetSelectionChangedAction(treeView);
            if (action == null) return;

            var selectedItem = treeView.GetValue(TreeView.SelectedItemProperty);

            action(selectedItem);
        }

        public static void SetSelectionChangedAction(TreeView treeView, Action<object> value)
        {
            treeView.SetValue(SelectionChangedActionProperty, value);
        }

        public static Action<object> GetSelectionChangedAction(TreeView treeView)
        {
            return (Action<object>)treeView.GetValue(SelectionChangedActionProperty);
        }


        public static readonly DependencyProperty DoubleClickActionProperty =
         DependencyProperty.RegisterAttached("DoubleClickAction", typeof(Action<object>), typeof(TreeViewBehavior), new PropertyMetadata(default(Action), DoubleClickActionChanged));

        private static void DoubleClickActionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var treeView = sender as TreeView;
            if (treeView == null) return;

            var action = GetDoubleClickAction(treeView);

            if (action != null)
            {
                // Remove the next line if you don't want to invoke immediately. 
                InvokeSelectionChangedAction(treeView);
                treeView.MouseDoubleClick += TreeView_MouseDoubleClick; ;
            }
            else
            {
                treeView.MouseDoubleClick -= TreeView_MouseDoubleClick;
            }
        }

        private static void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeView = sender as TreeView;
            if (treeView == null) return;

            InvokeMouseDoubleClickAction(treeView);
        }

        private static void InvokeMouseDoubleClickAction(TreeView treeView)
        {
            var action = GetDoubleClickAction(treeView);
            if (action == null) return;

            var selectedItem = treeView.GetValue(TreeView.SelectedItemProperty);

            action(selectedItem);
        }

        public static void SetDoubleClickAction(TreeView treeView, Action<object> value)
        {
            treeView.SetValue(DoubleClickActionProperty, value);
        }

        public static Action<object> GetDoubleClickAction(TreeView treeView)
        {
            return (Action<object>)treeView.GetValue(DoubleClickActionProperty);
        }
    }
}
