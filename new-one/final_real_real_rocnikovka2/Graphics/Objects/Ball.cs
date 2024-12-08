using final_real_real_rocnikovka2.Graphics.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace final_real_real_rocnikovka2.Graphics.Objects
{
    public class Ball : GraphicElement
    {
        public Text? BallText { get; set; }

        public double BallRadiusRatio;
        public double Width
        {
            get => ((Ellipse)MainUIElement).Width;
            set => ((Ellipse)MainUIElement).Width = value;
        }

        public double Height
        {
            get => ((Ellipse)MainUIElement).Height;
            set => ((Ellipse)MainUIElement).Height = value;
        }

        public double StrokeThickness
        {
            get => ((Ellipse)MainUIElement).StrokeThickness;
            set => ((Ellipse)MainUIElement).StrokeThickness = value;
        }
        public Ball(Canvas canvas, double opacity, Color color, Color stroke, double ballRadiusRatio) : base(canvas)
        {
            this.MainUIElement = CreateEllipse(opacity, color, stroke, ballRadiusRatio);
            BallRadiusRatio = ballRadiusRatio;
        }

        private static Ellipse CreateEllipse(Double opacity, Color color, Color stroke, double ballRadiusRatio)
        {
            return new Ellipse
            {
                Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(stroke),
                Width = ballRadiusRatio * Draw.BallRadius * 2,
                Height = ballRadiusRatio * Draw.BallRadius * 2,
                Opacity = opacity,
                StrokeThickness = 0.1 * ballRadiusRatio * Draw.BallRadius

            };
        }

        public override void AddToCanvas()
        {
            MainCanvas.Children.Add(MainUIElement);
            
            BallText?.AddToCanvas();
        }

        public override void SetPosition(double x, double y)
        {
            Canvas.SetLeft(this.MainUIElement, (double)x);
            Canvas.SetTop(MainUIElement, y);
            this.StrokeThickness = 0.1 * Draw.BallRadius * BallRadiusRatio;

            this.Width = 2 * Draw.BallRadius * BallRadiusRatio;
            this.Height = 2 * Draw.BallRadius * BallRadiusRatio;
            BallText?.Measure();
            BallText?.SetPosition(x + Draw.BallRadius - BallText.TextWidth / 2, y + Draw.BallRadius - BallText.TextHeight / 2);
        }

        public override void Update(double prevWidth, double prevHeight)
        {
            return;
        }
        public override void Update(double prevWidth, double prevHeight, double prevBallRadius)
        {
            return;
            //BallText?.Update(prevWidth, prevHeight, prevBallRadius); // tohle jen scaluje font
            //double widthScale = MainCanvas.ActualWidth / prevWidth;
            //double heightScale = MainCanvas.ActualHeight / prevHeight;

            //this.StrokeThickness = 0.1 * Draw.BallRadius;

            //this.Width = 2 * Draw.BallRadius;
            //this.Height = 2 * Draw.BallRadius;
            //SetPosition(X * widthScale, (Y + prevBallRadius) * heightScale - Draw.BallRadius); // nastavi pozici obojiho
        }

        public override void Delete()
        {
            MainCanvas.Children.Remove(MainUIElement);

            BallText?.Delete();
        }

        public override void ChangeColor(Color color)
        {
            ((Ellipse)MainUIElement).Fill = new SolidColorBrush(color);
        }
        public void SetStrokeColor(Color color)
        {
            ((Ellipse)MainUIElement).Stroke = new SolidColorBrush(color);
        }

        public Color GetStrokeColor()
        {
            return ((SolidColorBrush)((Ellipse)MainUIElement).Stroke).Color;
        }
        public Color GetFillColor()
        {
            return ((SolidColorBrush)((Ellipse)MainUIElement).Fill).Color;
        }
    }
}
