using rea_real_rocnikovka2.Step_Sorting_Algorithms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rea_real_rocnikovka2
{

    /// <summary>
    /// Interaction logic for Explanation.xaml
    /// </summary>
    /// 
    //auto step toggle doleva doprava
    //nejak locknout pomer height a width aby se to nezkazilo
    //locknout pravou cast aby nemenila velikost
    //udelat hezci toggle button na start stop auto stopping
    //pridat text toho, co se prave deje na obrazovce v explainu
    //fixnout selection sort aby mel porovnavatko
    //merge sort hezci mezery a lepsi cary
    //popisky
    //ui barvy
    // locknout zmenu velikosti po selekci algoritmu rmss
    //fixnout zelenou postupne menici se rmss
    public partial class Explanation : Page
    {
        public ObservableCollection<string> AlgorithmOptions { get; set; }
        public string SelectedAlgorithm { get; set; }
        private List<int> numbers { get; set; }


        private bool algorithmIsSelected;
        private bool isAutoStepping;


        private BubbleSortStep _bss;
        private InsertionSortStep _iss;
        private SelectionSortStep _sss;
        private RealMergeSortStep _rmss;
        private HeapSortStep _hss;
        private QuickSortStep _qss;

        public Explanation()
        {
            InitializeComponent();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            MainCanvas.RenderTransform = transformGroup;
            AlgorithmOptions = new ObservableCollection<string>(){
                "Bubble Sort",
                "Insertion Sort",
                "Selection Sort",
                "Quick Sort",
                "Heap Sort",
                "Merge Sort"
            };

            numbers = new List<int> { 10, 3, 5, 1, 9, 10, 3, 5, 1, 9 };
            AutoStepStopButton.IsEnabled = false;
            algorithmIsSelected = false;
            
            StepButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            AutoStepButton.IsEnabled = false;
            AutoStepStopButton.IsEnabled = false;

           
            DataContext = this;
        }
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (SelectedAlgorithm == null)
            {
                Draw(numbers);
            }
            _bss = new BubbleSortStep(numbers, MainCanvas, () => SpeedSlider.Value);
            _iss = new InsertionSortStep(numbers, MainCanvas, () => SpeedSlider.Value);
            _sss = new SelectionSortStep(numbers, MainCanvas, () => SpeedSlider.Value);
            _rmss = new RealMergeSortStep(numbers, MainCanvas, () => SpeedSlider.Value);
            _hss = new HeapSortStep(numbers, MainCanvas, () => SpeedSlider.Value);
            _qss = new QuickSortStep(numbers, MainCanvas, () => SpeedSlider.Value);
        }
        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            isAutoStepping=false;
        }
        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAutoStepping && algorithmIsSelected)
            {
                switch (SelectedAlgorithm)
                {
                    case "Bubble Sort":
                        _bss.Step();
                        //zbytek reset kdyz stepnu s timhle
                        break;
                    case "Insertion Sort":
                        _iss.Step();
                        break;
                    case "Selection Sort":
                        
                        _sss.Step();
                        break;
                    case "Merge Sort":
                        if (_rmss.IsSorted)
                            Application.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
                        _rmss.Step();
                        break;
                    case "Heap Sort":
                        _hss.Step();
                        break;
                    default:
                        MessageBox.Show("WORK IN PROGRESS");
                        break;
                }
            }
        }
    
        private void ResetButton_Click(Object sender, RoutedEventArgs e)
        {
            isAutoStepping = false;
            ResetAll();
            Draw(numbers);
        }

        
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!algorithmIsSelected)
            {
                if (SelectedAlgorithm != null)
                {
                    algorithmIsSelected = true;
                    AlgorithmComboBox.IsEnabled = false;
                    StepButton.IsEnabled = true;
                    ResetButton.IsEnabled = true;
                    AutoStepButton.IsEnabled = true;
                    SelectButton.Content = "Unselect";

                    switch (SelectedAlgorithm)
                    {
                        case "Merge Sort":
                            _rmss.OnSelect();
                            Application.Current.MainWindow.ResizeMode = ResizeMode.NoResize;
                            break;
                        case "Heap Sort":
                            _hss.OnSelect();
                            break;  
                        default:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("SELECT AN ALGORITHM");
                }
            } else
            {
                switch (SelectedAlgorithm)
                {
                    case "Bubble Sort":
                        numbers = _bss.Numbers;
                        Draw(numbers);
                        break;
                    case "Insertion Sort":
                        numbers = _iss.Numbers;
                        Draw(numbers);
                        break;
                    case "Selection Sort":
                        numbers = _sss.Numbers;
                        Draw(numbers);
                        break;
                    case "Merge Sort":
                        numbers = _rmss.Numbers;
                        Draw(numbers);
                        break;
                    case "Quick Sort":
                        _qss.Step();
                        Draw(numbers);
                        break;  
                    default:
                        Draw(numbers);
                        break;
                }
                ResetAll();
                SelectButton.Content = "Select";
                isAutoStepping = false;
                AlgorithmComboBox.IsEnabled = true;
                algorithmIsSelected = false;
                StepButton.IsEnabled = false;
                ResetButton.IsEnabled = false;
                AutoStepButton.IsEnabled = false;
            }
        }

        private async void AutoStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAutoStepping)
            {
                AutoStepButton.IsEnabled = false;
                isAutoStepping = true;
                AutoStepStopButton.IsEnabled = true;
                StepButton.IsEnabled = false;
                ResetButton.IsEnabled = false;
                SelectButton.IsEnabled = false;
                ScrambleButton.IsEnabled = false;


                switch (SelectedAlgorithm)
                {
                    case "Bubble Sort":
                        while (!_bss.IsSorted && isAutoStepping)
                        {
                            _bss.Step();
                            await Task.Delay((int)SpeedSlider.Value);
                        }
                        break;
                    case "Insertion Sort":
                        while (!_iss.IsSorted && isAutoStepping)
                        {
                            _iss.Step();
                            await Task.Delay((int)SpeedSlider.Value);
                        }
                        break;
                    case "Selection Sort":
                        while (!_sss.IsSorted && isAutoStepping)
                        {
                            _sss.Step();
                            await Task.Delay((int)SpeedSlider.Value);
                        }
                        break;
                    case "Merge Sort":
                        while (!_rmss.IsSorted && isAutoStepping)
                        {
                            _rmss.Step();
                            await Task.Delay((int)SpeedSlider.Value);
                        }
                        break;

                    case "Heap Sort":
                        while (!_rmss.IsSorted && isAutoStepping)
                        {
                            _hss.Step();
                            if (_hss.midWayAnimationIsRunning && !_hss.neverAgain)
                            {
                                //MessageBox.Show("WEWREEER");
                                await Task.Delay((int)SpeedSlider.Value + 1 );
                                _hss.neverAgain = true;
                            } else 
                                await Task.Delay((int)SpeedSlider.Value + 1);
                        }
                        break;
                        break;
                    default:
                        MessageBox.Show("HOW");
                        break;
                }
                Application.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
                ScrambleButton.IsEnabled = true;
                SelectButton.IsEnabled = true;
                ResetButton.IsEnabled = true;
                StepButton.IsEnabled = true;
                AutoStepStopButton.IsEnabled = false;
                AutoStepButton.IsEnabled = true;
                isAutoStepping = false;
            }
        }

        private void ResetAll()
        {
            _bss.Reset(numbers);
            _iss.Reset(numbers);
            _sss.Reset(numbers);
            _rmss.Reset(numbers);
        }
        private void StopAutoStepButton_Click(object sender, RoutedEventArgs e)
        {
            isAutoStepping = false;
        }



        private void Draw(List<int> _numbers)
        {
            MainCanvas.Children.Clear();

            double canvasHeight = MainCanvas.ActualHeight;
            double canvasWidth = MainCanvas.ActualWidth;

            int numOfBalls = _numbers.Count;

            double ballRadius = canvasWidth / (3 * numOfBalls + 1);
            double currentXPos = ballRadius;

            for (int i = 0; i < numOfBalls; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = ballRadius * 2,
                    Height = ballRadius * 2,
                    Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                    Stroke = new SolidColorBrush(Color.FromRgb(166, 92, 66)),
                    StrokeThickness = 0.1 * ballRadius//zavisle na velikosti r
                };

                Canvas.SetLeft(ellipse, currentXPos);
                Canvas.SetTop(ellipse, canvasHeight/2 - ballRadius);

                MainCanvas.Children.Add(ellipse);

                TextBlock number = new TextBlock
                {
                    Text = _numbers[i].ToString(),
                    FontSize = ballRadius/3 * 2, 
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                    TextAlignment = TextAlignment.Center
                };

                number.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = number.DesiredSize.Width;
                double textHeight = number.DesiredSize.Height;

                Canvas.SetLeft(number, currentXPos + ballRadius - textWidth / 2);
                Canvas.SetTop(number, canvasHeight / 2 - textHeight / 2);

                MainCanvas.Children.Add(number);



                currentXPos += 3 * ballRadius;
            }

        }


        private void ScrambleButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateRandomNumbers(numbers.Count, 1, 20, numbers);
            ResetAll();
            Draw(numbers);
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
        private double scaleFactor = 1.0;
        private double MinScale = 1.0;
        private bool isDragging = false;
        private Point lastMousePosition;
        private TranslateTransform translateTransform = new TranslateTransform();
        private ScaleTransform scaleTransform = new ScaleTransform(1, 1);
        private TransformGroup transformGroup = new TransformGroup();

        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Point mousePosition = e.GetPosition(MainCanvas);
                double delta = e.Delta > 0 ? 0.1 : -0.1;

                scaleFactor += delta;
                scaleFactor = Math.Max(scaleFactor, MinScale);

                ScaleTransform scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);

                double offsetX = mousePosition.X * (1 - scaleFactor);
                double offsetY = mousePosition.Y * (1 - scaleFactor);
                translateTransform.X = offsetX;
                translateTransform.Y = offsetY;

                MainCanvas.RenderTransform = new TransformGroup
                {
                    Children = { scaleTransform, translateTransform }
                };
            }
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                isDragging = true;
                lastMousePosition = e.GetPosition(MainCanvas);
                Mouse.Capture(MainCanvas);
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && scaleFactor > 1)
            {
                Point currentMousePosition = e.GetPosition(MainCanvas);

                // Calculate the change in position
                double offsetX = currentMousePosition.X - lastMousePosition.X;
                double offsetY = currentMousePosition.Y - lastMousePosition.Y;

                // Apply translation but prevent going outside the bounds after scaling
                double newTranslateX = translateTransform.X + offsetX;
                double newTranslateY = translateTransform.Y + offsetY;

                // Restrict the movement to avoid going beyond the initial position
                // (consider the canvas size and the scaled content size)
                double maxTranslateX = 0; // No left translation
                double maxTranslateY = 0; // No top translation
                double minTranslateX = -(MainCanvas.ActualWidth * (scaleFactor - 1)); // Prevent going right
                double minTranslateY = -(MainCanvas.ActualHeight * (scaleFactor - 1)); // Prevent going down

                translateTransform.X = Math.Max(minTranslateX, Math.Min(newTranslateX, maxTranslateX));
                translateTransform.Y = Math.Max(minTranslateY, Math.Min(newTranslateY, maxTranslateY));

                // Update last mouse position
                lastMousePosition = currentMousePosition;

                // Apply new transformations
                MainCanvas.RenderTransform = new TransformGroup
                {
                    Children = { new ScaleTransform(scaleFactor, scaleFactor), translateTransform }
                };
            }
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Mouse.Capture(null); // Release the mouse capture
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SelectedAlgorithm == "Heap Sort")
            {
                _hss.DrawComponenets();
            }
        }

        

    }
}
