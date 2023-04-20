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
using Microsoft.Win32;

namespace circle_display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly double angle1 = 0.0997331001; // Hoek tussen de segmenten in radialen. (Moet nog in formule verwerkt worden.)

        public static readonly int wheelCircle = 600; // Diameter van de kleinste cirkel van het wiel.
        public static readonly int ledDistance = 25; // This is a the distance between the leds in a row.

        public static readonly int numberOfLeds = 16; // Aantal leds per rij / lengte van de ledstrip.
        public static readonly int numberOfSegments = 63; // Aantal segmenten waarin het halve of hele wiel is ingedeeld.
        public static readonly int numberOfLedstrips = 3; // Aantal ledstrips aanwezig op het wiel.

        public static readonly int circleCenter = (wheelCircle + (ledDistance * 2) * (numberOfLeds + 1)) / 2; // Y-locatie van het centrum van het wiel.


        public static readonly PointF centerCirkel = new PointF(circleCenter, circleCenter);
        public static PointF endCirkel;
        public static PointF coord1;
        public static PointF coord2;

        public struct APA102C
        {
            public double brightness;
            public double red;
            public double green;
            public double blue;
        }

        public APA102C[,] DataByte = new APA102C[numberOfSegments, numberOfLeds];

        public static List<CheckBox> checkList = new List<CheckBox>();
        public static List<Ellipse> ellipseList = new List<Ellipse>();

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
                StaticDisplay.PrintWheelCirkels(ellipse, i);
                cnvsCircles.Children.Add(ellipse);

                // Zet de labels bij de checkboxen met het aantal rijen aangeduid
                Label label = new Label();
                StaticDisplay.PrintCheckboxLabelY(label, i);
                cnvsCheckboxes.Children.Add(label);
            }

            for (int j = 0; j < numberOfSegments; j++)
            {
                // Tekent de lijnen die de segmenten verdelen
                Line line = new Line();
                StaticDisplay.PrintWheelLine(line, j);
                cnvsCircles.Children.Add(line);

                // Tekent de labels bij de checkboxen met het aantal kolommen aangeduid
                Label lblCheckbox = new Label();
                StaticDisplay.PrintCheckboxLabelX(lblCheckbox, j);
                cnvsCheckboxes.Children.Add(lblCheckbox);

                // Tekent de labels bij de cirkel kolommen.
                Label lblCirkel = new Label();
                StaticDisplay.PrintWheelLabel(lblCirkel, j);
                cnvsCircles.Children.Add(lblCirkel);
            }

            for (int j = 0; j < numberOfSegments; j++)
            {
                for (int i = 0; i < numberOfLeds; i++)
                {
                    // Tekent de checkboxen die de data array moeten voorstellen
                    CheckBox _checkbox = new CheckBox();
                    DynamicEvents.CheckboxPrint(_checkbox, i, j);

                    _checkbox.Checked += NewChb_Checked;
                    _checkbox.Unchecked += NewChb_Unchecked;

                    checkList.Add(_checkbox);
                    cnvsCheckboxes.Children.Add(_checkbox);

                    // Tekent de bollen die de leds representateren op de snijpunten van de ellipsen met de rechten.
                    Ellipse LED = new Ellipse();
                    //LED.MouseLeftButtonDown += LED_MouseLeftButtonDown;
                    //LED.MouseRightButtonDown += LED_MouseRightButtonDown;
                    DynamicEvents.LedPrint(LED, i, j);

                    ellipseList.Add(LED);
                    cnvsCircles.Children.Add(LED);
                }
            }
        }

        private void NewChb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox? cb = sender as CheckBox;

            string currentEllipse = (string)cb.Tag;
            string[] coords = currentEllipse.Split(',');

            Ellipse ellipseChange = ellipseList[numberOfLeds * Convert.ToInt32(coords[0]) + Convert.ToInt32(coords[1])];

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

            Ellipse ellipseChange = ellipseList[numberOfLeds * Convert.ToInt32(coords[0]) + Convert.ToInt32(coords[1])];
            
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

        // Import file selector nog maken. Zal ook gebruikt worden om de afbeelding in te laden.
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            DynamicEvents.Reset();

            OpenFileDialog openFileDialog = new OpenFileDialog(); //Document openen met openfiledialog
            string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.InitialDirectory = startFolder;
            openFileDialog.Filter = "Image Files| *.jpg; *.png|Text Files| *.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true) // User klikt op 'openen'
            {
                string filePath = openFileDialog.FileName;

                // Als de user een txt file selecteerd wordt die direct geprint op de checkboxesn en dus ook op de visual display
                if (filePath.Contains(".txt"))
                {
                    APA102C[,] importAPA102C = new APA102C[numberOfSegments, numberOfLeds];
                    string importData = System.IO.File.ReadAllText(filePath);
                    string[] importArray = importData.Split(",");
                    for (int i = 0; i < numberOfSegments; i++)
                    {
                        for (int j = 0; j < numberOfLeds; j++)
                        {
                            importAPA102C[i, j].brightness = Convert.ToDouble(importArray[64 * i + 4 * j]);
                            importAPA102C[i, j].red = Convert.ToDouble(importArray[64 * i + 4 * j + 1]);
                            importAPA102C[i, j].green = Convert.ToDouble(importArray[64 * i + 4 * j + 2]);
                            importAPA102C[i, j].blue = Convert.ToDouble(importArray[64 * i + 4 * j + 3]);

                            if (importAPA102C[i, j].brightness != 0)
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

                // Als de ingelezen file een image is wordt deze op een ander window gezet samen met de 32 x 16 bitmap die gebruikt wordt om de data uit te halen.
                // Deze wordt dan ook op de checkboxes gezet. (WIP)
                else if(filePath.Contains(".png") || filePath.Contains(".jpg"))
                {
                    BitmapImage image = new BitmapImage();
                    BitmapImage imageDecode = new BitmapImage();
                    ImageWindow windowTwo = new ImageWindow(this);
                    Bitmap _bitmap;
                    
                    double imageWidth = 600;
                    windowTwo.Owner = this;
                    windowTwo.Show();

                    imageDecode.BeginInit();
                    imageDecode.UriSource = new Uri(filePath);
                    imageDecode.DecodePixelWidth = numberOfSegments;
                    imageDecode.DecodePixelHeight = numberOfLeds;
                    imageDecode.EndInit();

                    image.BeginInit();
                    image.UriSource = new Uri(filePath);
                    image.EndInit();

                    windowTwo.EditImage(imageWidth, imageWidth * image.Height / image.Width, image);
                    windowTwo.EditBitmap(imageWidth, imageWidth * image.Height / image.Width, imageDecode);

                    using (MemoryStream outStream = new MemoryStream())
                    {
                        BitmapEncoder enc = new BmpBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(imageDecode));
                        enc.Save(outStream);
                        _bitmap = new System.Drawing.Bitmap(outStream);
                    }

                    APA102C[,] importAPA102C = new APA102C[numberOfSegments, numberOfLeds];

                    for (int i = 0; i < numberOfSegments; i++)
                    {
                        for (int j = 0; j < numberOfLeds; j++)
                        {
                            System.Drawing.Color _color = _bitmap.GetPixel(i,j);

                            importAPA102C[i, j].brightness = _color.A;
                            importAPA102C[i, j].red = _color.R;
                            importAPA102C[i, j].green = _color.G;
                            importAPA102C[i, j].blue = _color.B;

                            if (importAPA102C[i, j].brightness != 0)
                            {
                                CheckBox setCheck = checkList[numberOfLeds * i + j];

                                sldRed.Value = importAPA102C[i, j].red;
                                sldGreen.Value = importAPA102C[i, j].green;
                                sldBlue.Value = importAPA102C[i, j].blue;

                                setCheck.IsChecked = true;
                            }
                        }
                    }
                }

                else
                {
                    MessageBox.Show("This is not a valid file.\nPick a text file or an image.","Warning",MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            DynamicEvents.Reset();
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            DynamicEvents.Export(DataByte);
        }
    }
}