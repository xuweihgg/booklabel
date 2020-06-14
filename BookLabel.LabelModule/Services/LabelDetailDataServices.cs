using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLToolkit.Data.Linq;

namespace BookLabel.LabelModule.Services
{
    public class LabelDetailDataServices : ILabelDetailDataServices
    {
        public List<BookLabelDetail> GetLabelDetails()
        {
            var items = new List<BookLabelDetail>();
            using (GlobalInfo.SystemDB.GetConnection())
            {
                foreach(var item in BookLabelDetailTable.Records)
                {
                    items.Add(new BookLabelDetail()
                    {
                        BookLabelId = item.BookLabelId,
                        BoolLabelName = item.BoolLabelName,
                        CreateTime = item.CreateTime,
                        LabelPath = item.LabelPath,
                        CatalogId = item.CatalogId,
                    });
                }
            }

            return items;
        }

        public bool InsertLable(BookLabelDetail detail)
        {
            int res = 0;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = new BookLabelDetailTable()
                {
                    BookLabelId = detail.BookLabelId,
                    BoolLabelName = detail.BoolLabelName,
                    CreateTime = detail.CreateTime,
                    LabelPath = detail.LabelPath,
                    CatalogId = detail.CatalogId,
                }.Insert();
            }
            return res > 0;
        }

        public bool Update(BookLabelDetail detail)
        {
            int res = 0;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = BookLabelDetailTable.Records.Update((x => x.BookLabelId == detail.BookLabelId), p => new BookLabelDetailTable
                {
                    BoolLabelName = detail.BoolLabelName,
                    CreateTime = detail.CreateTime,
                    LabelPath = detail.LabelPath,
                    CatalogId = detail.CatalogId,
                });
            }
            return res >= 0;
        }

        public bool Delete(string bookLabelId)
        {
            int res = 0;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = BookLabelDetailTable.Records.Delete(x => x.BookLabelId == bookLabelId);
            }
            return res > 0;
        }
    }
}
