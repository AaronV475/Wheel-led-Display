using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        static int wheelCircle = 400; // Diameter van de kleinste cirkel van het wiel.
        public static int ledDistance = 25; // This is a the distance between the leds in a row.

        static int numberOfLeds = 16; // Aantal leds per rij / lengte van de ledstrip.
        static int numberOfSegments = 32; // Aantal segmenten waarin het halve of hele wiel is ingedeeld.

        public static int circleCenter = (wheelCircle + (ledDistance * 2) * (numberOfLeds + 1)) / 2; // Y-locatie van het centrum van het wiel.


        PointF centerCirkel = new PointF(circleCenter, circleCenter);
        PointF endCirkel;
        PointF coord1;
        PointF coord2;

        struct APA102C
        {
            public double brightness;
            public double red;
            public double green;
            public double blue;
        }

        APA102C[,] DataByte = new APA102C[32, 16];


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
            cnvsCircles.Children.Add(rectangle);

            

            for (int i = 0; i < 16; i++)
            {
                // Zet de labels bij de checkboxen met het aantal rijen aangeduid
                Label labelChbRow = new Label();
                labelChbRow.Content = i.ToString();
                labelChbRow.Height = labelChbRow.Width = 30;
                labelChbRow.HorizontalContentAlignment = HorizontalAlignment.Right;
                labelChbRow.Margin = new Thickness(15, (15 - i) * 30 + circleCenter + 95, 0, 0);
                cnvsCircles.Children.Add(labelChbRow);

                // Zet de labels bij de cirkels met het aantal rijen aangeduid
                Label labelCrkRow = new Label();
                labelCrkRow.Content = (15 - i).ToString();
                labelCrkRow.Height = labelCrkRow.Width = 30;
                labelCrkRow.Margin = new Thickness(i * ledDistance + 15, circleCenter + 10, 0, 0);
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
                Label lblKolom = new Label();

                lblKolom.Content = j.ToString();
                lblKolom.Height = lblKolom.Width = 30;
                lblKolom.HorizontalContentAlignment = HorizontalAlignment.Center;
                lblKolom.Margin = new Thickness(j * 30 + 43, circleCenter + 90 + numberOfLeds * 30, 0, 0);
                cnvsCircles.Children.Add(lblKolom);

                // Tekent de labels bij de cirkel kolommen.
                Label lblCirkel = new Label();

                lblCirkel.Content = (31 - j).ToString();
                lblCirkel.Height = lblCirkel.Width = 30;
                lblCirkel.HorizontalAlignment = HorizontalAlignment.Left;
                lblCirkel.VerticalAlignment = VerticalAlignment.Top;

                endCirkel = new PointF((float)Convert.ToDecimal(circleCenter + Math.Cos(j * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-j * angle1)));

                Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter, centerCirkel, endCirkel, out coord1, out coord2);

                lblCirkel.Margin = new Thickness(coord1.X - 10, coord1.Y - 15, 0, 0);
                cnvsCircles.Children.Add(lblCirkel);
            }

            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    // Tekent de checkboxen die de data array moeten voorstellen
                    System.Windows.Controls.CheckBox newChb = new CheckBox();

                    newChb.Tag = (j).ToString() + "," + (15 -i).ToString();
                    newChb.Checked += NewChb_Checked;
                    newChb.Unchecked += NewChb_Unchecked;
                    newChb.Margin = new Thickness(j * 30 + 50, i * 30 + circleCenter + 100, 0, 0);
                    cnvsCircles.Children.Add(newChb);

                    // Tekent de bollen die de leds representateren op de snijpunten van de ellipsen met de rechten.
                    Ellipse cirkelLabel = new Ellipse();
                    cirkelLabel.Fill = new SolidColorBrush(Colors.Black);
                    cirkelLabel.MouseDown += CirkelLabel_MouseDown;
                    cirkelLabel.Tag = j.ToString() + "," + (15 -i).ToString();

                    endCirkel = new PointF((float)Convert.ToDecimal(circleCenter - Math.Cos(j * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-j * angle1)));

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
            cirkelLabel.Fill = new SolidColorBrush(Colors.Black);
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');
            cirkelLabel.Tag = cb.Tag;
            cirkelLabel.MouseDown += CirkelLabel_MouseDown;

            endCirkel = new PointF((float)Convert.ToDecimal(circleCenter - Math.Cos((Convert.ToDouble(coords[0])) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[0])) * angle1)));

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[1])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);
            cnvsCircles.Children.Add(cirkelLabel);

            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].brightness = 0;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].red = 0;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].green = 0;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].blue = 0;

        }

        private void NewChb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Ellipse cirkelLabel = new Ellipse();
            SolidColorBrush ledFill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(sldRed.Value), Convert.ToByte(sldGreen.Value), Convert.ToByte(sldBlue.Value)));
            cirkelLabel.Fill = ledFill;
            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');
            cirkelLabel.Tag = cb.Tag;
            cirkelLabel.MouseDown += CirkelLabel_MouseDown;

            endCirkel = new PointF((float)Convert.ToDecimal(circleCenter - Math.Cos((Convert.ToDouble(coords[0])) * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-1 * (Convert.ToDouble(coords[0])) * angle1)));

            Calculations.FindLineCircleIntersections(circleCenter, circleCenter, circleCenter - ledDistance - ((float)Convert.ToDecimal(coords[1])) * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);
            cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);

            cnvsCircles.Children.Add(cirkelLabel);


            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].brightness = sldIntensity.Value;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].red = sldRed.Value;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].green = sldGreen.Value;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].blue = sldBlue.Value;
            
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

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writeData = new StreamWriter("SendData.txt"))
            
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    writeData.Write($"{Convert.ToString(DataByte[i, j].brightness)},{Convert.ToString(DataByte[i, j].red)},{Convert.ToString(DataByte[i, j].green)},{Convert.ToString(DataByte[i, j].blue)},");
                }
            }
            
        }
    }
}
