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
		private Color selectedColor = Colors.Black;
		private Stroke newStroke = null;
		private WriteableBitmap wb;
		private List<InkPresenter> presenterList = new List<InkPresenter>();

		private int mode;
		private const int INKMODE = 0;
		private const int FILLMODE = 1;
		private const int DRAGMODE = 2;
		private bool dragStarted = false;
		private Point dragStartPos;
		private Point hitTestPos;
		private Color dragColor;
		private int dragFillIndex;

		private double deltaX;
		private double deltaY;

		public MainPage()
		{
			InitializeComponent();
		}

		private void convertToBitmap()
		{
			InkPresenter temp = new InkPresenter();
			for (int i = 0; i < presenterList.Count; i++)
			{
				for (int j = 0; j < presenterList[i].Strokes.Count; j++)
				{
					temp.Strokes.Add(presenterList[i].Strokes[j]);
				}
			}
			for (int i = 0; i < inkCanvas.Strokes.Count; i++)
			{
				temp.Strokes.Add(inkCanvas.Strokes[i]);
			}

			wb = new WriteableBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight);
			wb.Render(temp, new TranslateTransform());
			wb.Invalidate();
		}

		private void buttonRed(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Red);
			selectedColor = Colors.Red;
		}
		private void buttonOrange(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Orange);
			selectedColor = Colors.Orange;
		}
		private void buttonYellow(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Yellow);
			selectedColor = Colors.Yellow;
		}
		private void buttonGreen(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Green);
			selectedColor = Colors.Green;
		}
		private void buttonBlue(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Blue);
			selectedColor = Colors.Blue;
		}
		private void buttonMagenta(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Magenta);
			selectedColor = Colors.Magenta;
		}
		private void buttonPurple(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Purple);
			selectedColor = Colors.Purple;
		}
		private void buttonBlack(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Black);
			selectedColor = Colors.Black;
		}
		private void buttonWhite(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.White);
			selectedColor = Colors.White;
		}

		private void clear(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < presenterList.Count; i++)
			{
				LayoutRoot.Children.Remove(presenterList[i]);
			}
			inkCanvas.Strokes.Clear();
			presenterList.Clear();

			convertToBitmap();
			strokeCounter.Content = "Fill Layers: " + presenterList.Count;
		}

		private void ink(object sender, RoutedEventArgs e)
		{
			inkButton.FontWeight = FontWeights.Bold;
			fillButton.FontWeight = dragButton.FontWeight = FontWeights.Normal;
			mode = INKMODE;
		}
		private void fill(object sender, RoutedEventArgs e)
		{
			fillButton.FontWeight = FontWeights.Bold;
			inkButton.FontWeight = dragButton.FontWeight = FontWeights.Normal;
			mode = FILLMODE;
		}
		private void drag(object sender, RoutedEventArgs e)
		{
			dragButton.FontWeight = FontWeights.Bold;
			inkButton.FontWeight = fillButton.FontWeight = FontWeights.Normal;
			mode = DRAGMODE;
		}

		private void inkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			switch (mode)
			{
				case INKMODE:
					inkCanvas.CaptureMouse();
					newStroke = new Stroke();
					newStroke.DrawingAttributes.Color = selectedColor;
					newStroke.DrawingAttributes.Height = 5;
					newStroke.DrawingAttributes.Width = 5;

					newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
					inkCanvas.Strokes.Add(newStroke);
					break;
				case FILLMODE:

					break;
				case DRAGMODE:
					convertToBitmap();

					dragStarted = true;
					dragStartPos = new Point(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);

					dragFillIndex = -1;

					//identifying which fill area
					for (int i = presenterList.Count - 1; i >= 0; i--)
					{
						StylusPointCollection targetedStylusPoint = new StylusPointCollection();
						hitTestPos = new Point(e.GetPosition(presenterList[i]).X, e.GetPosition(presenterList[i]).Y);
						targetedStylusPoint.Add(new StylusPoint(hitTestPos.X, hitTestPos.Y));

						int count = presenterList[i].Strokes.HitTest(targetedStylusPoint).Count;
						if (count > 0)
						{
							dragFillIndex = i;
							count = 0;
							break;
						}
					}
					break;
			}
		}

		private void inkCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			switch (mode)
			{
				case INKMODE:
					if (newStroke != null)
					{
						newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
					}
					break;
				case FILLMODE:

					break;
				case DRAGMODE:
					if (dragStarted == true && dragFillIndex != -1)
					{
						deltaX = e.GetPosition(inkCanvas).X - dragStartPos.X;
						deltaY = e.GetPosition(inkCanvas).Y - dragStartPos.Y;

						InkPresenter ip = presenterList[dragFillIndex];
						ip.Margin = new Thickness(ip.Margin.Left + deltaX, ip.Margin.Top + deltaY, ip.Margin.Right + deltaX, ip.Margin.Bottom + deltaY);

						dragStartPos.X = e.GetPosition(inkCanvas).X;
						dragStartPos.Y = e.GetPosition(inkCanvas).Y;
					}
					break;
			}
		}

		private void inkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			switch (mode)
			{
				case INKMODE:
					newStroke = null;
					inkCanvas.ReleaseMouseCapture();
					break;
				case FILLMODE:
					convertToBitmap();
					Color targetColor = wb.GetPixel((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y);

					InkPresenter newPresenter = new InkPresenter();
					LayoutRoot.Children.Add(newPresenter);
					presenterList.Add(newPresenter);
					floodfill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, selectedColor, newPresenter);

					strokeCounter.Content = "Fill Layers: " + presenterList.Count;
					break;
				case DRAGMODE:
					dragStarted = false;
					if (dragFillIndex != -1)
					{
						InkPresenter tempIP = new InkPresenter();
						Color replacementColor = presenterList[dragFillIndex].Strokes[0].DrawingAttributes.Color;
						//redraw
						for (int i = 0; i < presenterList[dragFillIndex].Strokes.Count; i++)
						{
							InkPresenter ip = presenterList[dragFillIndex];
							ip.Strokes[i].StylusPoints.Add(new StylusPoint(ip.Strokes[i].StylusPoints[0].X + ip.Margin.Left, ip.Strokes[i].StylusPoints[0].Y + ip.Margin.Top));
							ip.Strokes[i].StylusPoints.Add(new StylusPoint(ip.Strokes[i].StylusPoints[1].X + ip.Margin.Left, ip.Strokes[i].StylusPoints[1].Y + ip.Margin.Top));
							ip.Strokes[i].StylusPoints.RemoveAt(0);
							ip.Strokes[i].StylusPoints.RemoveAt(0);
						}

						presenterList[dragFillIndex].Margin = new Thickness();
					}

					convertToBitmap();
					break;
			}
		}

		private void floodfill(Point pt, Color targetColor, Color replacementColor, InkPresenter presenter)
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
					presenter.Strokes.Add(stroke);

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
		}
	}
}
