using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SilverlightFill
{
	public class Drag
	{
		private static bool dragStarted = false;
		private static bool inkClicked = false;
		private static double deltaX;
		private static double deltaY;
		private static Point initialPos;
		private static int clickedLayer = -1;

		private static InkPresenter ipToMove;
		private static Color colorTemp = Colors.Transparent;
		private static int hitCount = -1;
		private static int layerIndex = -1;
		


		public static void down(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot)
		{
			dragStarted = true;
			clickedLayer = Common.hitTestLayer(e, inkCanvas);
			initialPos = e.GetPosition(inkCanvas);

			StylusPointCollection spc = new StylusPointCollection();
			spc.Add(new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y));

			hitCount = inkCanvas.Strokes.HitTest(spc).Count;

			if (hitCount > 0)
			{
				inkClicked = true;
				
				// add to a new IP
				for (int i = 0; i < hitCount; i++)
				{
					ipToMove = new InkPresenter();
					ipToMove.Strokes.Add(inkCanvas.Strokes.HitTest(spc)[i]);
				}

				LayoutRoot.Children.Add(ipToMove);

				// remove ink
				if (hitCount > 1)
				{
					layerIndex = inkCanvas.Strokes.IndexOf(inkCanvas.Strokes.HitTest(spc)[hitCount-1]);
					colorTemp = inkCanvas.Strokes.HitTest(spc)[hitCount-1].DrawingAttributes.Color;
					inkCanvas.Strokes.Remove(inkCanvas.Strokes.HitTest(spc)[hitCount-1]);
				}
				else
				{
					layerIndex = inkCanvas.Strokes.IndexOf(inkCanvas.Strokes.HitTest(spc)[0]);
					colorTemp = inkCanvas.Strokes.HitTest(spc)[0].DrawingAttributes.Color;
					inkCanvas.Strokes.Remove(inkCanvas.Strokes.HitTest(spc)[0]);
				}
			}

		}

		public static void move(MouseEventArgs e, InkPresenter inkCanvas)
		{
			if (dragStarted == true && clickedLayer != -1 && !inkClicked)
			{
				deltaX = e.GetPosition(inkCanvas).X - initialPos.X;
				deltaY = e.GetPosition(inkCanvas).Y - initialPos.Y;

				Image img = MainPage.imageList[clickedLayer];
				img.Margin = new Thickness(img.Margin.Left + deltaX, img.Margin.Top + deltaY, img.Margin.Right - deltaX, img.Margin.Bottom - deltaY);

				initialPos = e.GetPosition(inkCanvas);
			}
			else if (dragStarted == true && inkClicked)
			{
				deltaX = e.GetPosition(inkCanvas).X - initialPos.X;
				deltaY = e.GetPosition(inkCanvas).Y - initialPos.Y;

				ipToMove.Margin = new Thickness(ipToMove.Margin.Left + deltaX, ipToMove.Margin.Top + deltaY, ipToMove.Margin.Right + deltaX, ipToMove.Margin.Bottom + deltaY);

				initialPos = e.GetPosition(inkCanvas);
			}
		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot)
		{
			dragStarted = false;
			if (clickedLayer != -1 && !inkClicked)
			{
				WriteableBitmap tempWb = new WriteableBitmap(MainPage.wbList[clickedLayer]);
				Image img = MainPage.imageList[clickedLayer];

				//redraw
				int w = MainPage.wbList[clickedLayer].PixelWidth;
				int h = MainPage.wbList[clickedLayer].PixelHeight;

				MainPage.wbList[clickedLayer].Clear();
				for (int i = 0; i < w; i++)
				{
					for (int j = 0; j < h; j++)
					{
						if (tempWb.GetPixel(i, j) != Color.FromArgb(0, 0, 0, 0))
						{
							Color tempPixel = tempWb.GetPixel(i, j);

							int setX = i + (int)img.Margin.Left;
							int setY = j + (int)img.Margin.Top;
							if (setX < w && setX >= 0 && setY < h && setY >= 0)
							{
								MainPage.wbList[clickedLayer].SetPixel(setX, setY, tempPixel);
							}

						}
					}
				}

				MainPage.imageList[clickedLayer].Margin = new Thickness();

			}
			else if (inkClicked)
			{

				for (int i = 0; i < ipToMove.Strokes.Count; i++)
				{
					StylusPointCollection spcTemp = new StylusPointCollection();

					for (int j = 0; j < ipToMove.Strokes[i].StylusPoints.Count; j++)
					{
						StylusPoint spTemp = new StylusPoint(ipToMove.Strokes[i].StylusPoints[j].X + ipToMove.Margin.Left, ipToMove.Strokes[i].StylusPoints[j].Y + ipToMove.Margin.Top);
						spcTemp.Add(spTemp);
					}

					Stroke newStroke = new Stroke();
					newStroke.DrawingAttributes.Color = colorTemp;
					newStroke.DrawingAttributes.Height = 5;
					newStroke.DrawingAttributes.Width = 5;

					newStroke.StylusPoints.Add(spcTemp);
					inkCanvas.Strokes.Insert(layerIndex, newStroke);
					
				}
					

				LayoutRoot.Children.Remove(ipToMove);

				inkClicked = false;
				
			}
		}


	}
}
