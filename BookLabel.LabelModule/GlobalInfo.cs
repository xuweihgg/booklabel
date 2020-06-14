using BookLabel.LabelModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLabel.LabelModule
{
    public class GlobalInfo
    {
        public static DBConnectionPool SystemDB { get; set; }
    }
}
