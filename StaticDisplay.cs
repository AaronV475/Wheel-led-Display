using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

// In de StaticDisplay class worden alle objecten geplaatst die aangemaakt worden en niet meer veranderen.
// Dit zijn vooral de labels en de visuele hulpen de gebruikt worden.

namespace circle_display
{
    internal class StaticDisplay: MainWindow
    {
        public static Ellipse PrintWheelCirkels(Ellipse _ellipse, int ledNumber)
        {
            _ellipse.Width = _ellipse.Height = wheelCircle + ledNumber * (ledDistance * 2) + ledDistance * 2;
            _ellipse.StrokeThickness = 1;
            _ellipse.Margin = new Thickness(ledDistance * (numberOfLeds - ledNumber), ledDistance * (numberOfLeds - ledNumber), 0, 0);
            _ellipse.Stroke = new SolidColorBrush(Colors.Gray);
            return _ellipse;
        }

        public static Label PrintCheckboxLabelY(Label _label, int ledNumber)
        {
            _label.Content = ledNumber.ToString();
            _label.Height = _label.Width = 30;
            _label.HorizontalContentAlignment = HorizontalAlignment.Center;
            _label.Margin = new Thickness(0, (ledNumber) * 20 - 5, 0, 0);
            return _label;
        }

        public static Line PrintWheelLine(Line _line, int rowNumber)
        {
            _line.Stroke = new SolidColorBrush(Colors.Gray);
            _line.X1 = _line.Y1 = circleCenter;
            _line.X2 = circleCenter + Math.Cos(rowNumber * angle + Math.PI / 2) * (circleCenter - ledDistance);
            _line.Y2 = circleCenter - Math.Sin(rowNumber * angle + Math.PI / 2) * (circleCenter - ledDistance);
            return _line;
        }

        public static Label PrintCheckboxLabelX(Label _label, int rowNumber)
        {
            _label.Content = rowNumber.ToString();
            _label.Height = _label.Width = 30;
            _label.HorizontalContentAlignment = HorizontalAlignment.Center;
            _label.Margin = new Thickness(rowNumber * 20 + 23, numberOfLeds * 20 - 10, 0, 0);
            return _label;
        }

        public static Label PrintWheelLabel(Label label, int rowNumber)
        {
            label.Content = (rowNumber).ToString();
            label.Height = label.Width = 30;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;

            endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Sin(rowNumber * angle)), (float)Convert.ToDecimal(circleCenter - Math.Cos(-rowNumber * angle)));

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter, centerCirkel, endCirkel, out coord1, out coord2);

            label.Margin = new Thickness(coord1.X - 10, coord1.Y - 15, 0, 0);
            return label;
        }
    }
}
