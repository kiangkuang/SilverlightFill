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
	public class Layer
	{
		public Image img;
		public WriteableBitmap wb;
		public Point imageBackupOffset;
		public List<double> imageToBorderDist; // left, right, top, bottom
		public WriteableBitmap imgBackup;

		public Layer(Grid LayoutRoot, WriteableBitmap wb, double maxLeft, double maxRight, double maxTop, double maxBottom)
		{
			this.wb = wb;

			Image img = new Image();
			img.Source = wb;
			img.Stretch = Stretch.None;
			img.HorizontalAlignment = HorizontalAlignment.Left;
			this.img = img;
			LayoutRoot.Children.Add(img);

			this.imgBackup = null; // nothing

			this.imageBackupOffset = new Point(); // nothing

			this.imageToBorderDist = new List<double>();
			imageToBorderDist.Add(maxLeft);
			imageToBorderDist.Add(maxRight);
			imageToBorderDist.Add(maxTop);
			imageToBorderDist.Add(maxBottom);
		}
	}
}
