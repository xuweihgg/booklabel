using BLToolkit.Data.Linq;
using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Services
{
    public class CatalogDataService : ICatalogDataService
    {
        public List<CatalogConstruction> GetLabelDetails()
        {
            var items = new List<CatalogConstruction>();
            using (GlobalInfo.SystemDB.GetConnection())
            {
                foreach (var item in CatalogConstructionTable.Records)
                {
                    items.Add(new CatalogConstruction(item.CatalogId, item.CatalogName, item.CatalogParentId)
                    {
                        CatalogCreateDate = item.CatalogCreateDate,
                    });
                }
            }
            var catas = new List<CatalogConstruction>();
            foreach(var item in items.Where(x=>string.IsNullOrWhiteSpace(x.CatalogParentId)))
            {
                item.ChirdCatalogs = new System.Collections.ObjectModel.ObservableCollection<CatalogConstruction>(items.Where(x => x.CatalogParentId == item.CatalogId));
                catas.Add(item);
            }
            return catas;
        }

        public bool InsertLable(CatalogConstruction detail)
        {
            int res = 0;
            if (detail == null)
                return false;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = new CatalogConstructionTable()
                {
                    CatalogId = detail.CatalogId,
                    CatalogName = detail.CatalogName,
                    CatalogParentId = detail.CatalogParentId,
                    CatalogCreateDate = detail.CatalogCreateDate,
                }.Insert();
            }
            return res > 0;
        }

        public bool Update(CatalogConstruction detail)
        {
            int res = 0;
            if (detail == null)
                return false;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = CatalogConstructionTable.Records.Update((x => x.CatalogId == detail.CatalogId), p => new CatalogConstructionTable
                {
                    CatalogName = detail.CatalogName,
                    CatalogParentId = detail.CatalogParentId,
                    CatalogCreateDate = detail.CatalogCreateDate,
                });
            }
            return res >= 0;
        }

        public bool Delete(CatalogConstruction detail)
        {
            int res = 0;
            if (detail == null)
                return false;
            using (GlobalInfo.SystemDB.GetConnection())
            {
                res = CatalogConstructionTable.Records.Delete(x => x.CatalogId == detail.CatalogId);
            }
            return res > 0;
        }
    }
}
