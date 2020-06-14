using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BookLabel.LabelModule
{
    public static class ViewHelper
    {
        public static T TryFindParentWPF<T>(DependencyObject child) where T : class
        {
			//get parent item     
			DependencyObject parentObject = GetParentObject(child);

			if (parentObject == null && child is System.Windows.Controls.Grid)
				parentObject = (child as System.Windows.Controls.Grid).Parent;

			//we've reached the end of the tree    
			if (parentObject == null)
			{
				return null;     //check if the parent matches the type we're looking for    
			}

			T parent = parentObject as T;

			if (parent != null)
			{
				return parent;
			}
			else
			{
				//use recursion to proceed with next level       
				return TryFindParentWPF<T>(parentObject);
			}
		}

		public static DependencyObject GetParentObject(DependencyObject child)
		{
			if (child == null)
				return null;

			ContentElement contentElement = child as ContentElement;

			DependencyObject parent = null;

			if (contentElement != null)
			{
				parent = ContentOperations.GetParent(contentElement);

				if (parent != null)
					return parent;

				FrameworkContentElement fce = contentElement as FrameworkContentElement;

				return fce != null ? fce.Parent : null;
			}

			//if it's not a ContentElement, rely on VisualTreeHelper     
			parent = VisualTreeHelper.GetParent(child);

			if (parent == null && child is FrameworkElement)
				return (child as FrameworkElement).Parent;

			return parent;
		}

	}

	public static class VisualTreeHelperEx
	{
		public static T FindVisualAncertorByTypes<T>(DependencyObject dependencyObject) where T:DependencyObject
		{
			if (dependencyObject == null) return default(T);
			if (dependencyObject is T) return (T)dependencyObject;
			T parent = default(T);
			parent = FindVisualAncertorByTypes<T>(VisualTreeHelper.GetParent(dependencyObject));
			return parent;
		}
	}
}
