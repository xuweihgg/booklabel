using BookLabel.LabelModule.Services;
using BookLabel.LabelModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace BookLabel.LabelModule.Views
{
    /// <summary>
    /// MuenView.xaml 的交互逻辑
    /// </summary>
    public partial class MuenView : UserControl
    {
        public MuenView(MenuViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in ((System.Array)e.Data.GetData(DataFormats.FileDrop)))
            {
                builder.AppendLine(item.ToString());
            }
            ((TextBox)sender).Text = builder.ToString();
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
            }
            else
                e.Effects = DragDropEffects.None;
        }
    }
}
