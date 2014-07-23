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


			if (clickedLayer != -1)
			{

				//Calculate offset of left, right, top, bottom 
				calculateOffset(e, inkCanvas);

				//System.Diagnostics.Debug.WriteLine("maxLeft " + MainPage.layerList[clickedLayer].imageToBorderDist[LEFT]);
				//System.Diagnostics.Debug.WriteLine("offSetLeft " + offSetLeft);
				//System.Diagnostics.Debug.WriteLine("positionY" + e.GetPosition(inkCanvas).Y);
				//System.Diagnostics.Debug.WriteLine("maxTop " + MainPage.layerList[clickedLayer].imageToBorderDist[TOP]);
				//System.Diagnostics.Debug.WriteLine("offSetTop " + offSetTop);

				//if the clickedLayer's image is out of bound
				if (clickedLayer != -1 && MainPage.layerList[clickedLayer].imgBackup != null)
				{
					replaceMovingImgWithBackupImg(LayoutRoot);

				}
				else
				{
					//temporarly save image in image backup
					imageBackup = new WriteableBitmap((BitmapSource)MainPage.layerList[clickedLayer].img.Source);
				}

			}
			
			//Line clicked
			if (hitCount > 0)
			{
				moveInkToNewInkPresenter(inkCanvas, LayoutRoot, spc);
			}

		}

		private static void moveInkToNewInkPresenter(InkPresenter inkCanvas, Grid LayoutRoot, StylusPointCollection spc)
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
			layerIndex = inkCanvas.Strokes.IndexOf(inkCanvas.Strokes.HitTest(spc)[hitCount - 1]);
			colorTemp = inkCanvas.Strokes.HitTest(spc)[hitCount - 1].DrawingAttributes.Color;
			inkCanvas.Strokes.Remove(inkCanvas.Strokes.HitTest(spc)[hitCount - 1]);
		}

		private static void replaceMovingImgWithBackupImg(Grid LayoutRoot)
		{
			outOfBound = true;
			System.Diagnostics.Debug.WriteLine("OOB");
			//backup image
			imageBackup = MainPage.layerList[clickedLayer].imgBackup.Clone();

			//replace the moving img with the backupimage
			Image img = new Image();
			img.Source = MainPage.layerList[clickedLayer].imgBackup;
			img.Stretch = Stretch.None;

			int count = LayoutRoot.Children.IndexOf(MainPage.layerList[clickedLayer].img);
			LayoutRoot.Children.RemoveAt(count);
			LayoutRoot.Children.Insert(count, img);
			MainPage.layerList[clickedLayer].img = img;

			//displace the img to the original position
			img.Margin = new Thickness(MainPage.layerList[clickedLayer].imageBackupOffset.X, MainPage.layerList[clickedLayer].imageBackupOffset.Y, -MainPage.layerList[clickedLayer].imageBackupOffset.X, -MainPage.layerList[clickedLayer].imageBackupOffset.Y);
		}

		private static void calculateOffset(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			offSetLeft = e.GetPosition(inkCanvas).X - MainPage.layerList[clickedLayer].imageToBorderDist[LEFT];
			offSetRight = MainPage.layerList[clickedLayer].imageToBorderDist[RIGHT] - e.GetPosition(inkCanvas).X;
			offSetTop = e.GetPosition(inkCanvas).Y - MainPage.layerList[clickedLayer].imageToBorderDist[TOP];
			offSetBottom = MainPage.layerList[clickedLayer].imageToBorderDist[BOTTOM] - e.GetPosition(inkCanvas).Y;
		}

		
		public static void move(MouseEventArgs e, InkPresenter inkCanvas)
		{
			if (dragStarted == true && clickedLayer != -1 && !inkClicked)
			{
				moveLayer(e, inkCanvas);
			}
			else if (dragStarted == true && inkClicked)
			{
				moveInk(e, inkCanvas);
			}
		}

		private static void moveInk(MouseEventArgs e, InkPresenter inkCanvas)
		{
			deltaX = e.GetPosition(inkCanvas).X - initialPos.X;
			deltaY = e.GetPosition(inkCanvas).Y - initialPos.Y;

			ipToMove.Margin = new Thickness(ipToMove.Margin.Left + deltaX, ipToMove.Margin.Top + deltaY, ipToMove.Margin.Right + deltaX, ipToMove.Margin.Bottom + deltaY);

			initialPos = e.GetPosition(inkCanvas);
		}

		private static void moveLayer(MouseEventArgs e, InkPresenter inkCanvas)
		{
			deltaX = e.GetPosition(inkCanvas).X - initialPos.X;
			deltaY = e.GetPosition(inkCanvas).Y - initialPos.Y;

			Image img = MainPage.layerList[clickedLayer].img;
			img.Margin = new Thickness(img.Margin.Left + deltaX, img.Margin.Top + deltaY, img.Margin.Right - deltaX, img.Margin.Bottom - deltaY);

			initialPos = e.GetPosition(inkCanvas);
		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot)
		{
			dragStarted = false;

			if (clickedLayer != -1 && !inkClicked)
			{
				WriteableBitmap tempWb = new WriteableBitmap((BitmapSource)MainPage.layerList[clickedLayer].img.Source);			
				Image img = MainPage.layerList[clickedLayer].img;

				//redraw
				redrawMovedImage(tempWb, img);
	
				if (outOfBound)
				{
					backupImage();
				}

				//find the new img to border dist
				Common.findNewImageToBorderDist(img, e, inkCanvas, clickedLayer, offSetLeft, offSetRight, offSetTop, offSetBottom, MainPage.layerList[clickedLayer].imageToBorderDist[LEFT], MainPage.layerList[clickedLayer].imageToBorderDist[RIGHT], MainPage.layerList[clickedLayer].imageToBorderDist[TOP], MainPage.layerList[clickedLayer].imageToBorderDist[BOTTOM]);
				//Common.calculateMax(img, e, inkCanvas, clickedLayer);
				Common.checkIfOutOfBound(e, inkCanvas, img, clickedLayer, imageBackup, offSetLeft, offSetRight, offSetTop, offSetBottom, MainPage.layerList[clickedLayer].imageToBorderDist[LEFT], MainPage.layerList[clickedLayer].imageToBorderDist[RIGHT], MainPage.layerList[clickedLayer].imageToBorderDist[TOP], MainPage.layerList[clickedLayer].imageToBorderDist[BOTTOM]);
				MainPage.layerList[clickedLayer].img.Margin = new Thickness();
			}
			else if (inkClicked)
			{

				for (int i = 0; i < ipToMove.Strokes.Count; i++)
				{
					StylusPointCollection spcTemp = new StylusPointCollection();
					Stroke newStroke = new Stroke();

					placeInkBackToOriginalInkPresenter(inkCanvas, i, spcTemp, newStroke);
					
				}
					
				LayoutRoot.Children.Remove(ipToMove);

				inkClicked = false;
				
			}

			//if (clickedLayer != -1)
			//{
			//	MainPage.layerList[clickedLayer].img.Margin = new Thickness();

			//	//calculate img to border dist
			//	Image img = new Image();
			//	img.Source = imageBackup;
			//	Common.findNewImageToBorderDist(img, e, inkCanvas, clickedLayer, offSetLeft, offSetRight, offSetTop, offSetBottom, MainPage.layerList[clickedLayer].imageToBorderDist[LEFT], MainPage.layerList[clickedLayer].imageToBorderDist[RIGHT], MainPage.layerList[clickedLayer].imageToBorderDist[TOP], MainPage.layerList[clickedLayer].imageToBorderDist[BOTTOM]);
			//	//Common.calculateMax(img, e, inkCanvas, clickedLayer);
			//}
			

		}

		private static void placeInkBackToOriginalInkPresenter(InkPresenter inkCanvas, int i, StylusPointCollection spcTemp, Stroke newStroke)
		{
			for (int j = 0; j < ipToMove.Strokes[i].StylusPoints.Count; j++)
			{
				StylusPoint spTemp = new StylusPoint(ipToMove.Strokes[i].StylusPoints[j].X + ipToMove.Margin.Left, ipToMove.Strokes[i].StylusPoints[j].Y + ipToMove.Margin.Top);
				spcTemp.Add(spTemp);
			}

			newStroke.DrawingAttributes.Color = colorTemp;
			newStroke.DrawingAttributes.Height = 5;
			newStroke.DrawingAttributes.Width = 5;

			newStroke.StylusPoints.Add(spcTemp);
			inkCanvas.Strokes.Insert(layerIndex, newStroke);
		}

		private static void backupImage()
		{
			outOfBound = false;

			//link back to wbList
			MainPage.layerList[clickedLayer].img.Source = MainPage.layerList[clickedLayer].wb;

			//clear backupList
			MainPage.layerList[clickedLayer].imgBackup = null;
		}

		private static void redrawMovedImage(WriteableBitmap tempWb, Image img)
		{
			int w = MainPage.layerList[clickedLayer].wb.PixelWidth;
			int h = MainPage.layerList[clickedLayer].wb.PixelHeight;

			MainPage.layerList[clickedLayer].wb.Clear();
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
							MainPage.layerList[clickedLayer].wb.SetPixel(setX, setY, tempPixel);
						}

					}
				}
			}
		}

		


	}
}
