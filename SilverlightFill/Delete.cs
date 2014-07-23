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

			checkIfStrokeSelected(e, inkCanvas, spc);

			checkIfLayerSelected(LayoutRoot, clickedLayer);
		}

		private static void checkIfLayerSelected(Grid LayoutRoot, int clickedLayer)
		{
			if (clickedLayer == -1)
			{
				return;
			}

			removeSelectedLayer(LayoutRoot, clickedLayer);
		}

		private static void checkIfStrokeSelected(MouseButtonEventArgs e, InkPresenter inkCanvas, StylusPointCollection spc)
		{
			spc.Add(new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y));

			if (inkCanvas.Strokes.HitTest(spc).Count > 0)
			{
				removeSelectedStroke(inkCanvas, spc);
			}

			return;
		}

		private static void removeSelectedStroke(InkPresenter inkCanvas, StylusPointCollection spc)
		{
			inkCanvas.Strokes.Remove(inkCanvas.Strokes.HitTest(spc)[inkCanvas.Strokes.HitTest(spc).Count - 1]);
		}

		private static void removeSelectedLayer(Grid LayoutRoot, int clickedLayer)
		{
			LayoutRoot.Children.Remove(MainPage.layerList[clickedLayer].img);
			MainPage.layerList.RemoveAt(clickedLayer);
		}
	}
}
