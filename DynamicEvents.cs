using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

// In DynamicEvents worden de methoden geplaatst die aangemaakt worden en later zullen aangepast of vervangen worden.
// Dit zijn vooral de checkboxen, buttons en de cirkels die de leds voorstellen.

namespace circle_display
{
    internal class DynamicEvents : MainWindow
    {
        public static CheckBox CheckboxPrint(CheckBox _checkbox, int ledNumber, int rowNumber)
        {
            _checkbox.Tag = (rowNumber).ToString() + "," + (ledNumber).ToString();
            _checkbox.Margin = new Thickness(rowNumber * 20 + 30, ledNumber * 20, 0, 0);
            return _checkbox;
        }
        public static Ellipse LedPrint(Ellipse _ellipse, int ledNumbern, int rowNumber)
        {
            _ellipse.Fill = new SolidColorBrush(Colors.Black);
            _ellipse.Tag = rowNumber.ToString() + "," + (15 - ledNumbern).ToString();

            endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Sin(rowNumber * angle1)), (float)Convert.ToDecimal(circleCenter - Math.Cos(-rowNumber * angle1)));

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, (circleCenter - ledDistance) - ledNumbern * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);

            _ellipse.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
            return _ellipse;
        }
        public static void Export(APA102C[,] ExportData)
        {
            using StreamWriter writeData = new StreamWriter("SendData.txt");

            for (int i = 0; i < numberOfSegments; i++)
            {
                for (int j = 0; j < numberOfLeds; j++)
                {
                    writeData.Write($"{Convert.ToString(ExportData[i, j].brightness)},{Convert.ToString(ExportData[i, j].red)},{Convert.ToString(ExportData[i, j].green)},{Convert.ToString(ExportData[i, j].blue)},");
                }
            }
        }
        public static void Reset()
        {
            for (int i = 0; i < numberOfSegments; i++)
            {
                for (int j = 0; j < numberOfLeds; j++)
                {
                    CheckBox setCheck = checkList[16 * i + j];

                    setCheck.IsChecked = false;
                }
            }
        }
    }
}
