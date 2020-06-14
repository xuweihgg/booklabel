using BLToolkit.DataAccess;
using BLToolkit.Validation;
using BookLabel.LabelModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Models
{
    public class BookLabelDetail
    {
        public string BookLabelId { get; set; }
        public string BoolLabelName { get; set; }
        public DateTime CreateTime { get; set; }
        public string LabelPath { get; set; }
        public string CatalogId { get; set; }

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
