using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

        List<CheckBox> checkList = new List<CheckBox>();
        List<Ellipse> ellipseList = new List<Ellipse>();
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < numberOfLeds; i++)
            {
                // Tekent de cirkels van het wiel
                Ellipse ellipse = new Ellipse();
                ellipse.Width = ellipse.Height = wheelCircle + i * (ledDistance * 2) + ledDistance * 2;
                ellipse.StrokeThickness = 1;
                ellipse.Margin = new Thickness(ledDistance * (16 - i), ledDistance * (16 - i), 0, 0);
                ellipse.Stroke = new SolidColorBrush(Colors.Gray);
                cnvsCircles.Children.Add(ellipse);
            }

            // Tekent een wit vierkant om de onderste helft van de cirkels te verbergen.
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Height = circleCenter;
            rectangle.Width = circleCenter * 2;
            rectangle.Margin = new Thickness(0, circleCenter, 0, 0);
            cnvsCircles.Children.Add(rectangle);

            for (int i = 0; i < numberOfLeds; i++)
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

            for (int j = 0; j < numberOfSegments; j++)
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

            for (int j = 0; j < numberOfSegments; j++)
            {
                for (int i = 0; i < numberOfLeds; i++)
                {
                    // Tekent de checkboxen die de data array moeten voorstellen
                    System.Windows.Controls.CheckBox newChb = new CheckBox();

                    newChb.Tag = (j).ToString() + "," + (i).ToString();
                    newChb.Checked += NewChb_Checked;
                    newChb.Unchecked += NewChb_Unchecked;
                    newChb.Margin = new Thickness(j * 30 + 50, i * 30 + circleCenter + 100, 0, 0);

                    checkList.Add(newChb);
                    cnvsCircles.Children.Add(newChb);

                    // Tekent de bollen die de leds representateren op de snijpunten van de ellipsen met de rechten.
                    Ellipse cirkelLabel = new Ellipse();
                    cirkelLabel.Fill = new SolidColorBrush(Colors.Black);
                    cirkelLabel.Tag = j.ToString() + "," + (15 -i).ToString();

                    endCirkel = new PointF((float)Convert.ToDecimal(circleCenter - Math.Cos(j * angle1)), (float)Convert.ToDecimal(circleCenter + Math.Sin(-j * angle1)));

                    Calculations.FindLineCircleIntersections(circleCenter, circleCenter, (circleCenter - ledDistance) - i * ledDistance, centerCirkel, endCirkel, out coord1, out coord2);

                    cirkelLabel.Margin = new Thickness(coord1.X - 10, coord1.Y - 10, 0, 0);

                    ellipseList.Add(cirkelLabel);
                    cnvsCircles.Children.Add(cirkelLabel);
                }
            }
        }

        private void NewChb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox? cb = sender as CheckBox;

            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');

            Ellipse ellipseChange = ellipseList[16 * Convert.ToInt32(coords[0]) + Convert.ToInt32(coords[1])];

            ellipseChange.Fill = new SolidColorBrush(Colors.Black);

            cnvsCircles.Children.Remove(ellipseChange);
            cnvsCircles.Children.Add(ellipseChange);

            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].brightness = 0;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].red = 0;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].green = 0;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].blue = 0;
        }

        private void NewChb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? cb = sender as CheckBox;
            SolidColorBrush ledFill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(sldRed.Value), Convert.ToByte(sldGreen.Value), Convert.ToByte(sldBlue.Value)));

            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');

            Ellipse ellipseChange = ellipseList[16 * Convert.ToInt32(coords[0]) + Convert.ToInt32(coords[1])];
            
            ellipseChange.Fill = ledFill;
            
            cnvsCircles.Children.Remove(ellipseChange);
            cnvsCircles.Children.Add(ellipseChange);

            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].brightness = sldIntensity.Value;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].red = sldRed.Value;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].green = sldGreen.Value;
            DataByte[Convert.ToInt16(coords[0]), Convert.ToInt16(coords[1])].blue = sldBlue.Value;
            
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
            using StreamWriter writeData = new StreamWriter("SendData.txt");
            
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    writeData.Write($"{Convert.ToString(DataByte[i, j].brightness)},{Convert.ToString(DataByte[i, j].red)},{Convert.ToString(DataByte[i, j].green)},{Convert.ToString(DataByte[i, j].blue)},");
                }
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            btnReset_Click(sender,e);

            APA102C[,] importAPA102C = new APA102C[32,16];
            string importData = System.IO.File.ReadAllText(@"C:\\Users\\aaron\\Desktop\\Bussiness project 2\\Circle display for leds\\circle display\\bin\\Debug\\net6.0-windows\SendData.txt");
            string[] importArray = importData.Split(",");
            for (int i = 0; i < numberOfSegments; i++)
            {
                for (int j = 0; j < numberOfLeds; j++)
                {
                    importAPA102C[i, j].brightness = Convert.ToDouble(importArray[64 * i + 4 * j]);
                    importAPA102C[i, j].red = Convert.ToDouble(importArray[64 * i + 4 * j + 1]);
                    importAPA102C[i, j].green = Convert.ToDouble(importArray[64 * i + 4 * j + 2]);
                    importAPA102C[i, j].blue = Convert.ToDouble(importArray[64 * i + 4 * j + 3]);

                    if (importAPA102C[i,j].brightness != 0)
                    {
                        CheckBox setCheck = checkList[16 * i + j];
                        
                        sldRed.Value = importAPA102C[i, j].red;
                        sldGreen.Value = importAPA102C[i, j].green;
                        sldBlue.Value = importAPA102C[i, j].blue;

                        setCheck.IsChecked = true;
                    }
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
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
