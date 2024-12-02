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
    public class HeapSort
    {
        public List<int> _numbers = new List<int>();

        public async Task Sort(List<int> numbers, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            _numbers = numbers;
            int n = _numbers.Count;

            await BuildMaxHeap(canvas, getSliderValue, cancel, superSpeed);

            for (int i = n - 1; i >= 0; i--)
            {

                Swap(0, i);
                Draw(canvas, 0, i);

                if (superSpeed() == false)
                {
                    await Task.Delay((int)getSliderValue());
                } else
                {
                    await Task.Delay(1);
                }

                await Heapify(0, i, canvas, getSliderValue, cancel, superSpeed);
                if (cancel()) return;
            }
            Draw(canvas, -1, -1);
            await DoneDraw(canvas, getSliderValue);
        }
        private async Task BuildMaxHeap(Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            int n = _numbers.Count;

            for (int i = n / 2 - 1; i >= 0; i--)
            {
                await Heapify(i, n, canvas, getSliderValue, cancel, superSpeed);
                if (cancel()) return;
            }
        }
        private async Task Heapify(int root, int heapSize, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {
            if (cancel()) return;
            int largest = root;
            int left = 2 * root + 1;
            int right = 2 * root + 2;

            if (left < heapSize && _numbers[left] > _numbers[largest])
            {
                largest = left;
            }

            if (right < heapSize && _numbers[right] > _numbers[largest])
            {
                largest = right;
            }

            if (largest != root)
            {
                Swap(root, largest);
                Draw(canvas, root, largest);

                if (superSpeed() == false)
                {
                    await Task.Delay((int)getSliderValue());
                } else if (root % 20 == 0)
                {
                    await Task.Delay(1);
                }

                await Heapify(largest, heapSize, canvas, getSliderValue, cancel, superSpeed);
            }
        }
        private void Swap(int a, int b)
        {
            int temp = _numbers[a];
            _numbers[a] = _numbers[b];
            _numbers[b] = temp;
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

        private void Draw(Canvas canvas, int hightlightIndexA, int highlightIndexB)
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

                Rectangle rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = (hightlightIndexA == i || highlightIndexB == i) ? Brushes.Green : Brushes.White
                };

                double x = (i + 1) * spacing + i * rectWidth;
                double y = canvasHeight - rectHeight;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);

                canvas.Children.Add(rect);
            }
        }
    }
}
