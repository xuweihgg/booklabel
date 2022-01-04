using BLToolkit.DataAccess;
using BLToolkit.Validation;
using BookLabel.LabelModule.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Models
{
    public class BookLabelDetail : INotifyPropertyChanged
    {
        private bool isChecked;
        public bool IsChecked { get { return isChecked; } set { isChecked = value;NotifyPropertyChanged("IsChecked"); } }
        public string BookLabelId { get; set; }
        public string BoolLabelName { get; set; }
        public DateTime CreateTime { get; set; }
        public string LabelPath { get; set; }
        public string CatalogId { get; set; }

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

    [TableName(Name = "BookLabelDetailTable")]
    public class BookLabelDetailTable : ModelBase<BookLabelDetailTable>
    {


        static BookLabelDetailTable()
        {

        }

        internal static void Initialize(DBConnectionPool systemDB)
        {
            DBHelper.UpdateTableSchema<CatalogConstructionUpdater>(systemDB);
        }

        [PrimaryKey(0), Required]
        public string BookLabelId { get; set; }

        public string BoolLabelName { get; set; }

        public string LabelPath { get; set; }

        public string CatalogId { get; set; }

        public DateTime CreateTime { get; set; }
    }

   
}
