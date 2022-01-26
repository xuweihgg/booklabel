using BLToolkit.Data.Linq;
using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Services
{
    public class CatalogLabelService
    {
        LabelDetailDataServices dataServices;
        public CatalogLabelService()
        {
            dataServices = new LabelDetailDataServices();
        }
            
        public List<CatalogLabel> GetCatalogLabels()
        {
            var items = new List<CatalogLabel>();
            using (GlobalInfo.SystemDB.GetConnection())
            {
                foreach (var item in CatalogLabelTable.Records)
                {
                    items.Add(new CatalogLabel(item.CatalogLabelId, item.CatalogLabelName, item.CatalogLabelType));
                }
            }
            return items;
        }

        public bool InsertCatalogLable(CatalogLabel clabel)
        {
            int res = 0;
            if (clabel == null)
                return false;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = new CatalogLabelTable()
                {
                    CatalogLabelId = clabel.CatalogLabelId,
                    CatalogLabelName=clabel.CatalogLabelName,
                    CatalogLabelType =clabel.CatalogLabelType,
                }.Insert();
            }
            return res > 0;
        }

        public bool Update(CatalogLabel detail)
        {
            int res = 0;
            if (detail == null)
                return false;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = CatalogLabelTable.Records.Update((x => x.CatalogLabelId == detail.CatalogLabelId), p => new CatalogLabelTable
                {
                    CatalogLabelName = detail.CatalogLabelName,
                    CatalogLabelType=detail.CatalogLabelType,
                });
            }
            return res >= 0;
        }

        public bool Delete(CatalogLabel detail)
        {
            int res = 0;
            if (detail == null)
                return false;
        

            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = BookLabelDetailTable.Records.Delete(x => x.BoolLabelName == detail.CatalogLabelName);
            }
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = CatalogLabelTable.Records.Delete(x => x.CatalogLabelId == detail.CatalogLabelId);
            }
            return res > 0;
        }
    }
}
