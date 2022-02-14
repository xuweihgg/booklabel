using BookLabel.LabelModule.Models;
using BookLabel.LabelModule.Services;
using BookLabel.LabelModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Unity;
using Prism.Ioc;
using BookLabel.LabelModule.ViewModels;

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

        public void InitializeDB()
        {
            string dbpath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Systemdb.db"));
            if (!System.IO.File.Exists(dbpath))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(dbpath);
            }
            GlobalInfo.SystemDB = DBHelper.CreateDBConnectionPool<CatalogConstructionUpdater>(dbpath, 1, null, 1);
        }

        public void OnInitialized(Prism.Ioc.IContainerProvider containerProvider)
        {
            InitializeDB();
            var regionManager = containerProvider.Resolve<IRegionManager>();
            IRegion region = regionManager.Regions[RegionNames.MainRegion];
          
            var tabA = containerProvider.Resolve<ParseLableView>();
            (tabA.DataContext as ParseLableViewModel).Title = "标签设置";
            region.Add(tabA);

            var tabB = containerProvider.Resolve<BookLabelView>();
            (tabB.DataContext as BookLabelViewModel).Title = "查看标签";
            region.Add(tabB);
        }

        public void RegisterTypes(Prism.Ioc.IContainerRegistry containerRegistry)
        {
            
        }
    }
}
