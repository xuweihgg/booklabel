using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BookLabel.LabelModule
{
    public class TreeViewMouseRightButtonSelectionBehavior:Behavior<TreeView>
    {
        public TreeViewMouseRightButtonSelectionBehavior()
        {

        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_PreviewMouseRightButtonDown);
        }

        void AssociatedObject_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject sourse = e.OriginalSource as DependencyObject;
            if(sourse !=null)
            {
                var tvi = ViewHelper.TryFindParentWPF<TreeViewItem>(sourse);
                if (tvi != null)
                    tvi.IsSelected = true;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseRightButtonDown -= new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_PreviewMouseRightButtonDown);
        }
    }
}
