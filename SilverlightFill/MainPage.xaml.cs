﻿using System;
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
        private Color selectedColor = Colors.Red;
        private Boolean fillmode = false;
        private Stroke newStroke = null;
        private WriteableBitmap wb;
        private StrokeCollection lineList = new StrokeCollection();
        private Color inkColor = Colors.Black;

        private int mode;
        private const int INKMODE = 0;
        private const int FILLMODE = 1;
        private const int DRAGMODE = 2;



        public MainPage()
        {
            InitializeComponent();
        }

        private void convertToBitmap()
        {
            // render InkCanvas' visual tree to the RenderTargetBitmap
            wb = new WriteableBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight);
            wb.Render(inkCanvas, new TranslateTransform());
            wb.Invalidate();
        }

        private void buttonRed(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Red);
            selectedColor = Colors.Red;
            inkColor = Colors.Red;

        }
        private void buttonOrange(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Orange);
            selectedColor = Colors.Orange;
            inkColor = Colors.Orange;
        }
        private void buttonYellow(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Yellow);
            selectedColor = Colors.Yellow;
            inkColor = Colors.Yellow;
        }
        private void buttonGreen(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Green);
            selectedColor = Colors.Green;
            inkColor = Colors.Green;
        }
        private void buttonBlue(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Blue);
            selectedColor = Colors.Blue;
            inkColor = Colors.Blue;
        }
        private void buttonMagenta(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Magenta);
            selectedColor = Colors.Magenta;
            inkColor = Colors.Magenta;
        }
        private void buttonPurple(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Purple);
            selectedColor = Colors.Purple;
            inkColor = Colors.Purple;
        }
        private void buttonBlack(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.Black);
            selectedColor = Colors.Black;
            inkColor = Colors.Black;
        }
        private void buttonWhite(object sender, RoutedEventArgs e)
        {
            replaceBox.Fill = new SolidColorBrush(Colors.White);
            selectedColor = Colors.White;
            inkColor = Colors.White;
        }

        private void clear(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            lineList.Clear();
            convertToBitmap();
            strokeCounter.Content = "Strokes: " + inkCanvas.Strokes.Count;
        }

        private void fill(object sender, RoutedEventArgs e)
        {

            if (mode == 0)
            {
                fillButton.Content = "Ink";
                mode = 1;
            }
            else
            {
                fillButton.Content = "Fill";
                mode = 0;
            }

        }

        private void inkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mode == 0)
            {
                inkCanvas.CaptureMouse();
                newStroke = new Stroke();
                lineList.Add(newStroke);
                newStroke.DrawingAttributes.Color = inkColor;
                //newStroke.DrawingAttributes.OutlineColor = Colors.White;
                newStroke.DrawingAttributes.Height = 5;
                newStroke.DrawingAttributes.Width = 5;

                newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
                inkCanvas.Strokes.Add(newStroke);
            }
        }

        private void inkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mode == 0 && newStroke != null)
            {
                newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
            }
        }

        private void inkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (mode)
            {
                case INKMODE:
                    newStroke = null;
                    inkCanvas.ReleaseMouseCapture();
                    strokeCounter.Content = "Strokes: " + inkCanvas.Strokes.Count;
                    break;

                case FILLMODE:
                    convertToBitmap();
                    // fill method here
                    Color targetColor = wb.GetPixel((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y);
                
                    floodfill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, selectedColor);

                    foreach (Stroke s in lineList)
                    {
                        inkCanvas.Strokes.Remove(s);
                        inkCanvas.Strokes.Add(s);
                    }
                    break;

                default:
                    break;
            }
            
        }

        private void floodfill(Point pt, Color targetColor, Color replacementColor)
        {
            Queue<Point> q = new Queue<Point>();

            if (!ColorMatch(wb.GetPixel((int)pt.X, (int)pt.Y), targetColor) || ColorMatch(wb.GetPixel((int)pt.X, (int)pt.Y), replacementColor))
                return;

            q.Enqueue(pt);

            while (q.Count > 0)
            {
                Point n = q.Dequeue();

                if (ColorMatch(wb.GetPixel((int)n.X, (int)n.Y), targetColor)) //AreColorsSimilar(bitmap.GetPixel(n.X, n.Y), targetColor, 20)
                {
                    Point w = new Point(n.X, n.Y);
                    Point e = new Point(n.X, n.Y);

                    while ((w.X >= 0) && (w.X < wb.PixelWidth) && ColorMatch(wb.GetPixel((int)w.X, (int)w.Y), targetColor))//AreColorsSimilar(bitmap.GetPixel(w.X, w.Y), targetColor, 50)) //
                    {
                        w.X--;
                    }

                    while ((e.X >= 0) && (e.X < wb.PixelWidth) && ColorMatch(wb.GetPixel((int)e.X, (int)e.Y), targetColor))//AreColorsSimilar(bitmap.GetPixel(e.X, e.Y), targetColor, 50)) //
                    {
                        e.X++;
                    }

                    StylusPointCollection stylusPoints = new StylusPointCollection();
                    stylusPoints.Add(new StylusPoint(++w.X, w.Y));
                    stylusPoints.Add(new StylusPoint(--e.X, w.Y));
                    Stroke stroke = new Stroke(stylusPoints);
                    stroke.DrawingAttributes.Height = 4;
                    stroke.DrawingAttributes.Width = 4;
                    stroke.DrawingAttributes.Color = replacementColor;
                    inkCanvas.Strokes.Add(stroke);

                    for (int i = (int)w.X; i <= e.X; i++)
                    {
                        wb.SetPixel(i, (int)w.Y, replacementColor);
                        if ((i >= 0) && (i < wb.PixelWidth) && (w.Y + 1 >= 0 && w.Y + 1 < wb.PixelHeight) && ColorMatch(wb.GetPixel(i, (int)w.Y + 1), targetColor))
                        {
                            q.Enqueue(new Point(i, w.Y + 1));
                        }

                        if ((i >= 0) && (i < wb.PixelWidth) && (w.Y - 1 >= 0 && w.Y - 1 < wb.PixelHeight) && ColorMatch(wb.GetPixel(i, (int)w.Y - 1), targetColor))
                        {
                            q.Enqueue(new Point(i, w.Y - 1));
                        }

                    }

                }
            }

        }

        private static bool ColorMatch(Color a, Color b)
        {
            int tolerance = 32;
            return Math.Abs(a.A - b.A) < tolerance && 
                   Math.Abs(a.R - b.R) < tolerance &&
                   Math.Abs(a.G - b.G) < tolerance &&
                   Math.Abs(a.B - b.B) < tolerance;
            //return (a.Equals(b));
        }
    }
}
