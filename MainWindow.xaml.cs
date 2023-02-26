using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using Vives;

namespace circle_display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double angle1 = 0.1013416985;
        static int wheelCircle = 300;
        static int ledDistance = 30; //This is a the distance between the leds in a row.
        static int numberOfLeds = 16;
        static int circleCenter = (wheelCircle + (ledDistance * 2) * (numberOfLeds + 1)) / 2;
        public MainWindow()
        {
            InitializeComponent();
        }
        


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 17; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = ellipse.Height = wheelCircle + i * (ledDistance * 2);
                ellipse.StrokeThickness = 2;
                ellipse.Margin = new Thickness(ledDistance * (17 - i), ledDistance * (17 - i), 0, 0);
                ellipse.Stroke = new SolidColorBrush(Colors.Black);
                cnvsCircles.Children.Add(ellipse);
            }

            for (int j = 0; j < 33; j++)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.White);
                line.X1 = circleCenter;
                line.Y1 = circleCenter;
                line.X2 = circleCenter + Math.Cos(j * angle1) * (circleCenter - ledDistance);
                line.Y2 = circleCenter + Math.Sin(-j * angle1) * (circleCenter - ledDistance);
                cnvsCircles.Children.Add(line);
            }

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Height = circleCenter;
            rectangle.Width = circleCenter * 2;
            rectangle.Margin = new Thickness(0, circleCenter, 0, 0);
            rectangle.Fill = new SolidColorBrush(Colors.White);
            cnvsCircles.Children.Add(rectangle);

            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    System.Windows.Controls.CheckBox newChb = new CheckBox();

                    newChb.Tag = (15 - i).ToString() + "," + (j).ToString();
                    newChb.Height = newChb.Width = 30;
                    newChb.Checked += NewChb_Checked;
                    newChb.Unchecked += NewChb_Unchecked;
                    newChb.Margin = new Thickness(j * 30 + 50 , i * 30 + circleCenter + 100, 0, 0);
                    cnvsCircles.Children.Add(newChb);
                }

            }

            for (int k = 0; k < 16; k++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = k.ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.Margin = new Thickness( 20, (15 - k) * 30 + circleCenter + 95, 0, 0);
                cnvsCircles.Children.Add(newLbl);
            }

            for (int l = 0; l < 32; l++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = l.ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                newLbl.Margin = new Thickness(l * 30 + 43 , circleCenter + 90 + numberOfLeds * 30, 0, 0);
                cnvsCircles.Children.Add(newLbl);
            }

            for (int l = 0; l < 16; l++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = l.ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.Margin = new Thickness(l * ledDistance + 20 ,circleCenter + 10, 0, 0);
                cnvsCircles.Children.Add(newLbl);
            }

            for (int l = 0; l < 32; l++)
            {
                System.Windows.Controls.Label newLbl = new Label();

                newLbl.Content = (31 - l).ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.HorizontalAlignment = HorizontalAlignment.Left;
                newLbl.VerticalAlignment = VerticalAlignment.Top;

                PointF centerCirkel = new PointF(circleCenter, circleCenter);
                PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos(l * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-l * angle1)));

                PointF coord1;
                PointF coord2;

                FindLineCircleIntersections(circleCenter, circleCenter, circleCenter, centerCirkel, endCirkel, out coord1, out coord2);

                newLbl.Margin = new Thickness(coord1.X - 10, coord1.Y - 15, 0, 0);
                cnvsCircles.Children.Add(newLbl);
            }


            // Tekent de bollen die de leds representateren op de snijpunten van de ellipsen met de rechten.
            for (int l = 0; l < 32; l++)
            {
                for (int i = 0; i < 16; i++) 
                {
                    Ellipse cirkelLabel = new Ellipse();
                    cirkelLabel.Stroke = new SolidColorBrush(Colors.Black);
                    cirkelLabel.Fill = new SolidColorBrush(Colors.Black);
                    cirkelLabel.Width = cirkelLabel.Height = 20;
                    cirkelLabel.MouseDown += CirkelLabel_MouseDown;
                    cirkelLabel.Tag = (15 - i).ToString() + "," + (31 - l).ToString();

                    PointF centerCirkel = new PointF(circleCenter, circleCenter);
                    PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos(l * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-l * angle1)));

                    PointF coord1;
                    PointF coord2;

                    FindLineCircleIntersections(circleCenter, circleCenter, (circleCenter - ledDistance) - i * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);

                    cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
                    cnvsCircles.Children.Add(cirkelLabel);
                }
            }
        }

        private void NewChb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Ellipse cirkelLabel = new Ellipse();
            cirkelLabel.Stroke = new SolidColorBrush(Colors.Black);
            cirkelLabel.Fill = new SolidColorBrush(Colors.Black);
            cirkelLabel.Width = cirkelLabel.Height = 20;
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');

            PointF centerCirkel = new PointF(circleCenter, circleCenter);
            PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos((Convert.ToDouble(coords[1]) - 31) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[1])) * angle1)));

            PointF coord1;
            PointF coord2;

            FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[0])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
            cnvsCircles.Children.Add(cirkelLabel);
        }

        private void NewChb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Ellipse cirkelLabel = new Ellipse();
            cirkelLabel.Stroke = new SolidColorBrush(Colors.Black);
            cirkelLabel.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(sldRed.Value), Convert.ToByte(sldGreen.Value), Convert.ToByte(sldBlue.Value)));
            cirkelLabel.Width = cirkelLabel.Height = 20;
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');

            PointF centerCirkel = new PointF(circleCenter, circleCenter);
            PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos((Convert.ToDouble(coords[1]) - 31) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[1])) * angle1)));

            PointF coord1;
            PointF coord2;

            FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[0])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
            cnvsCircles.Children.Add(cirkelLabel);
        }

        private void CirkelLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse temp = sender as Ellipse;

            Debug.WriteLine(temp.Tag);

            string currentButton = "chb" + temp.Tag;            
        }

        private void cnvsCircles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }


        private double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        // Deze code berekend de snijpunten van de cirkel met de ellipsen.
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

        private void btnRedPreset_Click(object sender, RoutedEventArgs e)
        {
            sldRed.Value = 255;
            sldGreen.Value = 0;
            sldBlue.Value = 0;
        }

        private void btnWhitePreset_Click(object sender, RoutedEventArgs e)
        {
            sldRed.Value = 255;
            sldGreen.Value = 255;
            sldBlue.Value = 255;
        }
    }
}
