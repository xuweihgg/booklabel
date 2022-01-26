using BookLabel.LabelModule.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
            OpenCurrentFileCommand = new DelegateCommand(OpenCurrentFile);
            BookLables = new ObservableCollection<CatalogLabel>();
            BookLabelDetails = new ObservableCollection<BookLabelDetail>();
            SelectionChangedAction = new Action<object>(cc => SelectedChanage(cc));
            Initialize();
        }

        private void Initialize()
        {
            var lists = DataCoach.GetInstance().GetBookLabelDetais();
            BookLables = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().GetCatalogLabel());
            BookStatus = new ObservableCollection<string>();
            BookStatus.Add("全部");
            BookStatus.Add("完结");
            BookStatus.Add("连载");
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value;NotifyPropertyChanged("Title"); }
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
        public ObservableCollection<string> BookStatus { get; set; }

        private string selectedBookStatus;
        public string SelectedBookStatus { get { return selectedBookStatus; } set { selectedBookStatus = value; NotifyPropertyChanged("SelectedBookStatus"); } }

        private string selectedBookType;
        public string SelectedBookType { get { return selectedBookType; } set { selectedBookType = value; NotifyPropertyChanged("SelectedBookType"); } }

        private string labelName;
        public string LabelName { get { return labelName; } set { labelName = value;NotifyPropertyChanged("LabelName"); } }
        public DelegateCommand AddBookLabelCommand { get; set; }
        private void AddBookLabel()
        {
            try
            {
                if (Catalog == null)
                    return;
                if (string.IsNullOrWhiteSpace(SelectedBookType) && string.IsNullOrWhiteSpace(LabelName))
                    return;
                if (string.IsNullOrWhiteSpace(SelectedBookStatus))
                    return;

                if (!string.IsNullOrWhiteSpace(SelectedBookType))
                {
                    if (!DataCoach.GetInstance().CatalogLabels.Any(x => x.CatalogLabelType == (int)BookLabelType.AllType && x.CatalogLabelName == SelectedBookType))
                    {
                        DataCoach.GetInstance().InsertCatalogLabel(new CatalogLabel(Guid.NewGuid().ToString(), SelectedBookType, (int)BookLabelType.AllType));
                    }
                }

                if (!string.IsNullOrWhiteSpace(SelectedBookStatus))
                {
                    if (!DataCoach.GetInstance().CatalogLabels.Any(x => x.CatalogLabelType == (int)BookLabelType.AllStatus && x.CatalogLabelName == SelectedBookStatus))
                    {
                        DataCoach.GetInstance().InsertCatalogLabel(new CatalogLabel(Guid.NewGuid().ToString(), SelectedBookStatus, (int)BookLabelType.AllStatus));
                    }
                }

                if (!string.IsNullOrWhiteSpace(LabelName))
                {
                    if (!DataCoach.GetInstance().CatalogLabels.Any(x => x.CatalogLabelType == (int)BookLabelType.AllLabel && x.CatalogLabelName == LabelName))
                    {
                        DataCoach.GetInstance().InsertCatalogLabel(new CatalogLabel(Guid.NewGuid().ToString(), LabelName, (int)BookLabelType.AllLabel));
                    }
                }

                var detail = Catalog.BookLabelDetails.Where(x => x.IsChecked).ToList();
                foreach (var item in detail)
                {
                    if (BookLabelDetails.Any(x => x.LabelPath == item.LabelPath && x.LabelStatus == item.LabelStatus && x.LabelType == item.LabelType && x.BoolLabelName == item.BoolLabelName))
                        continue;

                    BookLabelDetail temp = new BookLabelDetail();
                    temp.BookLabelId = Guid.NewGuid().ToString();
                    temp.BoolLabelName = LabelName;
                    temp.LabelStatus = SelectedBookStatus;
                    temp.LabelType = SelectedBookType;
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
            if (SelectedLabel == null)
                return;
            if (string.IsNullOrWhiteSpace(LabelName))
                return;

            SelectedLabel.CatalogLabelName = LabelName;
            DataCoach.GetInstance().UpdateCatalogLabel(SelectedLabel);
            foreach (var item in BookLabelDetails)
            {
                DataCoach.GetInstance().UpdateLable(item);
            }
        }
        public ObservableCollection<CatalogLabel> BookLables { get; set; }

        private CatalogLabel selectedLabel;
        public CatalogLabel SelectedLabel 
        {
            get 
            {
                return selectedLabel;
            }
            set
            {
                if (selectedLabel == value)
                    return;
                if (value == null)
                    return;
                selectedLabel = value;
                LabelName = value.CatalogLabelName;
                BookLabelDetails.Clear();
                var items = DataCoach.GetInstance().GetBookLabelDetais(value.CatalogLabelName);
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
                    DataCoach.GetInstance().DeleteCatalogLabel(SelectedLabel);
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

        public DelegateCommand OpenCurrentFileCommand { get; set; }

        private void OpenCurrentFile()
        {
            if (SelectedBookLabelDetail != null)
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo(SelectedBookLabelDetail.LabelPath);
                process.StartInfo = processStartInfo;
                #region 下面这段被注释掉代码（可以用来全屏打开代码）
                ////建立新的系统进程    
                //System.Diagnostics.Process process = new System.Diagnostics.Process();
                ////设置文件名，此处为图片的真实路径+文件名（需要有后缀）    
                //process.StartInfo.FileName = NewFileName;
                ////此为关键部分。设置进程运行参数，此时为最大化窗口显示图片。    
                //process.StartInfo.Arguments = "rundll32.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen";
                //// 此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true    
                //process.StartInfo.UseShellExecute = true;
                #endregion
                try
                {
                    process.Start();
                    try
                    {
                        // process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    try
                    {
                        if (process != null)
                        {
                            process.Close();
                            process = null;
                        }
                    }
                    catch { }
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
