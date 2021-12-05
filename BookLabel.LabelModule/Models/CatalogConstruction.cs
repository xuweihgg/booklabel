using BLToolkit.DataAccess;
using BLToolkit.Validation;
using BookLabel.LabelModule.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Models
{
    public class CatalogConstruction: INotifyPropertyChanged
    {
        public CatalogConstruction(string catalogid, string catalogname, string catalogParentid ="")
        {
            this.CatalogId = catalogid;
            this.CatalogName = catalogname;
            this.CatalogParentId = catalogParentid;
            this.BookLabelDetails = new ObservableCollection<BookLabelDetail>();
            this.ChirdCatalogs = new ObservableCollection<CatalogConstruction>();
        }
        public string CatalogId { get; set; }

        public string CatalogName { get; set; }

        public string CatalogParentId { get; set; }

        public DateTime CatalogCreateDate { get; set; }

        private bool isInEditMode;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; OnPropertyChanged("isInEditMode"); }
        }

        public ObservableCollection<BookLabelDetail> BookLabelDetails { get; set; }

        public ObservableCollection<CatalogConstruction> ChirdCatalogs { get; set; }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }


    [TableName(Name = "CatalogConstructionTable")]
    public class CatalogConstructionTable : ModelBase<CatalogConstructionTable>
    {


        static CatalogConstructionTable()
        {

        }

        internal static void Initialize(DBConnectionPool systemDB)
        {
            DBHelper.UpdateTableSchema<CatalogConstructionUpdater>(systemDB);
        }

        [PrimaryKey(0), Required]
        public string CatalogId { get; set; }

        public string CatalogName { get; set; }

        public string CatalogParentId { get; set; }

        public DateTime CatalogCreateDate { get; set; }
    }

    internal class CatalogConstructionUpdater : DBUpdater
    {
        static CatalogConstructionUpdater()
        {
            Actions[typeof(CatalogConstructionUpdater)] = new List<UpdateAction>();
            Versions[typeof(CatalogConstructionUpdater)] = 0;
            AddUpdateAction(typeof(CatalogConstructionUpdater), @"CREATE TABLE CatalogConstructionTable(
CatalogId VARCHAR2(100) NOT NULL,
CatalogName VARCHAR2(100) NOT NULL,
CatalogParentId VARCHAR2(100) NOT NULL,
CatalogCreateDate datetime,
PRIMARY KEY (CatalogId))");

            AddUpdateAction(typeof(CatalogConstructionUpdater), @"CREATE TABLE BookLabelDetailTable(
BookLabelId VARCHAR2(100) NOT NULL,
BoolLabelName VARCHAR2(100) NOT NULL,
LabelPath VARCHAR2(100) NOT NULL,
CatalogId VARCHAR2(100) NOT NULL,
CreateTime datetime,
PRIMARY KEY (BookLabelId))");
        }

        public override bool NeedPreserveData(string tableName)
        {
            return false;
        }
    }
}
