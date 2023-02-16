using System;
using System.Collections.Generic;
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

namespace circle_display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 17; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = ellipse.Height = 300 + i * 40;
                ellipse.StrokeThickness = 2;
                ellipse.Margin = new Thickness(20 * (17 - i), 20 * (17 - i), 0, 0);
                ellipse.Stroke = new SolidColorBrush(Colors.Black);
                cnvsCircles.Children.Add(ellipse);
            }

            for (int j = 0; j < 33; j++)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.X1 = 490;
                line.Y1 = 490;
                line.X2 = 490 + Math.Cos(j * 0.098174770424681) * 470;
                line.Y2 = 490 + Math.Sin(-j * 0.098174770424681) * 470;
                cnvsCircles.Children.Add(line);
            }

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Height = 500;
            rectangle.Width = 1000;
            rectangle.Margin = new Thickness(0, 490, 0, 0);
            rectangle.Fill = new SolidColorBrush(Colors.White);
            cnvsCircles.Children.Add(rectangle);

            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    System.Windows.Controls.CheckBox newChb = new CheckBox();

                    newChb.Name = "chb" + i.ToString() + "_" + j.ToString();
                    newChb.Height = newChb.Width = 30;
                    newChb.Margin = new Thickness(j * 60 - 940, i * 60 - 600, 0, 0);
                    grdButtons.Children.Add(newChb);
                }

            }

            for (int k = 0; k < 16; k++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = k.ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.Margin = new Thickness(-1000, (15 - k) * 60 - 610, 0, 0);
                grdButtons.Children.Add(newLbl);
            }

            for (int l = 0; l < 32; l++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = l.ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.Margin = new Thickness(l * 60 - 940, 350, 0, 0);
                grdButtons.Children.Add(newLbl);
            }

            for (int l = 0; l < 32; l++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = (31 - l).ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                newLbl.VerticalContentAlignment = VerticalAlignment.Center;
                newLbl.HorizontalAlignment = HorizontalAlignment.Center;
                newLbl.VerticalAlignment = VerticalAlignment.Center;

                PointF centerCirkel = new PointF(490, 490);
                PointF endCirkel = new PointF((float)Convert.ToDecimal(490 + Math.Cos(l * 0.098174770424681 - 0.098174770424681 / 2)), (float)Convert.ToDecimal(490 + Math.Sin(-l * 0.098174770424681 - 0.098174770424681 / 2)));

                PointF coord1;
                PointF coord2;

                FindLineCircleIntersections(490,490,980, centerCirkel, endCirkel, out coord1, out coord2);

                newLbl.Margin = new Thickness(coord1.X - 500, coord1.Y - 1200, 0, 0);
                grdButtons.Children.Add(newLbl);
            }

            for (int l = 0; l < 32; l++)
            {
                Ellipse cirkelLabel = new Ellipse();
                cirkelLabel.Stroke = new SolidColorBrush(Colors.Black);
                cirkelLabel.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(sldRed.Value), Convert.ToByte(sldGreen.Value), Convert.ToByte(sldBlue.Value)));
                cirkelLabel.Width = cirkelLabel.Height = 20;

                PointF centerCirkel = new PointF(490, 490);
                PointF endCirkel = new PointF((float)Convert.ToDecimal(490 + Math.Cos(l * 0.098174770424681 - 0.098174770424681 / 2)), (float)Convert.ToDecimal(490 + Math.Sin(-l * 0.098174770424681 - 0.098174770424681 / 2)));

                PointF coord1;
                PointF coord2;

                FindLineCircleIntersections(490, 490, 470, centerCirkel, endCirkel, out coord1, out coord2);

                cirkelLabel.Margin = new Thickness(coord1.X, coord1.Y, 0, 0);
                cnvsCircles.Children.Add(cirkelLabel);
            }
        }

        private void cnvsCircles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse leds = new Ellipse();
            leds.Stroke = new SolidColorBrush(Colors.Black);
            leds.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(sldRed.Value), Convert.ToByte(sldGreen.Value), Convert.ToByte(sldBlue.Value)));
            leds.Width = leds.Height = 20;
            leds.Margin = new Thickness(Mouse.GetPosition(cnvsCircles).X - 10, Mouse.GetPosition(cnvsCircles).Y - 10, 0, 0);
            cnvsCircles.Children.Add(leds);
        }






        public PointF ClosestIntersection(float cx, float cy, float radius,
                                  PointF lineStart, PointF lineEnd)
        {
            PointF intersection1;
            PointF intersection2;
            int intersections = FindLineCircleIntersections(cx, cy, radius, lineStart, lineEnd, out intersection1, out intersection2);

            if (intersections == 1)
                return intersection1; // one intersection

            if (intersections == 2)
            {
                double dist1 = Distance(intersection1, lineStart);
                double dist2 = Distance(intersection2, lineStart);

                if (dist1 < dist2)
                    return intersection1;
                else
                    return intersection2;
            }

            return PointF.Empty; // no intersections at all
        }

        private double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        // Find the points of intersection.
        private int FindLineCircleIntersections(float cx, float cy, float radius,
                                                PointF point1, PointF point2, 
                                                out PointF intersection1, out PointF intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new PointF(float.NaN, float.NaN);
                intersection2 = new PointF(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new PointF(float.NaN, float.NaN);
                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                return 2;
            }
        }
    }
}
