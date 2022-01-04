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

        ILabelDetailDataServices dataServices;
        ICatalogDataService catalogDataService;
        private DataCoach()
        {
            dataServices = new LabelDetailDataServices();
            catalogDataService = new CatalogDataService();
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
                if (bookLabelDetails.Any(x => x.CatalogId == bookLabel.CatalogId))
                {
                    var item = bookLabelDetails.FirstOrDefault(x => x.BookLabelId == bookLabel.BookLabelId);
                    item.BoolLabelName = bookLabel.BoolLabelName;
                    item.CreateTime = bookLabel.CreateTime;
                    item.LabelPath = bookLabel.LabelPath;
                    item.CatalogId = bookLabel.CatalogId;
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
    }
}
