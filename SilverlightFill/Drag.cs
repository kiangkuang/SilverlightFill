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
		private static double deltaX;
		private static double deltaY;
		private static Point initialPos;
		private static int clickedLayer = -1;


		public static void down(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			dragStarted = true;
			clickedLayer = Common.hitTestLayer(e, inkCanvas);
			initialPos = e.GetPosition(inkCanvas);
			
		}

		public static void move(MouseEventArgs e, InkPresenter inkCanvas)
		{
			if (dragStarted == true && clickedLayer != -1)
			{
				deltaX = e.GetPosition(inkCanvas).X - initialPos.X;
				deltaY = e.GetPosition(inkCanvas).Y - initialPos.Y;

				Image img = MainPage.imageList[clickedLayer];
				img.Margin = new Thickness(img.Margin.Left + deltaX, img.Margin.Top + deltaY, img.Margin.Right - deltaX, img.Margin.Bottom - deltaY);

				initialPos = e.GetPosition(inkCanvas);
			}
		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			dragStarted = false;
			if (clickedLayer != -1)
			{
				WriteableBitmap tempWb = new WriteableBitmap(MainPage.wbList[clickedLayer]);
				Image img = MainPage.imageList[clickedLayer];

				//redraw
				int w = MainPage.wbList[clickedLayer].PixelWidth;
				int h = MainPage.wbList[clickedLayer].PixelHeight;

				MainPage.wbList[clickedLayer].Clear();
				for (int i = 0; i < w; i++)
				{
					for (int j = 0; j < h; j++ )
					{
						if (tempWb.GetPixel(i, j) != Color.FromArgb(0, 0, 0, 0))
						{
							Color tempPixel = tempWb.GetPixel(i,j);

							int setX = i + (int)img.Margin.Left;
							int setY = j + (int)img.Margin.Top;
							if (setX < w && setX >= 0 && setY < h && setY >= 0) {
								MainPage.wbList[clickedLayer].SetPixel(setX, setY, tempPixel);
							}
							
						}
					}
					

				}

				MainPage.imageList[clickedLayer].Margin = new Thickness();



				
			}
		}
	}
}
