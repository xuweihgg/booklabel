﻿using BLToolkit.DataAccess;
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
            this.ChirdCatalogs = new ObservableCollection<CatalogConstruction>();
            this.BookLabelDetails = new ObservableCollection<BookLabelDetail>();
        }
        public string CatalogId { get; set; }

        public string CatalogName { get; set; }

        public string CatalogParentId { get; set; }

        public DateTime CatalogCreateDate { get; set; }

        public ObservableCollection<BookLabelDetail> BookLabelDetails { get; set; }

        private bool isInEditMode;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; OnPropertyChanged("isInEditMode"); }
        }

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
            AddUpdateAction(typeof(CatalogConstructionUpdater), @"CREATE TABLE BookLabelDetailTable(
BookLabelId VARCHAR2(100) NOT NULL,
LabelType VARCHAR2(100) NOT NULL,
LabelStatus VARCHAR2(100) NOT NULL,
BoolLabelName VARCHAR2(100) NOT NULL,
LabelPath VARCHAR2(100) NOT NULL,
CreateTime datetime,
PRIMARY KEY (BookLabelId))");

            AddUpdateAction(typeof(CatalogConstructionUpdater), @"CREATE TABLE CatalogLabelTable(
CatalogLabelId VARCHAR2(100) NOT NULL,
CatalogLabelName VARCHAR2(100) NOT NULL,
CatalogLabelType INT32 NOT NULL,
PRIMARY KEY (CatalogLabelId))");

        }

        public override bool NeedPreserveData(string tableName)
        {
            return false;
        }
    }
}
