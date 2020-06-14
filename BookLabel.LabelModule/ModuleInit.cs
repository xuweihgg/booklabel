using BookLabel.LabelModule.Models;
using BookLabel.LabelModule.Services;
using BookLabel.LabelModule.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookLabel.LabelModule
{
    public class ModuleInit:IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public ModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }
        public void Initialize()
        {
            InitializeDB();
            this.container.RegisterType<ICatalogDataService, CatalogDataService>();
            this.regionManager.RegisterViewWithRegion(RegionNames.LeftRegion, () => this.container.Resolve<MuenView>());
        }

        public void InitializeDB()
        {
            string dbpath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Systemdb.db"));
            if (!System.IO.File.Exists(dbpath))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(dbpath);
            }
            GlobalInfo.SystemDB = DBHelper.CreateDBConnectionPool<CatalogConstructionUpdater>(dbpath, 1, null, 1);
        }
    }
}
