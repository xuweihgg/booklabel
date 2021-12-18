using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookLabel.LabelModule
{
    public static  class TextBoxBehavior
    {
        public static void SetPreviewDragOverAction(TextBox treeView, Action<object> value)
        {
            treeView.SetValue(PreviewDragOverActionProperty, value);
        }

        public static Action<DragEventArgs> GetPreviewDragOverAction(TextBox treeView)
        {
            return (Action<DragEventArgs>)treeView.GetValue(PreviewDragOverActionProperty);
        }

        public static readonly DependencyProperty PreviewDragOverActionProperty =
        DependencyProperty.RegisterAttached("PreviewDragOverAction", typeof(Action<object>), typeof(TextBoxBehavior), new PropertyMetadata(default(Action), PreviewDragOverActionChanged));

        private static void PreviewDragOverActionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var action = GetPreviewDragOverAction(textBox);

            if (action != null)
            {
                // Remove the next line if you don't want to invoke immediately. 
                textBox.PreviewDragOver += TextBox_PreviewDragOver; 
            }
            else
            {
                textBox.PreviewDragOver -= TextBox_PreviewDragOver;
            }
        }

        private static void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            InvokePreviewDragOverAction(textBox, e);
        }

        private static void InvokePreviewDragOverAction(TextBox textBox,  DragEventArgs e)
        {
            var action = GetPreviewDragOverAction(textBox);
            if (action == null) return;
            if(e!=null)
            action(e);
        }




        public static void SetPreviewDropAction(TextBox treeView, Action<object> value)
        {
            treeView.SetValue(PreviewDropActionProperty, value);
        }

        public static Action<DragEventArgs> GetPreviewDropAction(TextBox treeView)
        {
            return (Action<DragEventArgs>)treeView.GetValue(PreviewDropActionProperty);
        }

        public static readonly DependencyProperty PreviewDropActionProperty =
        DependencyProperty.RegisterAttached("PreviewDropAction", typeof(Action<object>), typeof(TextBoxBehavior), new PropertyMetadata(default(Action), PreviewDropActionChanged));

        private static void PreviewDropActionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var action = GetPreviewDropAction(textBox);

            if (action != null)
            {
                // Remove the next line if you don't want to invoke immediately. 
                textBox.PreviewDrop += TextBox_PreviewDrop;
            }
            else
            {
                textBox.PreviewDrop -= TextBox_PreviewDrop;
            }
        }

        private static void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            InvokePreviewDropAction(textBox, e);
        }

        private static void InvokePreviewDropAction(TextBox textBox, DragEventArgs e)
        {
            var action = GetPreviewDropAction(textBox);
            if (action == null) return;
            if (e != null)
                action(e);
        }
    }
}
