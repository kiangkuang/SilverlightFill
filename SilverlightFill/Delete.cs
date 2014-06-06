using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace SilverlightFill
{
	class Delete
	{
		public static void down(MouseButtonEventArgs e)
		{
			
		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Grid LayoutRoot)
		{
			int clickedLayer = Common.hitTestLayer(e, inkCanvas);
			
			StylusPointCollection spc = new StylusPointCollection();
			
			spc.Add(new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y));
			if (inkCanvas.Strokes.HitTest(spc).Count > 0)
			{
				for (int i = 0; i < inkCanvas.Strokes.HitTest(spc).Count; i++)
				{
					inkCanvas.Strokes.Remove(inkCanvas.Strokes.HitTest(spc)[i]);
				}

				return;
			}

			if (clickedLayer == -1)
			{
				return;
			}

			MainPage.wbList.RemoveAt(clickedLayer);
			LayoutRoot.Children.Remove(MainPage.imageList[clickedLayer]);
			MainPage.imageList.RemoveAt(clickedLayer);
		}
	}
}
