using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KanbanSystemShow
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : UserControl
    {
        public test(int j)
        {
            InitializeComponent();
            string path = Directory.GetCurrentDirectory();
            imgText.Source = new BitmapImage(new Uri($"{path}/ImagesSave/5VJ21A09005/{MainWindow.dtM.Rows[j][2]}.jpg", UriKind.RelativeOrAbsolute));
        }
    }
}
