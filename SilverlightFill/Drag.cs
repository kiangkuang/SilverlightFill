using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightFill
{
	public class Drag
	{
		private static bool dragStarted = false;
		private static double deltaX;
		private static double deltaY;

		public static void down(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			dragStarted = true;
			Common.hitTestLayer(e, inkCanvas);
		}

		public static void move(MouseEventArgs e, InkPresenter inkCanvas)
		{
			if (dragStarted == true && Common.clickedLayer != -1)
			{
				deltaX = e.GetPosition(inkCanvas).X - Common.clickedPos.X;
				deltaY = e.GetPosition(inkCanvas).Y - Common.clickedPos.Y;

				InkPresenter ip = MainPage.presenterList[Common.clickedLayer];
				ip.Margin = new Thickness(ip.Margin.Left + deltaX, ip.Margin.Top + deltaY, ip.Margin.Right + deltaX, ip.Margin.Bottom + deltaY);

				Common.clickedPos.X = e.GetPosition(inkCanvas).X;
				Common.clickedPos.Y = e.GetPosition(inkCanvas).Y;
			}
		}

		public static void up(MouseButtonEventArgs e)
		{
			dragStarted = false;
			if (Common.clickedLayer != -1)
			{
				InkPresenter tempIP = new InkPresenter();
				Color replacementColor = MainPage.presenterList[Common.clickedLayer].Strokes[0].DrawingAttributes.Color;
				//redraw
				for (int i = 0; i < MainPage.presenterList[Common.clickedLayer].Strokes.Count; i++)
				{
					InkPresenter ip = MainPage.presenterList[Common.clickedLayer];
					ip.Strokes[i].StylusPoints.Add(new StylusPoint(ip.Strokes[i].StylusPoints[0].X + ip.Margin.Left, ip.Strokes[i].StylusPoints[0].Y + ip.Margin.Top));
					ip.Strokes[i].StylusPoints.Add(new StylusPoint(ip.Strokes[i].StylusPoints[1].X + ip.Margin.Left, ip.Strokes[i].StylusPoints[1].Y + ip.Margin.Top));
					ip.Strokes[i].StylusPoints.RemoveAt(0);
					ip.Strokes[i].StylusPoints.RemoveAt(0);
				}

				MainPage.presenterList[Common.clickedLayer].Margin = new Thickness();
			}
		}
	}
}
