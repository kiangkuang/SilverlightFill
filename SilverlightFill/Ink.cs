using System;
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
			setDrawingAttributes();
			newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkCanvas));
			inkCanvas.Strokes.Add(newStroke);
		}

		private static void setDrawingAttributes()
		{
			newStroke = new Stroke();
			newStroke.DrawingAttributes.Color = MainPage.selectedColor;
			newStroke.DrawingAttributes.Height = 5;
			newStroke.DrawingAttributes.Width = 5;
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

		public static StylusPoint BezierPoint(StylusPoint A, StylusPoint B, StylusPoint C, StylusPoint D, StylusPoint E, double t)
		{
			StylusPoint P = new StylusPoint(Math.Pow(1 - t, 4) * A.X + 4 * Math.Pow(1 - t, 3) * B.X + 6 * Math.Pow(1 - t, 2) * Math.Pow(t, 2) * C.X + 4 * (1 - t) * Math.Pow(t, 3) * D.X + Math.Pow(t, 4) * E.X,
											Math.Pow(1 - t, 4) * A.Y + 4 * Math.Pow(1 - t, 3) * B.Y + 6 * Math.Pow(1 - t, 2) * Math.Pow(t, 2) * C.Y + 4 * (1 - t) * Math.Pow(t, 3) * D.Y + Math.Pow(t, 4) * E.Y);
			return P;
		}
		public static StylusPoint BezierPoint(StylusPoint A, StylusPoint B, StylusPoint C, StylusPoint D, double t)
		{
			StylusPoint P = new StylusPoint(Math.Pow((1 - t), 3) * A.X + 3 * t * Math.Pow((1 - t), 2) * B.X + 3 * (1 - t) * Math.Pow(t, 2) * C.X + Math.Pow(t, 3) * D.X,
											Math.Pow((1 - t), 3) * A.X + 3 * t * Math.Pow((1 - t), 2) * B.X + 3 * (1 - t) * Math.Pow(t, 2) * C.X + Math.Pow(t, 3) * D.X);
			return P;
		}
		public static StylusPoint BezierPoint(StylusPoint A, StylusPoint B, StylusPoint C, double t)
		{
			StylusPoint P = new StylusPoint(Math.Pow((1 - t), 2) * A.X + 2 * t * (1 - t) * B.X + Math.Pow(t, 2) * C.X,
											Math.Pow((1 - t), 2) * A.Y + 2 * t * (1 - t) * B.Y + Math.Pow(t, 2) * C.Y);
			return P;
		}

		public static Stroke TransformBezier(Stroke original)
		{
			Stroke result = new Stroke();

			switch ((original.StylusPoints.Count - 1) % 3)
			{
				case 0:
					for (int i = 0; i < (original.StylusPoints.Count - 1) / 3; i++)
					{
						for (double t = 0.0; t <= 1.0; t += 1.0 / 32)
						{
							result.StylusPoints.Add(BezierPoint(original.StylusPoints[i * 3 + 0],
																original.StylusPoints[i * 3 + 1],
																original.StylusPoints[i * 3 + 2],
																original.StylusPoints[i * 3 + 3], t));
						}
					}
					break;
				case 1:
					for (int i = 0; i < ((original.StylusPoints.Count - 1) / 3) - 1; i++)
					{
						for (double t = 0.0; t <= 1.0; t += 1.0 / 32)
						{
							result.StylusPoints.Add(BezierPoint(original.StylusPoints[i * 3 + 0],
																original.StylusPoints[i * 3 + 1],
																original.StylusPoints[i * 3 + 2],
																original.StylusPoints[i * 3 + 3], t));
						}
					}
					for (double t = 0.0; t <= 1.0; t += 1.0 / 32)
					{
						result.StylusPoints.Add(BezierPoint(original.StylusPoints[original.StylusPoints.Count - 5],
															original.StylusPoints[original.StylusPoints.Count - 4],
															original.StylusPoints[original.StylusPoints.Count - 3],
															original.StylusPoints[original.StylusPoints.Count - 2],
															original.StylusPoints[original.StylusPoints.Count - 1], t));
					}
					break;
				case 2:
					for (int i = 0; i < (original.StylusPoints.Count - 1) / 3; i++)
					{
						for (double t = 0.0; t <= 1.0; t += 1.0 / 32)
						{
							result.StylusPoints.Add(BezierPoint(original.StylusPoints[i * 3 + 0],
																original.StylusPoints[i * 3 + 1],
																original.StylusPoints[i * 3 + 2],
																original.StylusPoints[i * 3 + 3], t));
						}
					}

					for (double t = 0.0; t <= 1.0; t += 1.0 / 32)
					{
						result.StylusPoints.Add(BezierPoint(original.StylusPoints[original.StylusPoints.Count - 3],
															original.StylusPoints[original.StylusPoints.Count - 2],
															original.StylusPoints[original.StylusPoints.Count - 1], t));
					}
					break;
			}

			return result;
		}
	}
}
