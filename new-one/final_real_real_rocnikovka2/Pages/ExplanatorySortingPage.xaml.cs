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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace final_real_real_rocnikovka2.Pages
{
    /// <summary>
    /// Interaction logic for ExplanatorySortingPage.xaml
    /// </summary>
    // Udelat vsechny algoritmy :(
    // udelat lepsi toggle znazorneni autosteppingu
    public partial class ExplanatorySortingPage : Page
    {
        private double previousCanvasWidth;
        private double previousCanvasHeight;
        private double previousBallRadius;
        private bool IsAutoStepping;
        private bool FirstLoad;

        private readonly List<SortingAlgorithm> SortingAlgorithms;
        private SortingAlgorithm? SelectedAlgorithm { get; set; }
        private List<int> Numbers { get; set; }
        private List<Ball> Balls { get; set; }
        private List<GraphicElement> GraphicElements { get; set; }

        public ExplanatorySortingPage(List<SortingAlgorithm> sortingAlgorithms)
        {
            InitializeComponent();
            this.SortingAlgorithms = sortingAlgorithms;
            PopulateComboBox();

            GraphicElements = [];
            Numbers = [4, 2, 5, 3, 1];
            Balls = [];
            FirstLoad = true;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (FirstLoad)
            {
                InitializeBallObjects(Numbers);
                FirstLoad = false;
            }

            previousCanvasWidth = MainCanvas.ActualWidth;
            previousCanvasHeight = MainCanvas.ActualHeight;
            previousBallRadius = Draw.BallRadius;
            Globals.AnimationMs = (int)SpeedSlider.Value;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            IsAutoStepping = false;
            ResetBtn_Click(null, null);
        }
        private void InitializeBallObjects(List<int> numbers)
        {
            Draw.UpdateBallRadius(null, numbers.Count, MainCanvas);
            double xPos = Draw.BallRadius;
            double yPos = MainCanvas.ActualHeight / 2 - Draw.BallRadius;
            foreach (int number in numbers)
            {
                Ball newBall = new(MainCanvas, 1, ColorPalette.DefaultFill, ColorPalette.DefaultStroke);
                newBall.BallText = new(MainCanvas, 1, ColorPalette.TextColor, number.ToString(), 0);

                newBall.SetPosition(xPos, yPos);
                newBall.AddToCanvas();
                Balls.Add(newBall);

                xPos += 3 * Draw.BallRadius;
            }
        }
        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {

            Draw.UpdateBallRadius(SelectedAlgorithm, Numbers.Count, MainCanvas);
            foreach (Ball ball in Balls)
            {
                ball.Update(previousCanvasWidth, previousCanvasHeight, previousBallRadius);
            }
            foreach (GraphicElement gE in GraphicElements)
            {
                gE.Update(previousCanvasWidth, previousCanvasHeight);
                gE.Update(previousCanvasWidth, previousCanvasHeight, previousBallRadius);
            }
            previousCanvasWidth = MainCanvas.ActualWidth;
            previousCanvasHeight = MainCanvas.ActualHeight;
            previousBallRadius = Draw.BallRadius;

        }

        private void PopulateComboBox()
        {
            AlgorithmComboBox.Items.Clear();
            foreach (var algorithm in SortingAlgorithms)
            {
                AlgorithmComboBox.Items.Add(algorithm);
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            GraphicElements.ForEach(gE => gE.Delete());
            GraphicElements.Clear();
            SelectedAlgorithm?.Reset(Numbers, Balls, GraphicElements);
            Animate.AnimationSkip();
            Draw.ChangeColorForAll(Balls, ColorPalette.DefaultFill, ColorPalette.DefaultStroke);
            IsAutoStepping = false;
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is not Slider slider) return;

            Globals.AnimationMs = (int)slider.Value;
        }

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetBtn_Click(null, null);
            SelectedAlgorithm = (SortingAlgorithm)AlgorithmComboBox.SelectedItem;
            SelectedAlgorithm?.Reset(Numbers, Balls, GraphicElements);
        }

        private async void StepBtn_Click(object sender, RoutedEventArgs e)
        {
            IsAutoStepping = false;
            await Animate.AnimationSkip();
            SelectedAlgorithm?.Step();
        }
        
        private async void AutoStepBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (SelectedAlgorithm == null) return;
            if (!IsAutoStepping)
                IsAutoStepping = true;
            else
                IsAutoStepping = false;
            while (IsAutoStepping)
            {
                SelectedAlgorithm?.Step();
                await Task.Delay(TimeSpan.FromTicks(Math.Max(Animate.GetStoryboardDuration().Ticks, TimeSpan.FromMilliseconds(Globals.AnimationMs).Ticks)));

                if (SelectedAlgorithm.IsSortedBool) break;
            }
            IsAutoStepping = false;
        }
    }
}
