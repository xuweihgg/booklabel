using BookLabel.Shell.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookLabel.Shell
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(BookLabel.LabelModule.ModuleInit));
        }

        //protected override Window CreateShell()
        //{
        //    // Use the container to create an instance of the shell.
        //    ShellView view = this.Container.TryResolve<ShellView>();
        //    return view;
        //}

        protected override void InitializeShell(Window shell)
        {
            App.Current.MainWindow = shell;
            App.Current.MainWindow.Show();
        }
    }
}
