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

	
		private static WriteableBitmap imageBackup;
		private const int LEFT = 0;
		private const int RIGHT = 1;
		private const int TOP = 2;
		private const int BOTTOM = 3;


		private static double offSetLeft = -1;
		private static double offSetRight = -1;
		private static double offSetTop = -1;
		private static double offSetBottom = -1;

		private static bool outOfBound = false;

		
		


		public static void down(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot)
		{
			dragStarted = true;

			clickedLayer = Common.hitTestLayer(e, inkCanvas);
			initialPos = e.GetPosition(inkCanvas);

			StylusPointCollection spc = new StylusPointCollection();
			spc.Add(new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y));

			hitCount = inkCanvas.Strokes.HitTest(spc).Count;

			System.Diagnostics.Debug.WriteLine("layer " + clickedLayer);
			System.Diagnostics.Debug.WriteLine("maxLeft " + MainPage.imageMaxOffSet[clickedLayer][LEFT]);


			if (clickedLayer != -1)
			{

				//Calculate max left, right, top, bottom 
				offSetLeft = e.GetPosition(inkCanvas).X - MainPage.imageMaxOffSet[clickedLayer][LEFT];
				offSetRight = MainPage.imageMaxOffSet[clickedLayer][RIGHT] - e.GetPosition(inkCanvas).X;
				offSetTop = e.GetPosition(inkCanvas).Y - MainPage.imageMaxOffSet[clickedLayer][TOP];
				offSetBottom = MainPage.imageMaxOffSet[clickedLayer][BOTTOM] - e.GetPosition(inkCanvas).Y;


				//if the clickedLayer's image is out of bound
				if (clickedLayer != -1 && MainPage.imageBackupList[clickedLayer] != null)
				{
					outOfBound = true;
					
					//backup image
					imageBackup = MainPage.imageBackupList[clickedLayer].Clone();
					

					//replace the moving img with the backupimage
					Image img = new Image();
					img.Source = MainPage.imageBackupList[clickedLayer];
					img.Stretch = Stretch.None;


					int count = LayoutRoot.Children.IndexOf(MainPage.imageList[clickedLayer]);
					LayoutRoot.Children.RemoveAt(count);
					LayoutRoot.Children.Insert(count, img);
					MainPage.imageList[clickedLayer] = img;

					img.Margin = new Thickness(MainPage.imageBackupOffSet[clickedLayer].X, MainPage.imageBackupOffSet[clickedLayer].Y, -MainPage.imageBackupOffSet[clickedLayer].X, -MainPage.imageBackupOffSet[clickedLayer].Y);

				}
				else
				{
					//temporarly save image in image backup
					imageBackup = new WriteableBitmap((BitmapSource)MainPage.imageList[clickedLayer].Source);
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
					MainPage.imageBackupList[clickedLayer] = null;
				}

				System.Diagnostics.Debug.WriteLine("offSetLeft " + offSetLeft);

				Common.checkIfOutOfBound(e, inkCanvas, img, clickedLayer, imageBackup, offSetLeft, offSetRight, offSetTop, offSetBottom, MainPage.imageMaxOffSet[clickedLayer][LEFT], MainPage.imageMaxOffSet[clickedLayer][RIGHT], MainPage.imageMaxOffSet[clickedLayer][TOP], MainPage.imageMaxOffSet[clickedLayer][BOTTOM]);

				

			
				
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

				//calculate max
				Image img = new Image();
				img.Source = imageBackup;
				Common.calculateMax(img, e, inkCanvas, clickedLayer);
			}
			

		}

		


	}
}
