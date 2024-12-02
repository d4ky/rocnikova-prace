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
    public class MergeSort
    {
        public List<int> _numbers = new List<int>();

        public async Task Sort(List<int> numbers, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {

            _numbers = numbers;
            int n = _numbers.Count;

            await MergeSortRecursive(0, n - 1, canvas, getSliderValue, cancel, superSpeed);
            if (cancel()) return;

            Draw(canvas, -1, -1, -1);
            await DoneDraw(canvas, getSliderValue);
        }

        private async Task MergeSortRecursive(int left, int right, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            if (cancel()) return;
            if (left < right)
            {
                int middle = left + (right - left) / 2;

                await MergeSortRecursive(left, middle, canvas, getSliderValue, cancel, superSpeed);
                await MergeSortRecursive(middle + 1, right, canvas, getSliderValue, cancel, superSpeed);

                await Merge(left, middle, right, canvas, getSliderValue, cancel, superSpeed);
            }
        }

        private async Task Merge(int left, int middle, int right, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            if (cancel()) return;
            int n1 = middle - left + 1;
            int n2 = right - middle;

            // Create temporary arrays
            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];

            for (int i = 0; i < n1; i++)
            {
                leftArray[i] = _numbers[left + i];
            }

            for (int j = 0; j < n2; j++)
            {
                rightArray[j] = _numbers[middle + 1 + j];
            }

            int iIndex = 0, jIndex = 0;
            int kIndex = left;

            while (iIndex < n1 && jIndex < n2)
            {
                if (cancel()) return;

                if (leftArray[iIndex] <= rightArray[jIndex])
                {
                    _numbers[kIndex] = leftArray[iIndex];
                    iIndex++;
                }
                else
                {
                    _numbers[kIndex] = rightArray[jIndex];
                    jIndex++;
                }

                Draw(canvas, kIndex, left + iIndex, middle + 1 + jIndex);
                kIndex++;

                if (superSpeed() == false)
                {
                    await Task.Delay((int)getSliderValue());
                }
            }

            while (iIndex < n1)
            {
                if (cancel())
                {
                    return;
                }

                _numbers[kIndex] = leftArray[iIndex];
                iIndex++;
                kIndex++;

                Draw(canvas, kIndex, left + iIndex, -1);

                if (superSpeed() == false)
                {
                    await Task.Delay((int)getSliderValue());
                }
            }

            while (jIndex < n2)
            {
                if (cancel())
                {
                    return;
                }

                _numbers[kIndex] = rightArray[jIndex];
                jIndex++;
                kIndex++;

                Draw(canvas, kIndex, -1, middle + 1 + jIndex);

                if (superSpeed() == false)
                {
                    await Task.Delay((int)getSliderValue());
                }
            }
        }
        private void Draw(Canvas canvas, int highlightIndex, int highlightLeft, int highlightRight)
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
                if (i == highlightIndex)
                {
                    color = Brushes.Red;
                }
                else if (i == highlightLeft)
                {
                    color = Brushes.Blue;
                }
                else if (i == highlightRight)
                {
                    color = Brushes.Green;
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
