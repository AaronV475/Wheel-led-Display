using circle_display;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

namespace Vives
{
    public class Framework : MainWindow
    {
        public static void LedLayout(Ellipse ledLayout, SolidColorBrush color, CheckBox cb)
        {
            ledLayout.StrokeThickness = 1;
            ledLayout.Stroke = new SolidColorBrush(Colors.Black);
            ledLayout.Fill = color;
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');
            ledLayout.Tag = cb.Tag;

            PointF centerCirkel = new PointF(circleCenter, circleCenter);
            PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos((Convert.ToDouble(coords[1]) - 31) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[1])) * angle1)));

            PointF coord1;
            PointF coord2;

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[0])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            ledLayout.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
        }
    }
}
