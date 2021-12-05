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
    public class MenuViewModel
    {
        ICatalogDataService DataService;

        public MenuViewModel(ICatalogDataService dataService)
        {
            DataService = dataService;
            Catalogs = new ObservableCollection<CatalogConstruction>(dataService.GetLabelDetails());
            this.InsertCatalogCommand = new DelegateCommand(InsertCatalog);
            this.UpdateCatalogCommand = new DelegateCommand(UpdateCatalog);
            this.DeleteCatalogCommand = new DelegateCommand(DeleteCatalog);
            this.InsertChirdCommand = new DelegateCommand(InsertChird);
            OnTextModifyCommand += OnTextModifyChanged;
            SelectionChangedAction = new Action<object>((cc) =>
            {
                if (cc is CatalogConstruction)
                {
                    catalog = cc as CatalogConstruction;
                }
            });
            MouseDoubleClick = new Action<object>((cc) =>
            {
                if (catalog != null)
                {
                    catalog.IsInEditMode = true;
                }
            });
        }

        public ValueChanged OnTextModifyCommand;
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
        public Action<object> SelectionChangedAction { get; set; }

        public Action<object> MouseDoubleClick { get; set; }

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
            DataService.InsertLable(catalog);
            updateFlag = 0;
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

    }
}
