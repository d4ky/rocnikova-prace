using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace rea_real_rocnikovka2
{
    public class QuickSort
    {
        public List<int> _numbers = new List<int>();

        public async Task Sort(List<int> numbers, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            _numbers = numbers;
            int n = _numbers.Count;

            await QuickSortRecursive(0, n - 1, canvas, getSliderValue, cancel, superSpeed);

            if (cancel()) return;
            Draw(canvas, -1, -1, -1);
            await DoneDraw(canvas, getSliderValue);
        }

        private async Task QuickSortRecursive(int low, int high, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            if (cancel()) return;
            if (low < high)
            {
                int pivotIndex = await PartitionWithMedianOfThree(low, high, canvas, getSliderValue, cancel, superSpeed);

                await QuickSortRecursive(low, pivotIndex - 1, canvas, getSliderValue, cancel, superSpeed);
                await QuickSortRecursive(pivotIndex + 1, high, canvas, getSliderValue, cancel, superSpeed);
            }
        }

        private async Task<int> PartitionWithMedianOfThree(int low, int high, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            int mid = low + (high - low) / 2;

            int pivotIndex = MedianOfThree(low, mid, high);
            int pivot = _numbers[pivotIndex];

            Swap(pivotIndex, high);

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (cancel()) return -1;

                if (_numbers[j] < pivot)
                {
                    i++;
                    Swap(i, j);

                    Draw(canvas, i, j, high);
                    if (superSpeed() == false)
                    {
                        await Task.Delay((int)getSliderValue());
                    }
                }
            }
            Swap(i + 1, high);

            Draw(canvas, i + 1, high, high);
            if (superSpeed() == false)
            {
                await Task.Delay((int)getSliderValue());
            }

            return i + 1;
        }

        private int MedianOfThree(int low, int mid, int high)
        {
            int a = _numbers[low];
            int b = _numbers[mid];
            int c = _numbers[high];

            if ((a > b) != (a > c))
            {
                return low;
            }
            else if ((b > a) != (b > c))
            {
                return mid;
            }
            else
            {
                return high;
            }
        }

        private void Swap(int a, int b)
        {
            int temp = _numbers[a];
            _numbers[a] = _numbers[b];
            _numbers[b] = temp;
        }

        private void Draw(Canvas canvas, int index1, int index2, int pivotIndex)
        {
            canvas.Children.Clear();

            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;

            double maxNumber = _numbers.Max();

            double totalSpaces = _numbers.Count + 1;
            double spacing = canvasWidth / (totalSpaces + _numbers.Count);
            double rectWidth = spacing;
            double rectMaxHeight = canvasHeight;

            for (int i = 0; i < _numbers.Count; i++)
            {
                double rectHeight = (_numbers[i] / maxNumber) * rectMaxHeight;

                Brush color = Brushes.White;

                if (i == pivotIndex)
                {
                    color = Brushes.Red; //piv
                }
                else if (i == index1 || i == index2)
                {
                    color = Brushes.Blue;
                }

                Rectangle rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = color
                };

                double x = (i + 1) * spacing + i * rectWidth;
                double y = canvasHeight - rectHeight;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);

                canvas.Children.Add(rect);
            }
        }

        private async Task DoneDraw(Canvas canvas, Func<double> getSliderValue)
        {
            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;

            double maxNumber = _numbers.Max();

            double totalSpaces = _numbers.Count + 1;
            double spacing = canvasWidth / (totalSpaces + _numbers.Count);
            double rectWidth = spacing;
            double rectMaxHeight = canvasHeight;

            for (int i = 0; i < _numbers.Count; i++)
            {
                double rectHeight = (_numbers[i] / maxNumber) * rectMaxHeight;

                Rectangle rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = Brushes.Green
                };

                double x = (i + 1) * spacing + i * rectWidth;
                double y = canvasHeight - rectHeight;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);

                await Task.Delay((int)getSliderValue());

                canvas.Children.Add(rect);
            }
        }
    }

}

