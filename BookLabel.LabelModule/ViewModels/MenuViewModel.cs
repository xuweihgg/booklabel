using BookLabel.LabelModule.Controls;
using BookLabel.LabelModule.Models;
using BookLabel.LabelModule.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using static BookLabel.LabelModule.Controls.EditableTextBox;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace BookLabel.LabelModule.ViewModels
{
    public class MenuViewModel: INotifyPropertyChanged
    {
        public MenuViewModel()
        {
            Catalogs = new ObservableCollection<CatalogConstruction>(DataCoach.GetInstance().GetCatalogs());
            this.BookLabelDetails = new ObservableCollection<BookLabelDetail>();
            this.InsertCatalogCommand = new DelegateCommand(InsertCatalog);
            this.UpdateCatalogCommand = new DelegateCommand(UpdateCatalog);
            this.DeleteCatalogCommand = new DelegateCommand(DeleteCatalog);
            this.InsertChirdCommand = new DelegateCommand(InsertChird);
            this.OnTextModifyCommand = new DelegateCommand<string>((cc) => { OnTextModifyChanged(cc); });
            SelectionChangedAction = new Action<object>(cc => SelectedChanage(cc));
            MouseDoubleClick = new Action<object>((cc) => TreeViewDoubleClick(cc));

            AddBoolLabelCommand = new DelegateCommand(OnAddBoolLabel);
            DeleteBoolLabelCommand = new DelegateCommand(OnDeleteBoolLabel);
            OpenFileCommand = new DelegateCommand(OnOpenFile);
            var labels = DataCoach.GetInstance().GetBookLabelDetais();
            BookLables = new ObservableCollection<string>(labels.Select((x) => x.BoolLabelName).Distinct());
            PreviewDragOverCommand = new Action<object>((cc) => OnPreviewDragOver(cc));
            PreviewDropCommand = new Action<object>((cc) => OnPreviewDrop(cc));
            BatchInsertCommand = new DelegateCommand(BatchInsertDetail);
        }

        #region 左侧菜单操作
        /// <summary>
        /// 目录描述改变
        /// </summary>
        public DelegateCommand<string> OnTextModifyCommand { get; set; }
        /// <summary>
        /// 更新目录信息
        /// </summary>
        public DelegateCommand UpdateCatalogCommand { get; set; }
        /// <summary>
        /// 插入子节点
        /// </summary>
        public DelegateCommand InsertChirdCommand { get; set; }
        /// <summary>
        /// 删除节点
        /// </summary>
        public DelegateCommand DeleteCatalogCommand { get; set; }
        /// <summary>
        /// 树双击事件
        /// </summary>
        public Action<object> MouseDoubleClick { get; set; }
        /// <summary>
        /// 树控件选中事件
        /// </summary>
        public Action<object> SelectionChangedAction { get; set; }
        /// <summary>
        /// 树绑定的实体类
        /// </summary>
        public ObservableCollection<CatalogConstruction> Catalogs { get; set; }

        /// <summary>
        /// 明细数据，分为两个维度展示，目录维度和书签维度
        /// </summary>
        public ObservableCollection<BookLabelDetail> BookLabelDetails { get; set; }
        public CatalogConstruction Catalog { get; set; }
        /// <summary>
        /// 0-新增，1-修改，2-删除
        /// </summary>
        private int updateFlag = 0;
        public DelegateCommand InsertCatalogCommand { get; set; }
        public void OnTextModifyChanged(string args)
        {
            if (Catalog == null)
                return;

            if (args == null)
                return;

            string text = args.ToString();
            string oldValue = Catalog.CatalogName;

            if (oldValue == text)
                return;


            try
            {
                Catalog.CatalogName = text;
                if (updateFlag == 0)
                    DataCoach.GetInstance().AddCatalog(Catalog);
                else if (updateFlag == 1)
                    DataCoach.GetInstance().UpdateCatalog(Catalog);
                oldValue = text;
            }
            catch (Exception ex)
            {
                Catalog.CatalogName = oldValue;
                if (updateFlag == 0)
                    Catalogs.Remove(Catalog);

                MessageBox.Show(ex.Message);
            }
        }
      
        public void InsertCatalog()
        {
            Catalog = new CatalogConstruction(Guid.NewGuid().ToString(), "目录1");
            Catalog.IsInEditMode = true;
            Catalogs.Add(Catalog);
            DataCoach.GetInstance().AddCatalog(Catalog);
            updateFlag = 0;
        }

        public void UpdateCatalog()
        {
            if (Catalog == null)
                return;

            updateFlag = 1;

            Catalog.IsInEditMode = true;
        }

        public void DeleteCatalog()
        {
            updateFlag = 2;
            try
            {
                DataCoach.GetInstance().DeletCatalog(Catalog);
                Catalogs.Remove(Catalog);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertChird()
        {
            if (Catalog != null)
            {
                var chcata = new CatalogConstruction(Guid.NewGuid().ToString(), "子目录1", Catalog.CatalogId);
                Catalog.ChirdCatalogs.Add(chcata);
                DataCoach.GetInstance().AddCatalog(chcata);
            }
        }

        private void SelectedChanage(object cc)
        {
            BookLabelDetails.Clear();
            if (cc is CatalogConstruction)
            {
                Catalog = cc as CatalogConstruction;
                var items = DataCoach.GetInstance().GetBookLabelDetais().Where(x => x.CatalogId == Catalog.CatalogId).ToList();
                items.ForEach(x => BookLabelDetails.Add(x));
            }
        }

        private void TreeViewDoubleClick(object cc)
        {
            if (Catalog != null)
            {
                Catalog.IsInEditMode = true;
                updateFlag = 1;
            }
        }

        public ObservableCollection<string> BookLables { get; set; }
        public string SelectedLabel { get; set; }
        #endregion 左侧菜单操作

        #region 明细操作界面
        public string LableName { get; set; }
        public string LabelPath { get; set; }

        public DelegateCommand OpenFileCommand { get; set; }

        private void OnOpenFile()
        {
            OpenFileDialog sd = new OpenFileDialog();
            sd.Filter = "所有文件 (*.*)|*.*";
            sd.Title = "请选择要导入的文件";
            sd.Multiselect = true;
            if (sd.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(sd.FileName) && !sd.Multiselect)
                    LabelPath = sd.FileName;
                if (sd.Multiselect)
                {
                    var filenames = sd.FileNames;
                    LabelPath = filenames.Aggregate((s, e) => s + e);
                }
            }
        }

        public DelegateCommand GotFocus { get; set; }
        private void OnGotFocus()
        {
            
        }

        public DelegateCommand AddBoolLabelCommand { get; set; }
        private void OnAddBoolLabel()
        {
            if (Catalog == null)
                return;
            if (string.IsNullOrWhiteSpace(LableName))
                return;
            if (string.IsNullOrWhiteSpace(LabelPath))
                return;
            var detail = new BookLabelDetail { BookLabelId = Guid.NewGuid().ToString(), BoolLabelName = LableName, CatalogId = Catalog.CatalogId, CreateTime = DateTime.Now, LabelPath = LabelPath };
            BookLabelDetails.Add(detail);
            DataCoach.GetInstance().InsertLable(detail);

        }
        public BookLabelDetail SelectedBookLabel { get; set; }
        public DelegateCommand DeleteBoolLabelCommand { get; set; }
        private void OnDeleteBoolLabel()
        {
            if (Catalog == null)
                return;
            if (SelectedBookLabel == null)
                return;
            BookLabelDetails.Remove(SelectedBookLabel);
            DataCoach.GetInstance().DeletLable(SelectedBookLabel);
        }
        public Action<object> PreviewDragOverCommand { get; set; }
        public void OnPreviewDragOver(object obj)
        {
            if (obj is DragEventArgs)
            {
                var e = obj as DragEventArgs;
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effects = DragDropEffects.Link;
                    e.Handled = true;
                }
                else
                    e.Effects = DragDropEffects.None;
            }
        }

        public Action<object> PreviewDropCommand { get; set; }
        public void OnPreviewDrop(object obj)
        {
            if (obj is DragEventArgs)
            {
                var e = obj as DragEventArgs;
                StringBuilder builder = new StringBuilder();
                foreach (var item in ((System.Array)e.Data.GetData(DataFormats.FileDrop)))
                {
                    builder.AppendLine(item.ToString());
                }
                DropName = builder.ToString();
            }
        }
        private string dropName;
        public string DropName 
        { get
            {
                return dropName;
            } set
            {
                dropName = value;
                NotifyPropertyChanged("DropName");
            } }

        public DelegateCommand BatchInsertCommand { get; set; }
        private void BatchInsertDetail()
        {
            if (Catalog == null)
                return;
          
            if (!string.IsNullOrWhiteSpace(DropName))
            {
                var strs = DropName.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strs)
                {
                    string filename = Path.GetFileName(item);
                    var detail = new BookLabelDetail { BookLabelId = Guid.NewGuid().ToString(), BoolLabelName = LableName, CatalogId = Catalog.CatalogId, CreateTime = DateTime.Now, LabelPath = item };
                    BookLabelDetails.Add(detail);
                    DataCoach.GetInstance().InsertLable(detail);
                }
            }
        }
        #endregion 明细操作界面

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
