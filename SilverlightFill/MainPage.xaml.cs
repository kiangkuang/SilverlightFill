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

namespace SilverlightFill
{
    public partial class MainPage : UserControl
    {
        public Color color = Colors.Red;
        public Boolean fillmode = false;
        public MainPage()
        {
            InitializeComponent();
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
        }

        private void fill(object sender, RoutedEventArgs e)
        {
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



    }
}
