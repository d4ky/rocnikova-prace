using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rea_real_rocnikovka2
{
    /// <summary>
    /// Interaction logic for AlgorithmShowcase.xaml
    /// </summary>
    /// audiocha
    public partial class AlgorithmShowcase : Page
    {
        public ObservableCollection<string> AlgorithmOptions { get; set; }
        public string SelectedAlgorithm { get; set; }
        public List<int> numbers { get; set; }
        public bool scriptIsRunning;
        public bool arrIsSorted;
        private bool cancel;

        public AlgorithmShowcase()
        {
            InitializeComponent();

            AlgorithmOptions = new ObservableCollection<string>()
            {
                "Bubble Sort",
                "Insertion Sort",
                "Selection Sort",
                "Quick Sort",
                "Heap Sort",
                "Merge Sort"
            };

            scriptIsRunning = false;
            arrIsSorted = false;
            cancel = false;
            CancelButton.IsEnabled = false;

            numbers = new List<int>() { 15, 10, 13, 4, 9, 3, 6, 9, 8, 14, 21, 17, 15, 20, 25, 31, 22 };

            DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawBars(BarCanvas);
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (!arrIsSorted && !scriptIsRunning)
            {
                NumberInput.IsEnabled = false;
                CancelButton.IsEnabled = true;
                switch (SelectedAlgorithm)
                {
                    case "Bubble Sort":
                        scriptIsRunning = true;
                        BubbleSort bubbleSort = new BubbleSort();
                        await bubbleSort.Sort(numbers, BarCanvas, () => SpeedSlider.Value, () => cancel, () => SuperSpeed.IsChecked);
                        arrIsSorted = true;
                        scriptIsRunning = false;
                        break;
                    case "Insertion Sort":
                        scriptIsRunning = true;
                        InsertionSort insertionSort = new InsertionSort();
                        await insertionSort.Sort(numbers, BarCanvas, () => SpeedSlider.Value, () => cancel, () => SuperSpeed.IsChecked);
                        arrIsSorted = true;
                        scriptIsRunning = false;
                        break;
                    case "Selection Sort":
                        scriptIsRunning = true;
                        SelectionSort selectionSort = new SelectionSort();
                        await selectionSort.Sort(numbers, BarCanvas, () => SpeedSlider.Value, () => cancel, () => SuperSpeed.IsChecked);
                        arrIsSorted = true;
                        scriptIsRunning = false;
                        break;
                    case "Heap Sort":
                        scriptIsRunning = true;
                        HeapSort heapSort = new HeapSort();
                        await heapSort.Sort(numbers, BarCanvas, () => SpeedSlider.Value, () => cancel, () => SuperSpeed.IsChecked);
                        arrIsSorted = true;
                        scriptIsRunning = false;
                        break;
                    case "Merge Sort":
                        scriptIsRunning = true;
                        MergeSort mergeSort = new MergeSort();
                        await mergeSort.Sort(numbers, BarCanvas, () => SpeedSlider.Value, () => cancel, () => SuperSpeed.IsChecked);
                        arrIsSorted = true;
                        scriptIsRunning = false;
                        break;
                    case "Quick Sort":
                        scriptIsRunning = true;
                        QuickSort quickSort = new QuickSort();
                        await quickSort.Sort(numbers, BarCanvas, () => SpeedSlider.Value, () => cancel, () => SuperSpeed.IsChecked);
                        arrIsSorted = true;
                        scriptIsRunning = false;
                        break;
                    default:
                        MessageBox.Show("NO ALGORITHM SELECTED!");
                        break;
                }
                NumberInput.IsEnabled = true;
                CancelButton.IsEnabled = false;
                if (cancel)
                {
                    arrIsSorted = false;
                    cancel = false;
                    DrawBars(BarCanvas);
                }
            }
        }

        private void ScrambleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!scriptIsRunning)
            {
                Shuffle(numbers);
                arrIsSorted = false;
                DrawBars(BarCanvas);
            }
        }
        private void DrawBars(Canvas canvas)
        {
            canvas.Children.Clear();
            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;

            double maxNumber = numbers.Max();

            double totalSpaces = numbers.Count + 1;
            double spacing = canvasWidth / (totalSpaces + numbers.Count);
            double rectWidth = spacing;
            double rectMaxHeight = canvasHeight;

            for (int i = 0; i < numbers.Count; i++)
            {
                double rectHeight = (numbers[i] / maxNumber) * rectMaxHeight;

                Rectangle rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = (arrIsSorted) ? Brushes.Green : Brushes.White
                };

                double x = (i + 1) * spacing + i * rectWidth;
                double y = canvasHeight - rectHeight;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                canvas.Children.Add(rect);
            }
        }
        private void NumberInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }



        private static void Shuffle<T>(List<T> list)
        {
            Random rand = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private static void GenerateRandomNumbers(int count, int min, int max, List<int> numbers)
        {
            Random _random = new Random();
            numbers.Clear();

            for (int i = 0; i < count; i++)
            {
                int randomNumber = _random.Next(min, max + 1);
                numbers.Add(randomNumber);
            }
        }

        private void NumberInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!scriptIsRunning)
                {
                    if (int.Parse(NumberInput.Text) != 0)
                    {
                        GenerateShuffledNumbers(int.Parse(NumberInput.Text), numbers);
                        NumberInput.Text = string.Empty;
                        arrIsSorted = false;
                        DrawBars(BarCanvas);
                    }
                    else
                    {
                        NumberInput.Text = string.Empty;
                    }
                }
            }
        }
        private static void GenerateShuffledNumbers(int count, List<int> numbers)
        {
            Random _random = new Random();
            numbers.Clear();

            // Vytvoř seznam čísel od 1 do count
            numbers.AddRange(Enumerable.Range(1, count));

            // Zamíchej čísla pomocí Fisher-Yates algoritmu
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]); // Prohození
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (scriptIsRunning)
            {
                cancel = true;
            }

        }

        private void OnSizeChange(object sender, SizeChangedEventArgs e)
        {
            DrawBars(BarCanvas);
        }


    }
}
