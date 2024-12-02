using final_real_real_rocnikovka2.Algorithms;
using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Graphics.Rendering;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Comparsion.xaml
    /// </summary>
    public partial class ComparsionPage : Page
    {
        private double previousCanvasWidth;
        private double previousCanvasHeight;

        private List<int> Numbers1 { get; set; }
        private List<Box> Boxes1 { get; set; }
        private List<int> Numbers2 { get; set; }
        private List<Box> Boxes2 { get; set; }
        private List<int> Numbers3 { get; set; }
        private List<Box> Boxes3 { get; set; }
        private List<int> Numbers4 { get; set; }
        private List<Box> Boxes4 { get; set; }
        private List<int>[] Numbers { get; set; }
        private List<Box>[] Boxes { get; set; }
        private Canvas[] Canvases { get; set; }
        private ComboBox[] ComboBoxes { get; set; }

        private bool FirstLoad;
        private SortingAlgorithm SelectedAlgorithm1; 
        private SortingAlgorithm SelectedAlgorithm2;
        private SortingAlgorithm SelectedAlgorithm3;
        private SortingAlgorithm SelectedAlgorithm4;
        private SortingAlgorithm[] SelectedAlgorithms
        { get => new SortingAlgorithm[] { SelectedAlgorithm1, SelectedAlgorithm2, SelectedAlgorithm3, SelectedAlgorithm4 }; }




        private readonly List<SortingAlgorithm> SortingAlgorithms;

        public ComparsionPage(List<SortingAlgorithm> sortingAlgorithms)
        {
            InitializeComponent();
            this.SortingAlgorithms = sortingAlgorithms;
            
            Numbers1 = [5, 47, 98, 70, 74, 25, 81, 57, 13, 100, 84, 61, 11, 73, 75, 2, 48, 23, 17, 54, 22, 56, 19, 97, 79, 77, 38, 55, 26, 40, 45, 67, 24, 95, 94, 8, 33, 1, 30, 16, 28, 4, 50, 34, 12, 66, 52, 27, 59, 63, 82, 58, 65, 92, 10, 7, 78, 15, 44, 46, 91, 41, 6, 21, 85, 31, 14, 86, 80, 69, 20, 43, 71, 35, 42, 36, 72, 51, 96, 29, 64, 39, 53, 88, 90, 89, 9, 3, 62, 18, 99, 32, 37, 93, 83, 76, 68, 60, 49, 87];
            Numbers2 = [5, 47, 98, 70, 74, 25, 81, 57, 13, 100, 84, 61, 11, 73, 75, 2, 48, 23, 17, 54, 22, 56, 19, 97, 79, 77, 38, 55, 26, 40, 45, 67, 24, 95, 94, 8, 33, 1, 30, 16, 28, 4, 50, 34, 12, 66, 52, 27, 59, 63, 82, 58, 65, 92, 10, 7, 78, 15, 44, 46, 91, 41, 6, 21, 85, 31, 14, 86, 80, 69, 20, 43, 71, 35, 42, 36, 72, 51, 96, 29, 64, 39, 53, 88, 90, 89, 9, 3, 62, 18, 99, 32, 37, 93, 83, 76, 68, 60, 49, 87];
            Numbers3 = [5, 47, 98, 70, 74, 25, 81, 57, 13, 100, 84, 61, 11, 73, 75, 2, 48, 23, 17, 54, 22, 56, 19, 97, 79, 77, 38, 55, 26, 40, 45, 67, 24, 95, 94, 8, 33, 1, 30, 16, 28, 4, 50, 34, 12, 66, 52, 27, 59, 63, 82, 58, 65, 92, 10, 7, 78, 15, 44, 46, 91, 41, 6, 21, 85, 31, 14, 86, 80, 69, 20, 43, 71, 35, 42, 36, 72, 51, 96, 29, 64, 39, 53, 88, 90, 89, 9, 3, 62, 18, 99, 32, 37, 93, 83, 76, 68, 60, 49, 87];
            Numbers4 = [5, 47, 98, 70, 74, 25, 81, 57, 13, 100, 84, 61, 11, 73, 75, 2, 48, 23, 17, 54, 22, 56, 19, 97, 79, 77, 38, 55, 26, 40, 45, 67, 24, 95, 94, 8, 33, 1, 30, 16, 28, 4, 50, 34, 12, 66, 52, 27, 59, 63, 82, 58, 65, 92, 10, 7, 78, 15, 44, 46, 91, 41, 6, 21, 85, 31, 14, 86, 80, 69, 20, 43, 71, 35, 42, 36, 72, 51, 96, 29, 64, 39, 53, 88, 90, 89, 9, 3, 62, 18, 99, 32, 37, 93, 83, 76, 68, 60, 49, 87];

            Boxes1 = [];
            Boxes2 = [];
            Boxes3 = [];
            Boxes4 = [];
            FirstLoad = true;

            Numbers = [Numbers1, Numbers2, Numbers3, Numbers4];
            Boxes = [Boxes1, Boxes2, Boxes3, Boxes4];

            Canvases = [MainCanvas1, MainCanvas2, MainCanvas3, MainCanvas4];
            ComboBoxes = [AlgorithmComboBox1, AlgorithmComboBox2, AlgorithmComboBox3, AlgorithmComboBox4];
            Globals.AlgorithmIsRunning1 = false;

            PopulateComboBoxes();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (FirstLoad)
            {
                for (int i = 0; i < Numbers.Length; i++)
                {
                    InitializeRectObjects(Numbers[i], Canvases[i], Boxes[i]);
                }
                FirstLoad = false;
            }

            previousCanvasWidth = MainCanvas1.ActualWidth;
            previousCanvasHeight = MainCanvas1.ActualHeight;
            Globals.AnimationMs = (int)SpeedSlider.Value;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            Globals.Stop = true;
            Draw.ChangeColorForAll(Boxes.SelectMany(L => L), ColorPalette.DefaultBarFill);
        }
        private static void InitializeRectObjects(List<int> numbers, Canvas canvas, List<Box> boxes)
        {
            double width = canvas.ActualWidth / numbers.Count;
            double maxNumber = numbers.Max();
            double heightPerNum = (canvas.ActualHeight) / maxNumber;
            double xPos = 0;
            foreach (int number in numbers)
            {
                Box newBox = new(canvas, 1, ColorPalette.DefaultBarFill, width, number * heightPerNum);
                newBox.SetPosition(xPos, canvas.ActualHeight - number * heightPerNum);
                newBox.AddToCanvas();
                boxes.Add(newBox);

                xPos += width;
            }
        }
        private void PopulateComboBoxes()
        {
            for (int i = 0; i < ComboBoxes.Length; i++)
            {
                ComboBoxes[i].Items.Clear();
                foreach (var algorithm in SortingAlgorithms)
                {
                    ComboBoxes[i].Items.Add(algorithm);
                }
            }
        }
        private async void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            Globals.Stop = false;

            if (!Globals.AlgorithmIsRunning1)
            {
                Globals.AlgorithmIsRunning1 = true;

                List<Task> sortingTasks = new List<Task>();

                for (int i = 0; i < SelectedAlgorithms?.Length; i++)
                {
                    // Reset the algorithm before starting
                    SelectedAlgorithms[i]?.Reset(Numbers[i], Boxes[i]);

                    if (SelectedAlgorithms[i] != null)
                    {
                        sortingTasks.Add(SelectedAlgorithms[i].Sort());
                    }
                }

                await Task.WhenAll(sortingTasks);
                Globals.AlgorithmIsRunning1 = false;
            }
        }

        private void StopBtn_Click(Object sender, RoutedEventArgs e)
        {
            Globals.Stop = true;
            Globals.AlgorithmIsRunning1 = false;
            for (int i = 0; i < SelectedAlgorithms?.Length; i++)
            {
                if (SelectedAlgorithms[i] == null) continue;
                if (!SelectedAlgorithms[i].IsSorted())
                    Draw.ChangeColorForAll(Boxes.SelectMany(L => L), ColorPalette.DefaultBarFill);
            }

        }

        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Box box in Boxes.SelectMany(List => List))
            {
                box.Update(previousCanvasWidth, previousCanvasHeight);
            }
            previousCanvasWidth = MainCanvas1.ActualWidth;
            previousCanvasHeight = MainCanvas1.ActualHeight;
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Globals.AnimationMs = (int)SpeedSlider.Value;
        }

        private void OnComboBoxSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            CheckForDuplicateSelection(0);
        }

        private void OnComboBoxSelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            CheckForDuplicateSelection(1);
        }

        private void OnComboBoxSelectionChanged3(object sender, SelectionChangedEventArgs e)
        {
            CheckForDuplicateSelection(2);
        }

        private void OnComboBoxSelectionChanged4(object sender, SelectionChangedEventArgs e)
        {
            CheckForDuplicateSelection(3);
        }

        private void CheckForDuplicateSelection(int selectedIndex)
        {
            var selectedAlgorithm = ComboBoxes[selectedIndex].SelectedItem as SortingAlgorithm;

            for (int i = 0; i < ComboBoxes.Length; i++)
            {
                if (i != selectedIndex)
                {
                    if (ComboBoxes[i].SelectedItem == selectedAlgorithm)
                    {
                        ComboBoxes[selectedIndex].SelectedItem = null;

                        return;
                    }
                }
            }
            switch (selectedIndex + 1)
            {
                case 1:
                    SelectedAlgorithm1 = selectedAlgorithm;
                    break;
                case 2:
                    SelectedAlgorithm2 = selectedAlgorithm;
                    break;
                case 3:
                    SelectedAlgorithm3 = selectedAlgorithm;
                    break;
                case 4:
                    SelectedAlgorithm4 = selectedAlgorithm;
                    break;
            }
        }


    }
}
