using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule
{
    public static class UtityHelper
    {
        public static ObservableCollection<BookLabelDetail> getFileName(ObservableCollection<BookLabelDetail> list, string filepath, string catalog)
        {
            DirectoryInfo root = new DirectoryInfo(filepath);
            foreach (FileInfo f in root.GetFiles())
            {
                list.Add(new BookLabelDetail
                {
                    BookLabelId = Guid.NewGuid().ToString(),
                    BoolLabelName = f.Name,
                    LabelPath = f.FullName,
                    CatalogId = catalog,
                }) ;
            }
            return list;
        }
        public static ObservableCollection<CatalogConstruction> GetRootDirectory(string path)
        {
            ObservableCollection<CatalogConstruction> catalogs = new ObservableCollection<CatalogConstruction>();
            var guidId = Guid.NewGuid().ToString();
            catalogs.Add(new CatalogConstruction(guidId, path, "")
            {
                ChirdCatalogs = GetallDirectory(new ObservableCollection<CatalogConstruction>(), path, guidId),
                BookLabelDetails = getFileName(new ObservableCollection<BookLabelDetail>(), path, guidId),
            });
            return catalogs;
        }
        //获得指定路径下的所有子目录名
        // <param name="list">文件列表</param>
        // <param name="path">文件夹路径</param>
        public static ObservableCollection<CatalogConstruction> GetallDirectory(ObservableCollection<CatalogConstruction> list, string path, string parentId="")
        {
            DirectoryInfo root = new DirectoryInfo(path);
            var dirs = root.GetDirectories();
            if (dirs.Count() != 0)
            {
                foreach (DirectoryInfo d in dirs)
                {
                    var guidId = Guid.NewGuid().ToString();
                    list.Add(new CatalogConstruction(guidId, d.Name, parentId)
                    {
                        ChirdCatalogs = GetallDirectory(new ObservableCollection<CatalogConstruction>(), d.FullName, guidId),
                        BookLabelDetails = getFileName(new ObservableCollection<BookLabelDetail>(), d.FullName, guidId),
                    });
                }
            }
            return list;
            //ObservableCollection<CatalogConstruction> results = new ObservableCollection<CatalogConstruction>();
            //results.Add(new CatalogConstruction(parentId, path, "")
            //{
            //    ChirdCatalogs = list,
            //    BookLabelDetails = getFileName(new ObservableCollection<BookLabelDetail>(), path),
            //}); 
            //return results;
        }
    }
}
