using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLToolkit.Data.Linq;

namespace BookLabel.LabelModule.Services
{
    public class LabelDetailDataServices 
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
                        LabelStatus=item.LabelStatus,
                        LabelType=item.LabelType,
                        CreateTime = item.CreateTime,
                        LabelPath = item.LabelPath,
                    });
                }
            }

            return items;
        }

        public List<BookLabelDetail> GetLabelDetailsById(string bookLabelName)
        {
            var items = new List<BookLabelDetail>();

            using (GlobalInfo.SystemDB.GetConnection())
            {
                foreach (var item in BookLabelDetailTable.Records.Where(x=>x.BoolLabelName== bookLabelName).ToList())
                {
                    items.Add(new BookLabelDetail()
                    {
                        BookLabelId = item.BookLabelId,
                        LabelStatus = item.LabelStatus,
                        LabelType = item.LabelType,
                        BoolLabelName = item.BoolLabelName,
                        CreateTime = item.CreateTime,
                        LabelPath = item.LabelPath,
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
                    LabelType=detail.LabelType,
                    LabelStatus=detail.LabelStatus,
                    BoolLabelName = detail.BoolLabelName,
                    CreateTime = detail.CreateTime,
                    LabelPath = detail.LabelPath,
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
                    LabelType = detail.LabelType,
                    LabelStatus = detail.LabelStatus,
                    BoolLabelName = detail.BoolLabelName,
                    CreateTime = detail.CreateTime,
                    LabelPath = detail.LabelPath,
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
