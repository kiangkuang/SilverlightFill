using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace SilverlightFill
{
    public partial class MainPage : UserControl
    {
        private Color color = Colors.Red;
        private Boolean fillmode = false;
        private Stroke newStroke = null;
        public WriteableBitmap wb;
        public MainPage()
        {
            InitializeComponent();
        }

         private void convertToBitmap()
        {
            // render InkCanvas' visual tree to the RenderTargetBitmap
            wb = new WriteableBitmap(1500, 1500);
            wb.Render(inkCanvas, new TranslateTransform());
            wb.Invalidate();

        }

        private void buttonRed(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Red);
            color = Colors.Red;
        }

        private void buttonOrange(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Orange);
            color = Colors.Orange;
        }

        private void buttonYellow(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Yellow);
            color = Colors.Yellow;
        }
        private void buttonGreen(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Green);
            color = Colors.Green;
        }
        private void buttonBlue(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Blue);
            color = Colors.Blue;
        }
        private void buttonMagenta(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Magenta);
            color = Colors.Magenta;
        }
        private void buttonPurple(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Purple);
            color = Colors.Purple;
        }
        private void buttonBlack(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Black);
            color = Colors.Black;
        }
        private void buttonWhite(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.White);
            color = Colors.White;
        }

        private void clear(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            convertToBitmap();
            strokeCounter.Content = "Strokes: " + inkCanvas.Strokes.Count;
        }

        private void fill(object sender, RoutedEventArgs e)
        {
            convertToBitmap();

            if (fillmode == false)
            {
                fillButton.Content = "Ink";
                fillmode = true;
            }
            else
            {
                fillButton.Content = "Fill";
                fillmode = false;
            }

        }

        private void inkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            inkCanvas.CaptureMouse();
            newStroke = new Stroke();
            newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
            inkCanvas.Strokes.Add(newStroke);
        }

        private void inkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (newStroke != null)
            {
                newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
            }
        }

        private void inkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            newStroke = null;
            inkCanvas.ReleaseMouseCapture();
            strokeCounter.Content = "Strokes: " + inkCanvas.Strokes.Count;
        }

    }
}
