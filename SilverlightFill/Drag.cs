using System;
using System.Collections.Generic;
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

		public static List<WriteableBitmap> imageBackupList = new List<WriteableBitmap>();
		private static WriteableBitmap imageBackup;
		private static double maxLeft;
		private static double maxRight;
		private static double maxTop;
		private static double maxBottom;
		private static bool flag1 = false;
		private static bool flag2 = false;

		private static double offSetLeft = -1;
		private static double offSetRight = -1;
		private static double offSetTop = -1;
		private static double offSetBottom = -1;

		private static bool outOfBound = false;

		public static List<Point> imageBackupOffSet = new List<Point>();


		public static void down(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot)
		{
			dragStarted = true;

			

			clickedLayer = Common.hitTestLayer(e, inkCanvas);
			initialPos = e.GetPosition(inkCanvas);

			StylusPointCollection spc = new StylusPointCollection();
			spc.Add(new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y));

			hitCount = inkCanvas.Strokes.HitTest(spc).Count;

			System.Diagnostics.Debug.WriteLine("layer " + clickedLayer);
			if (clickedLayer != -1)
			{
				//temporarly save image in image backup
				imageBackup = new WriteableBitmap((BitmapSource)MainPage.imageList[clickedLayer].Source);

				//Calculate max left, right, top, bottom 
				calculateMax(MainPage.imageList[clickedLayer]);

				offSetLeft = e.GetPosition(inkCanvas).X - maxLeft;
				offSetRight = maxRight - e.GetPosition(inkCanvas).X;
				offSetTop = e.GetPosition(inkCanvas).Y - maxTop;
				offSetBottom = maxBottom - e.GetPosition(inkCanvas).Y;


				//if the clickedLayer's image is out of bound
				if (clickedLayer != -1 && imageBackupList[clickedLayer] != null)
				{
					outOfBound = true;
					System.Diagnostics.Debug.WriteLine("here");
					imageBackup = new WriteableBitmap(imageBackupList[clickedLayer]);

					//replace the moving img with the backupimage
					Image img = new Image();
					img.Source = imageBackupList[clickedLayer];
					img.Stretch = Stretch.None;



					int count = LayoutRoot.Children.IndexOf(MainPage.imageList[clickedLayer]);
					LayoutRoot.Children.RemoveAt(count);
					LayoutRoot.Children.Insert(count, img);
					MainPage.imageList[clickedLayer] = img;

					img.Margin = new Thickness(imageBackupOffSet[clickedLayer].X, imageBackupOffSet[clickedLayer].Y,-imageBackupOffSet[clickedLayer].X, -imageBackupOffSet[clickedLayer].Y);

				}

			}
			
			
			

			//Line clicked
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
				layerIndex = inkCanvas.Strokes.IndexOf(inkCanvas.Strokes.HitTest(spc)[hitCount-1]);
				colorTemp = inkCanvas.Strokes.HitTest(spc)[hitCount-1].DrawingAttributes.Color;
				inkCanvas.Strokes.Remove(inkCanvas.Strokes.HitTest(spc)[hitCount-1]);

			}

		}

		private static void calculateMax(Image image)
		{

			WriteableBitmap wb = MainPage.wbList[clickedLayer];

			//max left and right
			for (int w = 0; w < MainPage.wbList[clickedLayer].PixelWidth; w++)
			{
				for (int h = 0; h < MainPage.wbList[clickedLayer].PixelHeight; h++)
				{
					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && !flag1)
					{
						flag1 = true;
						maxLeft = w;
					}

					if (wb.GetPixel(w, h) != Color.FromArgb(0,0,0,0) && flag1)
					{
						flag2 = true;
					}
				}

				if (flag1 && !flag2)
				{
					maxRight = w;
					break;
				}


				flag2 = false;
			}


			flag1 = false;
			flag2 = false;

			//max top and bottom
			for (int h = 0; h < MainPage.wbList[clickedLayer].PixelHeight; h++)
			{
				for (int w = 0; w < MainPage.wbList[clickedLayer].PixelWidth; w++)
				{
					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && !flag1)
					{
						flag1 = true;
						maxTop = h;
				}	

					if (wb.GetPixel(w, h) != Color.FromArgb(0, 0, 0, 0) && flag1)
					{
						flag2 = true;
					}
				}

				if (flag1 && !flag2)
				{
					maxBottom = h;
					break;
				}


				flag2 = false;
			}

			flag1 = false;
			flag2 = false;

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


				WriteableBitmap tempWb = new WriteableBitmap((BitmapSource)MainPage.imageList[clickedLayer].Source);
				

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

				if (outOfBound)
				{
					outOfBound = false;

					//link back to wbList
					MainPage.imageList[clickedLayer].Source = MainPage.wbList[clickedLayer];

					//clear backupList
					imageBackupList[clickedLayer] = null;

					System.Diagnostics.Debug.WriteLine("here in outOfBound");
				}

				//if picture out of border

				//havent do when no longer out of border//
				if (e.GetPosition(inkCanvas).X - offSetLeft <= 0)
				{
					System.Diagnostics.Debug.WriteLine("left out");
					imageBackupList.Insert(clickedLayer, imageBackup);
					imageBackupOffSet.Insert(clickedLayer, new Point(img.Margin.Left, img.Margin.Top));
					
				} else if (e.GetPosition(inkCanvas).X + offSetRight >= MainPage.wbList[clickedLayer].PixelWidth)
				{
					System.Diagnostics.Debug.WriteLine("right out");
					imageBackupList.Insert(clickedLayer, imageBackup);
					imageBackupOffSet.Insert(clickedLayer, new Point(img.Margin.Left, img.Margin.Top));
				} else if (e.GetPosition(inkCanvas).Y - offSetTop <= 0)
				{
					System.Diagnostics.Debug.WriteLine("top out");
					imageBackupList.Insert(clickedLayer, imageBackup);
					imageBackupOffSet.Insert(clickedLayer, new Point(img.Margin.Left, img.Margin.Top));
				} else if (e.GetPosition(inkCanvas).Y + offSetBottom >= MainPage.wbList[clickedLayer].PixelHeight)
				{
					System.Diagnostics.Debug.WriteLine("bottom out");
					imageBackupList.Insert(clickedLayer, imageBackup);
					imageBackupOffSet.Insert(clickedLayer, new Point(img.Margin.Left, img.Margin.Top));
				}

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

			if (clickedLayer != -1)
			{
				MainPage.imageList[clickedLayer].Margin = new Thickness();
			}
			

		}


	}
}
