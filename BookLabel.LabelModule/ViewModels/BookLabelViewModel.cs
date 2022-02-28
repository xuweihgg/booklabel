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

namespace BookLabel.LabelModule.ViewModels
{
    public class BookLabelViewModel : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("Title"); }
        }

        private List<BookLabelDetail> AllCatalabels;
        public BookLabelViewModel()
        {
            FilterTypes = new ObservableCollection<CatalogLabel>();
            BookTypes = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().CatalogLabels.Where(x => x.CatalogLabelType == (int)BookLabelType.AllType));
            StatusTypes = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().CatalogLabels.Where(x => x.CatalogLabelType == (int)BookLabelType.AllStatus));
            LabelTypes = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().CatalogLabels.Where(x => x.CatalogLabelType == (int)BookLabelType.AllLabel));
            SelectBookTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectBookType);
            SelectFilterTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectFilterType);
            SelectStatusTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectStatusType);
            SelectLabelTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectLabelType);
            AllCatalabels = DataCoach.GetInstance().GetBookLabelDetais();
            BookLabelDetails = new ObservableCollection<BookLabelDetail>(AllCatalabels);
            OpenCurrentFileCommand = new DelegateCommand(OpenCurrentFile);
            DeleteLabelDetailCommand = new DelegateCommand(DeleteCurrentDetail);
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
                        
                    }
                }
                catch (Exception ex)
                {
                    
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

        public DelegateCommand DeleteLabelDetailCommand { get; set; }
        private void DeleteCurrentDetail()
        {
            if (SelectedBookLabelDetail != null)
            {
                BookLabelDetails.Remove(SelectedBookLabelDetail);
                AllCatalabels.Remove(SelectedBookLabelDetail);
                DataCoach.GetInstance().DeletLable(SelectedBookLabelDetail);
            }
        }
        public ObservableCollection<BookLabelDetail> BookLabelDetails { get; set; }
        public BookLabelDetail SelectedBookLabelDetail { get; set; }
        public ObservableCollection<CatalogLabel> FilterTypes { get; set; }
        public ObservableCollection<CatalogLabel> BookTypes { get; set; }

        public ObservableCollection<CatalogLabel> StatusTypes { get; set; }

        public ObservableCollection<CatalogLabel> LabelTypes { get; set; }

        public DelegateCommand<CatalogLabel> SelectFilterTypeCommand { get; set; }
        private void OnSelectFilterType(CatalogLabel arg)
        {
            FilterTypes.Remove(arg);
            FilterLabel();
        }

        public DelegateCommand<CatalogLabel> SelectBookTypeCommand { get; set; }
        private void OnSelectBookType(CatalogLabel arg)
        {
            FilterTypes.Add(arg);
            FilterLabel();
        }

        public DelegateCommand<CatalogLabel> SelectStatusTypeCommand { get; set; }
        private void OnSelectStatusType(CatalogLabel arg)
        {
            FilterTypes.Add(arg);
            FilterLabel();
        }

        public DelegateCommand<CatalogLabel> SelectLabelTypeCommand { get; set; }
        private void OnSelectLabelType(CatalogLabel arg)
        {
            FilterTypes.Add(arg);
            FilterLabel();
        }

        private void FilterLabel()
        {
            BookLabelDetails.Clear();
            if (!FilterTypes.Any())
            {
                AllCatalabels.ForEach(x => BookLabelDetails.Add(x));
            }
            else
            {
                foreach (var item in AllCatalabels)
                {
                    if (FilterTypes.Any(x => x.CatalogLabelType == (int)BookLabelType.AllType))
                    {
                        if (FilterTypes.Where(x => x.CatalogLabelType == (int)BookLabelType.AllType).ToList().Any(z => z.CatalogLabelName == item.LabelType))
                        { }
                        else
                            continue;
                    }
                    if (FilterTypes.Any(x => x.CatalogLabelType == (int)BookLabelType.AllStatus))
                    {
                        if (FilterTypes.Where(x => x.CatalogLabelType == (int)BookLabelType.AllStatus).ToList().Any(z => z.CatalogLabelName == item.LabelStatus))
                        { }
                        else
                            continue;
                    }
                    if (FilterTypes.Any(x => x.CatalogLabelType == (int)BookLabelType.AllLabel))
                    {
                        if (FilterTypes.Where(x => x.CatalogLabelType == (int)BookLabelType.AllLabel).ToList().Any(z => z.CatalogLabelName == item.BoolLabelName))
                        { }
                        else
                            continue;
                    }
                    BookLabelDetails.Add(item);
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
