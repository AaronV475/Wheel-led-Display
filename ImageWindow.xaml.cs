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
using System.Windows.Shapes;

namespace circle_display
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {

        public ImageWindow(MainWindow mainWindow)
        {
            InitializeComponent();;
        }

        public void EditImage(double width, double height, BitmapImage source)
        {
            imgImport.Width = width;
            imgImport.Height = height;
            imgImport.Source = source;
        }

        public void EditBitmap(double width, double height, BitmapImage source)
        {
            imgExport.Width = width;
            imgExport.Height = height;
            imgExport.Source = source;
        }
    }
}
