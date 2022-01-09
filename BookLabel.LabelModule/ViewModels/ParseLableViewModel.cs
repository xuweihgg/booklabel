using BookLabel.LabelModule.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookLabel.LabelModule.ViewModels
{
   public  class ParseLableViewModel : INotifyPropertyChanged
    {
        public ParseLableViewModel()
        {
            OpenFolderCommand = new DelegateCommand(OpenFolder);
            ParseFolderCommand = new DelegateCommand(ParseFolder);
            Catalogs = new ObservableCollection<CatalogConstruction>();
            AddBookLabelCommand = new DelegateCommand(AddBookLabel);
            ModifyBookLabelCommand = new DelegateCommand(ModifyBookLabel);
            DeleteLabelCommand = new DelegateCommand(DeleteLabel);
            DeleteLabelDetailCommand = new DelegateCommand(DeleteLabelDetail);
            BookLables = new ObservableCollection<string>();
            BookLabelDetails = new ObservableCollection<BookLabelDetail>();
            SelectionChangedAction = new Action<object>(cc => SelectedChanage(cc));
            Initialize();
        }

        private void Initialize()
        {
            var lists = DataCoach.GetInstance().GetBookLabelDetais();
            BookLables = new ObservableCollection<string>(lists.Select((x) => x.BoolLabelName).Distinct());
        }

        public Action<object> SelectionChangedAction { get; set; }

        public CatalogConstruction Catalog { get; set; }
        private void SelectedChanage(object cc)
        {
            if (cc is CatalogConstruction)
            {
                Catalog = cc as CatalogConstruction;
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (Catalog != null && Catalog.BookLabelDetails.Any())
                {
                    foreach (var item in Catalog.BookLabelDetails)
                        item.IsChecked = value;
                }
            }
        }
        private string folderPath;
        public string FolderPath { get { return folderPath; } set { folderPath = value;NotifyPropertyChanged("FolderPath"); } }

        public DelegateCommand OpenFolderCommand { get; set; }
        public DelegateCommand ParseFolderCommand { get; set; }
        private void OpenFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(FolderPath))
                    ParseFolder();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解析目录错误：{ex.Message}");
            }
        }
        public ObservableCollection<CatalogConstruction> Catalogs { get; set; }
        private void ParseFolder()
        {
            var catalogs =UtityHelper.GetRootDirectory(FolderPath);
            foreach (var item in catalogs)
                Catalogs.Add(item);
        }
        private string labelName;
        public string LabelName { get { return labelName; } set { labelName = value;NotifyPropertyChanged("LabelName"); } }
        public DelegateCommand AddBookLabelCommand { get; set; }
        private void AddBookLabel()
        {
            try
            {
                if (Catalog == null)
                    return;
                if (string.IsNullOrWhiteSpace(LabelName))
                    return;

                if (!BookLables.Any(x => x == LabelName))
                {
                    BookLables.Add(LabelName);
                    BookLabelDetails.Clear();
                    SelectedLabel = LabelName;
                }

                var detail = Catalog.BookLabelDetails.Where(x => x.IsChecked).ToList();
                foreach (var item in detail)
                {
                    if (BookLabelDetails.Any(x => x.BookLabelId == item.BookLabelId))
                        continue;

                    BookLabelDetail temp = new BookLabelDetail();
                    temp.BookLabelId = item.BookLabelId;
                    temp.BoolLabelName = labelName;
                    temp.CatalogId = item.CatalogId;
                    temp.CreateTime = item.CreateTime;
                    temp.LabelPath = item.LabelPath;
                    temp.CreateTime = item.CreateTime;

                    BookLabelDetails.Add(temp);
                    DataCoach.GetInstance().InsertLable(temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加书签错误：{ex.Message}");
            }
        }
        public DelegateCommand ModifyBookLabelCommand { get; set; }
        private void ModifyBookLabel()
        {
            if (string.IsNullOrWhiteSpace(SelectedLabel))
                return;
            if (string.IsNullOrWhiteSpace(LabelName))
                return;

            SelectedLabel = LabelName;
            foreach (var item in BookLabelDetails)
            {
                DataCoach.GetInstance().UpdateLable(item);
            }
        }
        public ObservableCollection<string> BookLables { get; set; }

        private string selectedLabel;
        public string SelectedLabel 
        {
            get 
            {
                return selectedLabel;
            }
            set
            {
                if (selectedLabel == value)
                    return;
                selectedLabel = value;
                LabelName = value;
                BookLabelDetails.Clear();
                var items = DataCoach.GetInstance().GetBookLabelDetais(value);
                items.ForEach(x => BookLabelDetails.Add(x));
            }
        }
        public ObservableCollection<BookLabelDetail> BookLabelDetails { get; set; }

        public BookLabelDetail SelectedBookLabelDetail { get; set; }

        public DelegateCommand DeleteLabelCommand { get; set; }
        private void DeleteLabel()
        {
            try
            {
                if (SelectedLabel != null)
                {
                    var details = DataCoach.GetInstance().GetBookLabelDetais(SelectedLabel);
                    foreach (var item in details)
                    {
                        DataCoach.GetInstance().DeletLable(item);
                    }
                    BookLables.Remove(SelectedLabel);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public DelegateCommand DeleteLabelDetailCommand { get; set; }
        private void DeleteLabelDetail()
        {
            if (SelectedBookLabelDetail != null)
            {
                try
                {
                    DataCoach.GetInstance().DeletLable(SelectedBookLabelDetail);
                    BookLabelDetails.Remove(SelectedBookLabelDetail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

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
