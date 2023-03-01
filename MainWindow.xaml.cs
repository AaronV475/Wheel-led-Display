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
        public static double angle1 = 0.1013416985; // Hoek tussen de segmenten in radialen. (Moet nog in formule verwerkt worden.)

        static int wheelCircle = 350; // Diameter van de kleinste cirkel van het wiel.
        public static int ledDistance = 25; // This is a the distance between the leds in a row.

        static int numberOfLeds = 16; // Aantal leds per rij / lengte van de ledstrip.
        static int numberOfSegments = 32; // Aantal segmenten waarin het halve of hele wiel is ingedeeld.

        public static int circleCenter = (wheelCircle + (ledDistance * 2) * (numberOfLeds + 1)) / 2; // Y-locatie van het centrum van het wiel.
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                // Tekent de cirkels van het wiel
                Ellipse ellipse = new Ellipse();
                ellipse.Width = ellipse.Height = wheelCircle + i * (ledDistance * 2) + ledDistance * 2;
                ellipse.StrokeThickness = 1;
                ellipse.Margin = new Thickness(ledDistance * (16 - i), ledDistance * (16 - i), 0, 0);
                ellipse.Stroke = new SolidColorBrush(Colors.Gray);
                cnvsCircles.Children.Add(ellipse);
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

            for (int i = 0; i < 16; i++)
            {
                // Zet de labels bij de checkboxen met het aantal rijen aangeduid
                Label labelChbRow = new Label();
                labelChbRow.Content = i.ToString();
                labelChbRow.Height = labelChbRow.Width = 30;
                labelChbRow.Margin = new Thickness(20, (15 - i) * 30 + circleCenter + 95, 0, 0);
                cnvsCircles.Children.Add(labelChbRow);

                // Zet de labels bij de cirkels met het aantal rijen aangeduid
                Label labelCrkRow = new Label();
                labelCrkRow.Content = i.ToString();
                labelCrkRow.Height = labelCrkRow.Width = 30;
                labelCrkRow.Margin = new Thickness(i * ledDistance + 20, circleCenter + 10, 0, 0);
                cnvsCircles.Children.Add(labelCrkRow);
            }

            for (int j = 0; j < 32; j++)
            {
                // Tekent de lijnen die de segmenten verdelen
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.X1 = circleCenter;
                line.Y1 = circleCenter;
                line.X2 = circleCenter + Math.Cos(j * angle1) * (circleCenter - ledDistance);
                line.Y2 = circleCenter + Math.Sin(-j * angle1) * (circleCenter - ledDistance);
                cnvsCircles.Children.Add(line);

                // Tekent de labels bij de checkboxen met het aantal kolommen aangeduid
                Label newLbl = new Label();
                newLbl.Content = j.ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                newLbl.Margin = new Thickness(j * 30 + 43, circleCenter + 90 + numberOfLeds * 30, 0, 0);
                cnvsCircles.Children.Add(newLbl);
            }

            for (int l = 0; l < 32; l++)
            {
                Label newLbl = new Label();

                newLbl.Content = (31 - l).ToString();
                newLbl.Height = newLbl.Width = 30;
                newLbl.HorizontalAlignment = HorizontalAlignment.Left;
                newLbl.VerticalAlignment = VerticalAlignment.Top;

                PointF centerCirkel = new PointF(circleCenter, circleCenter);
                PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos(l * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-l * angle1)));

                PointF coord1;
                PointF coord2;

                Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter, centerCirkel, endCirkel, out coord1, out coord2);

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

                    Calculations.FindLineCircleIntersections(circleCenter, circleCenter, (circleCenter - ledDistance) - i * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);

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
            cirkelLabel.StrokeThickness = 1;
            cirkelLabel.Fill = new SolidColorBrush(Colors.Black);
            cirkelLabel.Width = cirkelLabel.Height = 20;
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');
            cirkelLabel.Tag = cb.Tag;
            cirkelLabel.MouseDown += CirkelLabel_MouseDown;

            PointF centerCirkel = new PointF(circleCenter, circleCenter);
            PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos((Convert.ToDouble(coords[1]) - 31) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[1])) * angle1)));

            PointF coord1;
            PointF coord2;

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[0])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
            cnvsCircles.Children.Add(cirkelLabel);
        }

        private void NewChb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Ellipse cirkelLabel = new Ellipse();
            SolidColorBrush ledFill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(sldRed.Value), Convert.ToByte(sldGreen.Value), Convert.ToByte(sldBlue.Value)));
            cirkelLabel.Fill = ledFill;
            cirkelLabel.StrokeThickness = 1;
            cirkelLabel.Stroke = new SolidColorBrush(Colors.Black);
            cirkelLabel.Width = cirkelLabel.Height = 20;
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');
            cirkelLabel.Tag = cb.Tag;
            cirkelLabel.MouseDown += CirkelLabel_MouseDown;

            PointF centerCirkel = new PointF(circleCenter, circleCenter);
            PointF endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos((Convert.ToDouble(coords[1]) - 31) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[1])) * angle1)));

            PointF coord1;
            PointF coord2;

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[0])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);

            cnvsCircles.Children.Add(cirkelLabel);
        }

        private void CirkelLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse temp = sender as Ellipse;

            Debug.WriteLine(temp.Tag);

            string currentButton = "chb" + temp.Tag;            
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
