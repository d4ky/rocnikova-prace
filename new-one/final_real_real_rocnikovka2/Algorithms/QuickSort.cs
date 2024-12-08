using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Graphics.Rendering;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace final_real_real_rocnikovka2.Algorithms
{
    public class QuickSort : SortingAlgorithm
    {
        private static readonly QuickSort instance = new();
        public static new QuickSort Instance => instance;

        private QuickSort()
        {
            Name = AlgorithmName.QUICK_SORT_NAME;
        }

        public override void Reset(List<int> numbers, List<Box> boxes)
        {
            Numbers = numbers;
            Boxes = boxes;
            ComparisonCount = 0;
            SwapCount = 0;
        }
        public override void Reset(List<int> numbers, List<Ball> balls, List<GraphicElement> graphicElements)
        {
            Numbers = numbers;
            Balls = balls;
        }

        public override async Task Sort()
        {
            if (IsSorted()) return;

            int n = Numbers.Count;

            await QuickSortRecursion(0, n - 1);

            if (IsSorted())
                Draw.DrawDone(Boxes, ColorPalette.SelectedBarFill);
            else
                Draw.ChangeColorForAll(Boxes, ColorPalette.DefaultBarFill);
        }

        private async Task QuickSortRecursion(int left, int right)
        {
            if (Globals.Stop) return;
            if (left >= right) return;

            int partionIndex = await Partion(left, right);
            if (Globals.Stop) return;

            await QuickSortRecursion(left, partionIndex - 1);
            await QuickSortRecursion(partionIndex + 1, right);
     
        }
        private async Task<int> Partion(int left, int right)
        {
            int pivot = Numbers[right];
            int i = left - 1;

            Boxes[right].ChangeColor(ColorPalette.PivotBarFill);

            for (int j = left; j < right; j++)
            {
                if (Globals.Stop) return 0;
                ComparisonCount++;
                if (Numbers[j] < pivot)
                {
                    i++;
                    SwapCount++;
                    SwapInList(Numbers, i, j);
                    SwapInList(Boxes, i, j);
                    Draw.SwapXPos(Boxes[i], Boxes[j]);

                    Boxes[i].ChangeColor(ColorPalette.SelectedBarFill);
                    Boxes[j].ChangeColor(ColorPalette.SelectedBarFill);

                    await Wait(Globals.AnimationMs, j);

                    Boxes[i].ChangeColor(ColorPalette.DefaultBarFill);
                    Boxes[j].ChangeColor(ColorPalette.DefaultBarFill); 

                }
            }

            Boxes[right].ChangeColor(ColorPalette.DefaultBarFill);
            SwapCount++;
            SwapInList(Numbers, i + 1, right);
            SwapInList(Boxes, i + 1, right);
            Draw.SwapXPos(Boxes[i + 1], Boxes[right]);

            return i + 1;
        }

        public override void Step()
        {
            throw new NotImplementedException();
        }

        public override void OnSelect(List<int> numbers, List<Ball> balls)
        {
            double xPos = Draw.BallRadius;
            double yPos = balls[0].MainCanvas.ActualHeight / 2 - Draw.BallRadius;
            foreach (Ball ball in Balls)
            {
                ball.SetPosition(xPos, yPos);
                xPos += 3 * Draw.BallRadius;
            }
        }
    }
}
