using BookLabel.LabelModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Services
{
    interface ILabelDetailDataServices
    {
        List<BookLabelDetail> GetLabelDetails();
        List<BookLabelDetail> GetLabelDetailsById(string catalogId);
        bool InsertLable(BookLabelDetail detail);


        bool Update(BookLabelDetail detail);

        bool Delete(string bookLabelId);
    }
}
