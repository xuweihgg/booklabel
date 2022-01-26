using BookLabel.LabelModule.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public BookLabelViewModel()
        {
            FilterTypes = new ObservableCollection<CatalogLabel>();
            BookTypes = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().CatalogLabels.Where(x => x.CatalogLabelType == (int)BookLabelType.AllType));
            StatusTypes = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().CatalogLabels.Where(x => x.CatalogLabelType == (int)BookLabelType.AllStatus));
            LabelTypes = new ObservableCollection<CatalogLabel>(DataCoach.GetInstance().CatalogLabels.Where(x => x.CatalogLabelType == (int)BookLabelType.AllLabel));
            BookLabelDetails = new ObservableCollection<BookLabelDetail>(DataCoach.GetInstance().GetBookLabelDetais());
            SelectBookTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectBookType);
            SelectFilterTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectFilterType);
            SelectStatusTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectStatusType);
            SelectLabelTypeCommand = new DelegateCommand<CatalogLabel>(OnSelectLabelType);
        }

        public ObservableCollection<BookLabelDetail> BookLabelDetails { get; set; }
        public ObservableCollection<CatalogLabel> FilterTypes { get; set; }
        public ObservableCollection<CatalogLabel> BookTypes { get; set; }

        public ObservableCollection<CatalogLabel> StatusTypes { get; set; }

        public ObservableCollection<CatalogLabel> LabelTypes { get; set; }

        public DelegateCommand<CatalogLabel> SelectFilterTypeCommand { get; set; }
        private void OnSelectFilterType(CatalogLabel arg)
        {
            FilterTypes.Remove(arg);
        }

        public DelegateCommand<CatalogLabel> SelectBookTypeCommand { get; set; }
        private void OnSelectBookType(CatalogLabel arg)
        {
            FilterTypes.Add(arg);
        }

        public DelegateCommand<CatalogLabel> SelectStatusTypeCommand { get; set; }
        private void OnSelectStatusType(CatalogLabel arg)
        {
            FilterTypes.Add(arg);
        }

        public DelegateCommand<CatalogLabel> SelectLabelTypeCommand { get; set; }
        private void OnSelectLabelType(CatalogLabel arg)
        {
            FilterTypes.Add(arg);
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
