using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace rea_real_rocnikovka2
{
    public class BubbleSort
    {
        public List<int> _numbers = new List<int>();

        public async Task Sort(List<int> numbers, Canvas canvas, Func<double> getSliderValue, Func<bool> cancel, Func<bool?> superSpeed)
        {

            _numbers = numbers;
            int n = _numbers.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (cancel())
                    {
                        return;
                    }
                    if (_numbers[j] > _numbers[j + 1])
                    {
                        int temp = _numbers[j];
                        _numbers[j] = _numbers[j + 1];
                        _numbers[j + 1] = temp;
                        
                        Draw(canvas, j, j + 1);

                        if (superSpeed() == false)
                        {
                            await Task.Delay((int)getSliderValue());
                        }
                        else if (j % (n/20) == 0) 
                        {
                            await Task.Delay(1);    
                        }
                    }
                }
            }

            Draw(canvas, -1, -1);
            await DoneDraw(canvas, getSliderValue);
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
                double rectHeight = (_numbers[i]/maxNumber) * rectMaxHeight;

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
