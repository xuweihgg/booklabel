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
    public class CatalogLabel: INotifyPropertyChanged
    {
        public CatalogLabel(string id, string name, int labelType)
        {
            this.CatalogLabelId = id;
            this.CatalogLabelName = name;
            this.CatalogLabelType = labelType;
        }
        public string CatalogLabelId { get; set; }
        public string CatalogLabelName { get; set; }
        public int CatalogLabelType { get; set; }

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

    [TableName(Name = "CatalogLabelTable")]
    public class CatalogLabelTable:ModelBase<CatalogLabelTable>
    {
        static CatalogLabelTable()
        {

        }

        internal static void Initialize(DBConnectionPool systemDB)
        {
            DBHelper.UpdateTableSchema<CatalogConstructionUpdater>(systemDB);
        }

        [PrimaryKey(0), Required]
        public string CatalogLabelId { get; set; }
        public string CatalogLabelName { get; set; }
        public int CatalogLabelType { get; set; }
    }
}


