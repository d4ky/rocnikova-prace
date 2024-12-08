using final_real_real_rocnikovka2.Algorithms;
using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Graphics.Rendering;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace final_real_real_rocnikovka2.Pages
{
    /// <summary>
    /// Interaction logic for ClassicSortingPage.xaml
    /// </summary>
    /// 

    //super speed text kdyz bude animation speed mensi nez 14
    //zavorka za super speed ktera oznami ze super speed je disabled kdyz multisichecked
    //obdelnik v levem hornim rohu s comparison, swap, lookup count
    public partial class ClassicSortingPage : Page
    {
        private double previousCanvasWidth;
        private double previousCanvasHeight;
        private bool FirstLoad;

        private readonly List<SortingAlgorithm> SortingAlgorithms;
        public SortingAlgorithm? SelectedAlgorithm { get; set; }
        private List<int> Numbers { get; set; }
        private List<Box> Boxes { get; set; }


        private SortingAlgorithm _runningAlgorithm;


        
        public ClassicSortingPage(List<SortingAlgorithm> sortingAlgorithms)
        {
            InitializeComponent();
            this.SortingAlgorithms = sortingAlgorithms;
            PopulateComboBox();
            Numbers = [5, 47, 98, 70, 74, 25, 81, 57, 13, 100, 84, 61, 11, 73, 75, 2, 48, 23, 17, 54, 22, 56, 19, 97, 79, 77, 38, 55, 26, 40, 45, 67, 24, 95, 94, 8, 33, 1, 30, 16, 28, 4, 50, 34, 12, 66, 52, 27, 59, 63, 82, 58, 65, 92, 10, 7, 78, 15, 44, 46, 91, 41, 6, 21, 85, 31, 14, 86, 80, 69, 20, 43, 71, 35, 42, 36, 72, 51, 96, 29, 64, 39, 53, 88, 90, 89, 9, 3, 62, 18, 99, 32, 37, 93, 83, 76, 68, 60, 49, 87];
            Boxes = [];
            SwapCountTextBlock.Text = $"Swap Count: 0";
            ComparisonCountTextBlock.Text = $"Comparison count: 0";
            FirstLoad = true;
            SpeedText.Text = $"{(int)SpeedSlider.Value} ms";
            DataContext = this;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (FirstLoad)
            {
                InitializeRectObjects(Numbers);
                FirstLoad = false;
            }
                
            previousCanvasWidth = MainCanvas.ActualWidth;
            previousCanvasHeight = MainCanvas.ActualHeight;
            Globals.AnimationMs = (int)SpeedSlider.Value;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            Globals.Stop = true;
            Draw.ChangeColorForAll(Boxes, ColorPalette.DefaultBarFill);
        }


        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Box box in Boxes)
            {
                box.Update(previousCanvasWidth, previousCanvasHeight);
            }
            previousCanvasWidth = MainCanvas.ActualWidth;
            previousCanvasHeight = MainCanvas.ActualHeight;

        }

        private void InitializeRectObjects(List<int> numbers)
        {
            double width = MainCanvas.ActualWidth/numbers.Count;
            double maxNumber = numbers.Max();
            double heightPerNum = (MainCanvas.ActualHeight) / maxNumber;
            double xPos = 0;
            foreach (int number in numbers)
            {
                Box newBox = new(MainCanvas, 1, ColorPalette.DefaultBarFill, width, number * heightPerNum);
                newBox.SetPosition(xPos, MainCanvas.ActualHeight - number * heightPerNum);
                newBox.AddToCanvas();
                Boxes.Add(newBox);

                xPos += width;
            }
        }

        private void PopulateComboBox()
        {
            AlgorithmComboBox.Items.Clear();
            foreach (var algorithm in SortingAlgorithms)
            {
                AlgorithmComboBox.Items.Add(algorithm);
            }
        }


        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAlgorithm = (SortingAlgorithm)AlgorithmComboBox.SelectedItem;
        }

        private async void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAlgorithm == null) return;
            Globals.Stop = false;
            if (!Globals.AlgorithmIsRunning || Globals.MultiIsChecked)
            {
                _runningAlgorithm = SelectedAlgorithm;
                Globals.AlgorithmIsRunning = true;

                SelectedAlgorithm.PropertyChanged += OnAlgorithmStatsChanged;

                SelectedAlgorithm.Reset(Numbers, Boxes);
                await SelectedAlgorithm.Sort();

                SelectedAlgorithm.PropertyChanged -= OnAlgorithmStatsChanged;

                Globals.AlgorithmIsRunning = false;
            } 
        }
        private void OnAlgorithmStatsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == _runningAlgorithm)
            {
                SwapCountTextBlock.Text = $"Swap Count: {_runningAlgorithm.SwapCount.ToString()}";
                ComparisonCountTextBlock.Text = $"Comparison count: {_runningAlgorithm.ComparisonCount.ToString()}";
            }
        }
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Globals.AnimationMs = (int)SpeedSlider.Value;

            if (SpeedText == null) return;

            if (Globals.AnimationMs >= 15)
            {
                SpeedText.Inlines.Clear();
                SpeedText.Text = $"{Globals.AnimationMs} ms";
                return;
            }

            if (Globals.MultiIsChecked)
            {
                SpeedText.Inlines.Clear();
                SpeedText.Inlines.Add(new Run("1 ms ("));


                Run redRun = new Run("multi is checked")
                {
                    Foreground = new SolidColorBrush(Colors.Red)
                };
                SpeedText.Inlines.Add(redRun);

                SpeedText.Inlines.Add(new Run(")"));

                return;
            }
            string text = "Super fast!";
            int maxValue = 15;

            SpeedText.Inlines.Clear();

            double greenness = 1 - (Globals.AnimationMs / (double)maxValue);

            for (int i = 0; i < text.Length; i++)
            {
                Color color = Color.FromRgb(
                    (byte)(255 * (1 - greenness)),
                    255,
                    (byte)(255 * (1 - greenness)) 
                );

                Run run = new Run(text[i].ToString())
                {
                    Foreground = new SolidColorBrush(color)
                };

                SpeedText.Inlines.Add(run);
            }

        }

        private void StopBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (SelectedAlgorithm == null) return;
            Globals.Stop = true;
            if (!SelectedAlgorithm.IsSorted())
                Draw.ChangeColorForAll(Boxes, ColorPalette.DefaultBarFill);

        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Globals.MultiIsChecked = false;
            Globals.Stop = true;
            SpeedSlider_ValueChanged(null, null);
            if (SelectedAlgorithm == null) return;
            if (!SelectedAlgorithm.IsSorted())
                Draw.ChangeColorForAll(Boxes, ColorPalette.DefaultBarFill);
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            Globals.MultiIsChecked = true; 
            SpeedSlider_ValueChanged(null, null);
        }

        private void ScrambleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.EndAnimationIsRunning) return;
            Random random = new Random();
            int n = Numbers.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = Numbers[i];
                Numbers[i] = Numbers[j];
                Numbers[j] = temp;
                Box tempBox = Boxes[i];
                Boxes[i] = Boxes[j];
                Boxes[j] = tempBox;

                Draw.SwapXPos(Boxes[i], Boxes[j]);
            }
            StopBtn_Click(null, null );
        }
    }
}
