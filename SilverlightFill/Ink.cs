using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace SilverlightFill
{
	public class Ink
	{
		private static Stroke newStroke = null;
		public static void down(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			inkCanvas.CaptureMouse();
			newStroke = new Stroke();
			newStroke.DrawingAttributes.Color = MainPage.selectedColor;
			newStroke.DrawingAttributes.Height = 5;
			newStroke.DrawingAttributes.Width = 5;

			newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
			inkCanvas.Strokes.Add(newStroke);
		}

		public static void move(MouseEventArgs e, InkPresenter inkCanvas)
		{
			if (newStroke != null)
			{
				newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
			}
		}

		public static void up(MouseButtonEventArgs e, InkPresenter inkCanvas)
		{
			newStroke = null;
			inkCanvas.ReleaseMouseCapture();
		}
	}
}
