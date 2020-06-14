using BookLabel.LabelModule.Controls;
using BookLabel.LabelModule.Models;
using BookLabel.LabelModule.Services;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static BookLabel.LabelModule.Controls.EditableTextBox;

namespace BookLabel.LabelModule.ViewModels
{
    public class MenuViewModel:IDisposable
    {
        ICatalogDataService DataService;
        TreeView treeView;
        private UserControl UserControl;
        public MenuViewModel(ICatalogDataService dataService)
        {
            DataService = dataService;
            Catalogs = new ObservableCollection<CatalogConstruction>(dataService.GetLabelDetails());
            this.InsertCatalogCommand = new DelegateCommand(InsertCatalog);
            this.UpdateCatalogCommand = new DelegateCommand(UpdateCatalog);
            this.DeleteCatalogCommand = new DelegateCommand(DeleteCatalog);
            this.InsertChirdCommand = new DelegateCommand(InsertChird);
        }

        public object GetElementByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var element = UserControl.FindName(name);

            if (element == null)
                element = ((FrameworkElement)UserControl).FindName(name);

            if (element == null && UserControl.Content is FrameworkElement)
                element = (UserControl.Content as FrameworkElement).FindName(name);

            return element;
        }


        public void SetControl(UserControl control)
        {
            UserControl = control;
            treeView = GetElementByName("tree1") as TreeView;
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
            this.treeView.AddHandler(EditableTextBox.OnTextModifyCompleteEvent, new ValueChanged(OnTextModifyChanged));
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (catalog != null)
            {
                catalog.IsInEditMode = true;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            catalog = e.NewValue as CatalogConstruction;
        }

        private CatalogConstruction catalog;
        /// <summary>
        /// 0-新增，1-修改，2-删除
        /// </summary>
        private int updateFlag = 0;
        public DelegateCommand InsertCatalogCommand { get; set; }
        public void InsertCatalog()
        {
                catalog = new CatalogConstruction(Guid.NewGuid().ToString(), "目录1");
                catalog.IsInEditMode = true;
                Catalogs.Add(catalog);
                updateFlag = 0;
        }

        public void OnTextModifyChanged(object sender, LostFocusEventArgs args)
        {
            if (catalog == null)
                return;

            if (args.NewValue == null)
                return;

            string text = args.NewValue.ToString();
            string oldValue = catalog.CatalogName;

            if (oldValue == text)
                return;

           
            try
            {
                catalog.CatalogName = text;
                if (updateFlag == 0)
                    DataService.InsertLable(catalog);
                else if (updateFlag == 1)
                    DataService.Update(catalog);
                oldValue = text;
            }
            catch (Exception ex)
            {
                catalog.CatalogName = oldValue;
                if (updateFlag == 0)
                    Catalogs.Remove(catalog);

                MessageBox.Show(ex.Message);
            }
        }

        public DelegateCommand UpdateCatalogCommand { get; set; }

        public void UpdateCatalog()
        {
            if (catalog == null)
                return;

            updateFlag = 1;

            catalog.IsInEditMode = true;
        }

        public DelegateCommand DeleteCatalogCommand { get; set; }
        public void DeleteCatalog()
        {
            updateFlag = 2;
            try
            {
                DataService.Delete(catalog);
                Catalogs.Remove(catalog);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public DelegateCommand InsertChirdCommand { get; set; }
        private void InsertChird()
        {
            if (catalog != null)
            {
                catalog.ChirdCatalogs.Add(new CatalogConstruction(Guid.NewGuid().ToString(), "子目录1", catalog.CatalogId));
            }
        }
        public ObservableCollection<CatalogConstruction> Catalogs { get; set; }

        public void Dispose()
        {
            if (treeView != null)
            {
                treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;
                treeView.MouseDoubleClick -= TreeView_MouseDoubleClick;
                this.treeView.RemoveHandler(EditableTextBox.OnTextModifyCompleteEvent, new ValueChanged(OnTextModifyChanged));
            }
        }
    }
}
