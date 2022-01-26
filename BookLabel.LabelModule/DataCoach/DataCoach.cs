using BookLabel.LabelModule.Models;
using BookLabel.LabelModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule
{
    public class DataCoach
    {
        private static  DataCoach dataCoach;

        LabelDetailDataServices dataServices;
        CatalogDataService catalogDataService;
        CatalogLabelService catalogLabelService;
        private DataCoach()
        {
            dataServices = new LabelDetailDataServices();
            catalogDataService = new CatalogDataService();
            catalogLabelService = new CatalogLabelService();
        }

        private static object dataCoach_lock = new object();
        public static DataCoach GetInstance()
        {
            lock (dataCoach_lock)
            {
                if (dataCoach == null)
                {
                    dataCoach = new DataCoach();
                }
            }
            return dataCoach;
        }

        private List<CatalogConstruction> catalogConstructions = null;
        public List<CatalogConstruction> GetCatalogs()
        {
            if (catalogConstructions == null)
            {
                catalogConstructions = new List<CatalogConstruction>();
               
                catalogConstructions = catalogDataService.GetLabelDetails();
            }
            return catalogConstructions;
        }

        private  List<BookLabelDetail> bookLabelDetails = null;
        public  List<BookLabelDetail> GetBookLabelDetais()
        {
            if (bookLabelDetails == null)
            {
                bookLabelDetails = new List<BookLabelDetail>();
                
                bookLabelDetails = dataServices.GetLabelDetails();
            }
            return bookLabelDetails;
        }

        public List<BookLabelDetail> GetBookLabelDetais(string bookLabelName)
        {
            if (bookLabelDetails != null)
            {
                return bookLabelDetails.Where(x => x.BoolLabelName == bookLabelName).ToList();
            }
            return null;
        }

        private List<CatalogLabel> catalogLabels = null;
        public List<CatalogLabel> GetCatalogLabels()
        {
            if (catalogLabels == null)
            {
                catalogLabels = new List<CatalogLabel>();
                catalogLabels = catalogLabelService.GetCatalogLabels();
            }
            return catalogLabels;
        }
        public List<CatalogLabel> GetCatalogLabelsByType(int lableType)
        {
            if (catalogLabels != null)
            {
                return catalogLabels.Where(x => x.CatalogLabelType == lableType).ToList();
            }
            return null;
        }

        public bool AddCatalog(CatalogConstruction catalog)
        {
            if (catalog == null)
                return false;
            if (catalogConstructions != null)
            {
                catalogConstructions.Add(catalog);
                catalogDataService.InsertLable(catalog);
            }
            return true;
        }

        public bool UpdateCatalog(CatalogConstruction catalog)
        {
            if (catalog == null)
                return false;
            if (catalogConstructions != null)
            {
                if (catalogConstructions.Any(x => x.CatalogId == catalog.CatalogId))
                {
                    var item = catalogConstructions.FirstOrDefault(x => x.CatalogId == catalog.CatalogId);
                    item.CatalogName = catalog.CatalogName;
                    item.CatalogParentId = catalog.CatalogParentId;
                    item.CatalogCreateDate = catalog.CatalogCreateDate;
                }
                catalogDataService.Update(catalog);
            }
            return true;
        }

        public bool DeletCatalog(CatalogConstruction catalog)
        {
            if (catalog == null)
                return false;
            if (catalogConstructions != null)
            {
                catalogConstructions.Remove(catalog);
                catalogDataService.Delete(catalog);
            }
            return true;
        }

        public bool InsertLable(BookLabelDetail bookLabel)
        {
            if (bookLabel == null)
                return false;
            if (bookLabelDetails != null)
            {
                bookLabelDetails.Add(bookLabel);
                dataServices.InsertLable(bookLabel);
            }
            return true;
        }

        public bool UpdateLable(BookLabelDetail bookLabel)
        {
            if (bookLabel == null)
                return false;
            if (bookLabelDetails != null)
            {
                if (bookLabelDetails.Any(x => x.BookLabelId == bookLabel.BookLabelId))
                {
                    var item = bookLabelDetails.FirstOrDefault(x => x.BookLabelId == bookLabel.BookLabelId);
                    item.LabelStatus = bookLabel.LabelStatus;
                    item.LabelType = bookLabel.LabelType;
                    item.BoolLabelName = bookLabel.BoolLabelName;
                    item.CreateTime = bookLabel.CreateTime;
                    item.LabelPath = bookLabel.LabelPath;
                }
                dataServices.Update(bookLabel);
            }
            return true;
        }

        public bool DeletLable(BookLabelDetail bookLabel)
        {
            if (bookLabel == null)
                return false;
            if (bookLabelDetails != null)
            {
                bookLabelDetails.Remove(bookLabel);
                dataServices.Delete(bookLabel.BookLabelId);
            }
            return true;
        }

        public List<CatalogLabel> CatalogLabels;
        public List<CatalogLabel> GetCatalogLabel()
        {
            if (CatalogLabels == null)
            {
                CatalogLabels = new List<CatalogLabel>();
                CatalogLabels = catalogLabelService.GetCatalogLabels();
            }
            return CatalogLabels;
        }
        public bool InsertCatalogLabel(CatalogLabel label)
        {
            if(label == null)
            {
                return false;
            }
            return catalogLabelService.InsertCatalogLable(label);
        }

        public bool UpdateCatalogLabel(CatalogLabel label) 
        {
            if (label == null)
                return false;
            return catalogLabelService.Update(label);
        }

        public bool DeleteCatalogLabel(CatalogLabel label)
        {
            if (label == null)
                return false;

            if (bookLabelDetails != null)
            {
                var items = bookLabelDetails.Where(x => x.BoolLabelName == label.CatalogLabelName).ToList();
                foreach (var item in items)
                {
                    bookLabelDetails.Remove(item);
                }
            }
            catalogLabelService.Delete(label);

            return true;
        }


    }
}
