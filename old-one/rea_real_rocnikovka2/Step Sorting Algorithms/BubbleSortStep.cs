using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media.Converters;
using Accessibility;
using System.Windows.Media.Animation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Eventing.Reader;
using System.Numerics;

namespace rea_real_rocnikovka2.Step_Sorting_Algorithms
{
    public class BubbleSortStep
    {
        private List<int> _numbers;
        private int _currentIndex;
        private bool _isSorted;

        public List<int> Numbers => _numbers;
        public bool IsSorted => _isSorted;
        private int _stepState;
        private bool _swappedThisPass;
        Func<double> _getAnimationStepSpeed;
        private TimeSpan AnimationSeconds => TimeSpan.FromMilliseconds(_getAnimationStepSpeed());

        Storyboard _storyboard = new Storyboard();


        private Canvas _canvas;

        public BubbleSortStep(List<int> numbers, Canvas canvas, Func<double> getAnimationStepSpeed)
        {
            _numbers = new List<int>(numbers);
            _currentIndex = 0;
            _isSorted = false;
            _stepState = 0;
            _swappedThisPass = false;
            _canvas = canvas;
            _getAnimationStepSpeed = getAnimationStepSpeed;
        }

        public void Reset(List<int> numbers)
        {
            _numbers = new List<int>(numbers);
            _currentIndex = 0;
            _isSorted = false;
            _stepState = 0;
            _swappedThisPass = false;
        }

        public void Step()
        {
            if (_isSorted) return;
            
            switch (_stepState)
            {
                case 0:
                    DrawBallSelection(_currentIndex, _currentIndex + 1);
                    // select 2 balls animation
                    _stepState = 1;
                    break;
                case 1:
                    //compare 2 balls (select 2 balls)
                    DrawBallSelection(_currentIndex, _currentIndex + 1);
                    if (_numbers[_currentIndex] > _numbers[_currentIndex + 1])
                    {
                        DrawGreaterThan(_currentIndex, true);
                        _stepState = 2;
                    } else
                    {
                        DrawGreaterThan(_currentIndex, false);
                        _currentIndex++;
                        if (_currentIndex >= _numbers.Count - 1)
                        {
                            _currentIndex = 0;

                            if (!_swappedThisPass)
                            {
                                _isSorted = true;
                                DrawDone();
                            }
                            _swappedThisPass = false;
                        }

                        
                        _stepState = 0;
                    }
                    break;
                case 2:
                    //swapping animation
                    AnimateBallSwap(_currentIndex, _currentIndex + 1);
                    int temp = _numbers[_currentIndex];
                    _numbers[_currentIndex] = _numbers[_currentIndex + 1];
                    _numbers[_currentIndex + 1] = temp;


                    _swappedThisPass = true;

                    _currentIndex++;
                    if (_currentIndex >= _numbers.Count - 1)
                    {
                        _currentIndex = 0;
                    }
                    _stepState = 0;
                    break;


            }


        }


        private void AnimateBallSwap(int indexA, int indexB)
        {
            double canvasHeight = _canvas.ActualHeight;
            double canvasWidth = _canvas.ActualWidth;

            int numOfBalls = _numbers.Count;

            double ballRadius = canvasWidth / (3 * numOfBalls + 1);

            double ballA_XPos = ballRadius * (3 * (indexA + 1) - 2);
            double ballB_XPos = ballRadius * (3 * (indexB + 1) - 2);

            double arcHeight = ballRadius * 1.5;

            

            Ellipse ballA = _canvas.Children.OfType<Ellipse>().ElementAt(indexA);
            Ellipse ballB = _canvas.Children.OfType<Ellipse>().ElementAt(indexB);

            TextBlock textA = _canvas.Children.OfType<TextBlock>().ElementAt(indexA);
            TextBlock textB = _canvas.Children.OfType<TextBlock>().ElementAt(indexB);
            TextBlock greaterThanSymbol = _canvas.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Text == ">");

            TimeSpan animationSeconds = AnimationSeconds;

            ColorAnimation strokeColorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(38, 162, 150),  // Původní barva okraje
                To = Color.FromRgb(166, 92, 66),     // Nová barva okraje
                Duration = animationSeconds,         // Délka animace
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }  // Způsob animace
            };

            Storyboard.SetTarget(strokeColorAnimation, ballB);
            Storyboard.SetTargetProperty(strokeColorAnimation, new PropertyPath("Stroke.Color"));


            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,  
                To = 0,    
                Duration = animationSeconds,  
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(fadeOutAnimation, greaterThanSymbol);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

            DoubleAnimation moveBallA = new DoubleAnimation
            {
                From = ballA_XPos,
                To = ballB_XPos,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveBallB = new DoubleAnimation
            {
                From = ballB_XPos,
                To = ballA_XPos,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveBallUpA = new DoubleAnimation
            {
                From = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexA)),
                To = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexA)) - arcHeight,
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            DoubleAnimation moveBallDownA = new DoubleAnimation
            {
                From = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexA)) - arcHeight,
                To = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexA)),
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
                BeginTime = animationSeconds/2
            };

            DoubleAnimation moveBallDownB = new DoubleAnimation
            {
                From = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexB)),
                To = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexB)) + arcHeight,
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            };

            DoubleAnimation moveBallUpB = new DoubleAnimation
            {
                From = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexB)) + arcHeight,
                To = Canvas.GetTop(_canvas.Children.OfType<Ellipse>().ElementAt(indexB)),
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
                BeginTime = animationSeconds/2
            };


            textA.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double ATextWidth = textA.DesiredSize.Width;
            double ATextHeight = textA.DesiredSize.Height;

            textB.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double BTextWidth = textB.DesiredSize.Width;
            double BTextHeight = textB.DesiredSize.Height;


            DoubleAnimation moveTextA = new DoubleAnimation
            {
                From = ballA_XPos + ballRadius - ATextWidth / 2,
                To = ballB_XPos + ballRadius - ATextWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveTextB = new DoubleAnimation
            {
                From = ballB_XPos + ballRadius - BTextWidth / 2,
                To = ballA_XPos + ballRadius - BTextWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveTextUpA = new DoubleAnimation
            {
                From = Canvas.GetTop(textA),
                To = Canvas.GetTop(textA) - arcHeight,
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            DoubleAnimation moveTextDownA = new DoubleAnimation
            {
                From = Canvas.GetTop(textA) - arcHeight,
                To = Canvas.GetTop(textA),
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
                BeginTime = animationSeconds/2
            };

            DoubleAnimation moveTextUpB = new DoubleAnimation
            {
                From = Canvas.GetTop(textB) + arcHeight,
                To = Canvas.GetTop(textB),
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
                BeginTime = animationSeconds/2
            };

            DoubleAnimation moveTextDownB = new DoubleAnimation
            {
                From = Canvas.GetTop(textB),
                To = Canvas.GetTop(textB) + arcHeight,
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(moveBallUpA, ballA);
            Storyboard.SetTarget(moveBallDownA, ballA);
            Storyboard.SetTargetProperty(moveBallUpA, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(moveBallDownA, new PropertyPath(Canvas.TopProperty));

            Storyboard.SetTarget(moveBallUpB, ballB);
            Storyboard.SetTarget(moveBallDownB, ballB);
            Storyboard.SetTargetProperty(moveBallUpB, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(moveBallDownB, new PropertyPath(Canvas.TopProperty));

            Storyboard.SetTarget(moveBallA, ballA);
            Storyboard.SetTarget(moveBallB, ballB);
            Storyboard.SetTargetProperty(moveBallA, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(moveBallB, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTarget(moveTextA, textA);
            Storyboard.SetTarget(moveTextB, textB);
            Storyboard.SetTargetProperty(moveTextA, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(moveTextB, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTarget(moveTextUpA, textA);
            Storyboard.SetTarget(moveTextDownA, textA);
            Storyboard.SetTargetProperty(moveTextUpA, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(moveTextDownA, new PropertyPath(Canvas.TopProperty));

            Storyboard.SetTarget(moveTextUpB, textB);
            Storyboard.SetTarget(moveTextDownB, textB);
            Storyboard.SetTargetProperty(moveTextUpB, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTargetProperty(moveTextDownB, new PropertyPath(Canvas.TopProperty));

            _storyboard.Children.Add(moveBallA);
            _storyboard.Children.Add(moveBallB);
            _storyboard.Children.Add(moveTextA);
            _storyboard.Children.Add(moveTextB);
            _storyboard.Children.Add(moveBallUpA);
            _storyboard.Children.Add(moveBallDownA);
            _storyboard.Children.Add(moveBallDownB);
            _storyboard.Children.Add(moveBallUpB);
            _storyboard.Children.Add(moveTextUpA);
            _storyboard.Children.Add(moveTextDownA);
            _storyboard.Children.Add(moveTextDownB);
            _storyboard.Children.Add(moveTextUpB);
            _storyboard.Children.Add(fadeOutAnimation);
            _storyboard.Children.Add(strokeColorAnimation);

            _storyboard.Begin();
        }



        private void DrawGreaterThan(int IndexA, bool truth)
        {
            double canvasHeight = _canvas.ActualHeight;
            double canvasWidth = _canvas.ActualWidth;

            int numOfBalls = _numbers.Count;

            double ballRadius = canvasWidth / (3 * numOfBalls + 1);
            double xPos = ballRadius *( 3 * (IndexA + 1) + 0.5);
            SolidColorBrush color;
            if (truth)
            {
                color = Brushes.Green;
            } else
            {
                color = Brushes.Red;
            }
          
            TextBlock textBlock = new TextBlock
            {
                Text = ">",
                FontSize = ballRadius,         
                FontWeight = FontWeights.Bold,
                Foreground = color,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = textBlock.DesiredSize.Width;
            double textHeight = textBlock.DesiredSize.Height;

            Canvas.SetLeft(textBlock, xPos - textWidth/2);
            Canvas.SetTop(textBlock, canvasHeight / 2 - ballRadius + textHeight/8);

            _canvas.Children.Add(textBlock);
        }

        private void DrawBallSelection(int IndexA, int IndexB)
        {
            _canvas.Children.Clear();

            double canvasHeight = _canvas.ActualHeight;
            double canvasWidth = _canvas.ActualWidth;

            int numOfBalls = _numbers.Count;

            double ballRadius = canvasWidth / (3 * numOfBalls + 1);
            double currentXPos = ballRadius;

            for (int i = 0; i < numOfBalls; i++)
            {
                SolidColorBrush strokeColor = new SolidColorBrush(Color.FromRgb(166, 92, 66));
                if (i == IndexA || i == IndexB)
                {
                    strokeColor = new SolidColorBrush(Color.FromRgb(38, 162, 150));

                }
                Ellipse ellipse = new Ellipse
                {
                    Width = ballRadius * 2,
                    Height = ballRadius * 2,
                    Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                    Stroke = strokeColor,
                    StrokeThickness = 0.1 * ballRadius//zavisle na velikosti r
                };

                Canvas.SetLeft(ellipse, currentXPos);
                Canvas.SetTop(ellipse, canvasHeight / 2 - ballRadius);

                _canvas.Children.Add(ellipse);

                TextBlock number = new TextBlock
                {
                    Text = _numbers[i].ToString(),
                    FontSize = ballRadius / 3 * 2,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                    TextAlignment = TextAlignment.Center
                };

                number.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = number.DesiredSize.Width;
                double textHeight = number.DesiredSize.Height;

                Canvas.SetLeft(number, currentXPos + ballRadius - textWidth / 2);
                Canvas.SetTop(number, canvasHeight / 2 - textHeight / 2);

                _canvas.Children.Add(number);

                currentXPos += 3 * ballRadius;
            }

        }

        private void DrawDone()
        {
            _canvas.Children.Clear();

            double canvasHeight = _canvas.ActualHeight;
            double canvasWidth = _canvas.ActualWidth;

            int numOfBalls = _numbers.Count;

            double ballRadius = canvasWidth / (3 * numOfBalls + 1);
            double currentXPos = ballRadius;

            for (int i = 0; i < numOfBalls; i++)
            {
                SolidColorBrush strokeColor = new SolidColorBrush(Color.FromRgb(54, 144, 92));

                Ellipse ellipse = new Ellipse
                {
                    Width = ballRadius * 2,
                    Height = ballRadius * 2,
                    Fill = new SolidColorBrush(Color.FromRgb(118, 174, 91)),
                    Stroke = strokeColor,
                    StrokeThickness = 0.1 * ballRadius//zavisle na velikosti r
                };

                Canvas.SetLeft(ellipse, currentXPos);
                Canvas.SetTop(ellipse, canvasHeight / 2 - ballRadius);

                _canvas.Children.Add(ellipse);

                TextBlock number = new TextBlock
                {
                    Text = _numbers[i].ToString(),
                    FontSize = ballRadius / 3 * 2,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                    TextAlignment = TextAlignment.Center
                };

                number.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = number.DesiredSize.Width;
                double textHeight = number.DesiredSize.Height;

                Canvas.SetLeft(number, currentXPos + ballRadius - textWidth / 2);
                Canvas.SetTop(number, canvasHeight / 2 - textHeight / 2);

                _canvas.Children.Add(number);

                currentXPos += 3 * ballRadius;
            }

        }
    }
}
