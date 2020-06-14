using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Services
{
    public interface ICatalogDataService
    {
        List<CatalogConstruction> GetLabelDetails();

        bool InsertLable(CatalogConstruction detail);


        bool Update(CatalogConstruction detail);

        bool Delete(CatalogConstruction detail);
    }
}
