using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Graphics.Rendering;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace final_real_real_rocnikovka2.Algorithms
{
    public class MergeSort : SortingAlgorithm
    {
        private static readonly MergeSort instance = new();
        public static new MergeSort Instance => instance;

        private MergeSort()
        {
            Name = AlgorithmName.MERGE_SORT_NAME;
        }

        public override void Reset(List<int> numbers, List<Box> boxes)
        {
            Numbers = numbers;
            Boxes = boxes;
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

            await MergeSortRecursion(0, n - 1);

            if (Globals.Stop) return;

            if (IsSorted())
                Draw.DrawDone(Boxes, ColorPalette.SelectedBarFill);
            else 
                Draw.DrawDone(Boxes, ColorPalette.DefaultBarFill);
        }

        private async Task MergeSortRecursion(int left, int right)
        {
            if (Globals.Stop) return;
            if (left >= right) return;

            int mid = left + (right - left) / 2;

            await MergeSortRecursion(left, mid);
            await MergeSortRecursion(mid + 1, right);

            await Merge(left, mid, right);
        }
        private async Task Merge(int left, int mid, int right)
        {
            int firstRight = mid + 1;

            if (Numbers[mid] <= Numbers[firstRight]) return;

            while (left <= mid && firstRight <= right)
            {
                if (Globals.Stop) return;
                if (Numbers[left] <= Numbers[firstRight])
                {
                    left++;
                } else
                {
                    int index = firstRight;

                    
                    while (index > left) // Takhle komplkovane to delam jen kvuli vykresu, jinak by to bylo lehci
                    {
                        if (Globals.Stop) return;
                        SwapInList(Numbers, index, index - 1);
                        SwapInList(Boxes, index, index - 1);
                        Draw.SwapXPos(Boxes[index], Boxes[index - 1]);

                        index--;
                    }

                    Boxes[firstRight].ChangeColor(ColorPalette.SelectedBarFill);
                    Boxes[index].ChangeColor(ColorPalette.SelectedBarFill);

                    await Wait(Globals.AnimationMs, index);

                    Boxes[firstRight].ChangeColor(ColorPalette.DefaultBarFill);
                    Boxes[index].ChangeColor(ColorPalette.DefaultBarFill);
                    left++;
                    mid++;
                    firstRight++;
                }

            }

        }

        public override void Step()
        {
            throw new NotImplementedException();
        }
    }
}
