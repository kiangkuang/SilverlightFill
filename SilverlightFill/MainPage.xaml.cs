using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SilverlightFill
{
	public partial class MainPage : UserControl
	{
		public static Color selectedColor = Colors.Black;
		public static Color targetColor;
		public static List<InkPresenter> presenterList = new List<InkPresenter>();

		private int mode;
		private const int INKMODE = 0;
		private const int FILLMODE = 1;
		private const int DRAGMODE = 2;
		private const int MERGEMODE = 3;
		private const int SUBTRACTMODE = 4;

		public MainPage()
		{
			InitializeComponent();
		}

		private void buttonRed(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Red);
			selectedColor = Colors.Red;
		}
		private void buttonOrange(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Orange);
			selectedColor = Colors.Orange;
		}
		private void buttonYellow(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Yellow);
			selectedColor = Colors.Yellow;
		}
		private void buttonGreen(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Green);
			selectedColor = Colors.Green;
		}
		private void buttonBlue(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Blue);
			selectedColor = Colors.Blue;
		}
		private void buttonMagenta(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Magenta);
			selectedColor = Colors.Magenta;
		}
		private void buttonPurple(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Purple);
			selectedColor = Colors.Purple;
		}
		private void buttonBlack(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.Black);
			selectedColor = Colors.Black;
		}
		private void buttonWhite(object sender, RoutedEventArgs e)
		{
			replaceBox.Fill = new SolidColorBrush(Colors.White);
			selectedColor = Colors.White;
		}
		private void clear(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < presenterList.Count; i++)
			{
				LayoutRoot.Children.Remove(presenterList[i]);
			}
			inkCanvas.Strokes.Clear();
			presenterList.Clear();

			Common.convertToBitmap(inkCanvas);
			strokeCounter.Content = "Fill Layers: " + presenterList.Count;
		}
		private void ink(object sender, RoutedEventArgs e)
		{
			inkButton.FontWeight = FontWeights.Bold;
			subtractButton.FontWeight = mergeButton.FontWeight = fillButton.FontWeight = dragButton.FontWeight = FontWeights.Normal;
			mode = INKMODE;
		}
		private void fill(object sender, RoutedEventArgs e)
		{
			fillButton.FontWeight = FontWeights.Bold;
			subtractButton.FontWeight = mergeButton.FontWeight = inkButton.FontWeight = dragButton.FontWeight = FontWeights.Normal;
			mode = FILLMODE;
		}
		private void drag(object sender, RoutedEventArgs e)
		{
			dragButton.FontWeight = FontWeights.Bold;
			subtractButton.FontWeight = mergeButton.FontWeight = inkButton.FontWeight = fillButton.FontWeight = FontWeights.Normal;
			mode = DRAGMODE;
		}
		private void merge(object sender, RoutedEventArgs e)
		{
			mergeButton.FontWeight = FontWeights.Bold;
			subtractButton.FontWeight = dragButton.FontWeight = inkButton.FontWeight = fillButton.FontWeight = FontWeights.Normal;
			mode = MERGEMODE;
		}
		private void subtract(object sender, RoutedEventArgs e)
		{
			subtractButton.FontWeight = FontWeights.Bold;
			mergeButton.FontWeight = dragButton.FontWeight = inkButton.FontWeight = fillButton.FontWeight = FontWeights.Normal;
			mode = SUBTRACTMODE;
		}

		private void inkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			switch (mode)
			{
				case INKMODE:
					Ink.down(e, inkCanvas);
					break;
				case DRAGMODE:
					Drag.down(e, inkCanvas);
					break;
			}
		}

		private void inkCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			switch (mode)
			{
				case INKMODE:
					Ink.move(e, inkCanvas);
					break;
				case DRAGMODE:
					Drag.move(e, inkCanvas);
					break;
			}
		}

		private void inkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			switch (mode)
			{
				case INKMODE:
					Ink.up(e, inkCanvas);
					break;
				case FILLMODE:
					Fill.up(e, inkCanvas, LayoutRoot, selectedColor);
					break;
				case DRAGMODE:
					Drag.up(e);
					break;
				case MERGEMODE:
					Merge.up(e, inkCanvas, selectedColor);
					break;
				case SUBTRACTMODE:
					Subtract.up(e, inkCanvas, LayoutRoot, selectedColor);
					break;
			}
			strokeCounter.Content = "Fill Layers: " + presenterList.Count;
		}
	}
}
