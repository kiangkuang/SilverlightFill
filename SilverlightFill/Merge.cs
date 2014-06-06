using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightFill
{
	public class Merge
	{
		public static void down(MouseButtonEventArgs e)
		{

		}

		public static void move(MouseEventArgs e)
		{

		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas, Color selectedColor)
		{
			int clickedLayer = Common.hitTestLayer(e, inkCanvas);
			if (clickedLayer == -1)
			{
				return;
			}

			WriteableBitmap wb1 = Common.convertToBitmap(inkCanvas);
			Color targetColor = Common.getTargetColor(e, inkCanvas, wb1);

			Fill.floodFill(new Point((int)e.GetPosition(inkCanvas).X, (int)e.GetPosition(inkCanvas).Y), targetColor, selectedColor, wb1, MainPage.wbList[clickedLayer]);
			
		}
	}
}
