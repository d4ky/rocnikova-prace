using final_real_real_rocnikovka2.Algorithms;
using final_real_real_rocnikovka2.Graphics.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace final_real_real_rocnikovka2.Graphics.Rendering
{
    public static class Draw
    {
        private static double _ballRadius;

        public static double BallRadius
        {
            get => _ballRadius;
            private set => _ballRadius = value;
        }

        public static void UpdateBallRadius(SortingAlgorithm? sortingAlgorithm, int n, Canvas canvas)
        {
            if (sortingAlgorithm == null || sortingAlgorithm is BubbleSort || sortingAlgorithm is SelectionSort || sortingAlgorithm is InsertionSort)
            {
                BallRadius = Math.Min(canvas.ActualWidth / (3 * n + 1), canvas.ActualHeight / 6);
            }
        }

        public static void SwapXPos(GraphicElement gE1, GraphicElement gE2)
        {
            double tempX = gE1.X;

            gE1.SetPosition(gE2.X, gE1.Y);
            gE2.SetPosition(tempX, gE2.Y);
        }

        public static async void DrawDone(IEnumerable<GraphicElement> listGE, Color color)
        {
            foreach (GraphicElement gE in listGE)
            {
                gE.ChangeColor(color);
                await Task.Delay(1);
            }
        }
        public static async void ChangeColorForAll(IEnumerable<Ball> listGE, Color fillColor, Color strokeColor)
        {
            foreach (Ball gE in listGE)
            {
                gE.ChangeColor(fillColor);
                gE.ChangeStrokeColor(strokeColor);
                await Task.Delay(1);
            }
        }

        public static void ChangeColorForAll(IEnumerable<GraphicElement> listGE, Color color)
        {
            foreach (GraphicElement gE in listGE)
            {
                gE.ChangeColor(color);
            }
        }



    }
}
