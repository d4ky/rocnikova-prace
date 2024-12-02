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
    public class HeapSort : SortingAlgorithm
    {
        private static readonly HeapSort instance = new();
        public static new SortingAlgorithm Instance => instance;

        private HeapSort()
        {
            Name = AlgorithmName.HEAP_SORT_NAME;
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

            for (int i = n/2 - 1; i >= 0; i--)
            {
                if (Globals.Stop) return;
                await Heapify(n, i);
            }

            for (int i = n -1; i >= 1; i--)
            {
                if (Globals.Stop) return;
                SwapInList(Numbers, 0, i);
                SwapInList(Boxes, 0, i);

                Draw.SwapXPos(Boxes[i], Boxes[0]);
                Boxes[0].ChangeColor(ColorPalette.DefaultBarFill);
                Boxes[i].ChangeColor(ColorPalette.SelectedBarFill);

                await Wait(Globals.AnimationMs, i);

                await Heapify(i, 0);
            }
            Boxes[0].ChangeColor(ColorPalette.SelectedBarFill);
        }

        private async Task Heapify(int n, int i)
        {
            if (Globals.Stop) return;
            int largest = i;
            int leftChild = 2 * i + 1;
            int rightChild = 2 * i + 2;

            if (leftChild < n && Numbers[leftChild] > Numbers[largest])
            {
                largest = leftChild;
            }

            if (rightChild < n && Numbers[rightChild] > Numbers[largest])
            {
                largest = rightChild;
            }

            if (largest != i)
            {
                SwapInList(Numbers, i, largest);
                SwapInList(Boxes, i, largest);

                Draw.SwapXPos(Boxes[i], Boxes[largest]);
                Boxes[largest].ChangeColor(ColorPalette.SelectedBarFill);
                Boxes[i].ChangeColor(ColorPalette.SelectedBarFill);

                await Wait(Globals.AnimationMs, i);

                Boxes[largest].ChangeColor(ColorPalette.DefaultBarFill);
                Boxes[i].ChangeColor(ColorPalette.DefaultBarFill);

                await Heapify(n, largest);
            }
        }

        public override void Step()
        {
            throw new NotImplementedException();
        }
    }
}
