using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace rea_real_rocnikovka2.Step_Sorting_Algorithms
{
    public class RealMergeSortStep
    {
        private List<int> _numbers;
        private Canvas _canvas;
        Func<double> _getAnimationStepSpeed;
        private TimeSpan AnimationSeconds => TimeSpan.FromMilliseconds(_getAnimationStepSpeed());
        private List<Node> _nodes;
        private Dictionary<int, List<Node>> splitLayers;
        private Dictionary<int, List<Node>> mergeLayers;
        private double _verticalGap = 1; // tohle krat R je mezera
        private int maxSplitLayerIndex;
        public double _ballRadius => Math.Min(_canvas.ActualWidth / (6 * _numbers.Count), _canvas.ActualHeight / (_verticalGap*(mergeLayers.Keys.Count + splitLayers.Keys.Count + 1) + 2 * (mergeLayers.Keys.Count + splitLayers.Keys.Count)));
        private Storyboard _storyboard;
        private int _stepState;
        private bool _isSorted;
        public List<int> Numbers => _numbers;

        public RealMergeSortStep(List<int> numbers, Canvas canvas, Func<double> getAnimationStepSpeed)
        {
            _numbers = numbers;
            _canvas = canvas;
            _stepState = 0;
            _getAnimationStepSpeed = getAnimationStepSpeed;
            _nodes = new List<Node>();
            splitLayers = new Dictionary<int, List<Node>>();
            mergeLayers = new Dictionary<int, List<Node>>();
            _currentLayer = 1;
            _realLayer = 0;
            _secondPhase = false;
            _leftIsBigger = false;
            _xPos = 0;
            _leftIndexB = 0;
            _leftIndexA = 0;
            _storyboard = new Storyboard();
            _isFirstStep = false;
            _currentIndex = 0;
        }
        public bool IsSorted => _isSorted;
        private int _currentLayer;
        private bool _secondPhase;
        private int _realLayer;
        private int _leftIndexA;
        private int _leftIndexB;
        private int _currentIndex;
        private bool _isFirstStep;
        double _xPos;
        private bool _leftIsBigger;

        public void Reset(List<int> numbers)
        {
            _numbers = numbers;
            _nodes.Clear();
            splitLayers.Clear();
            mergeLayers.Clear();
            maxSplitLayerIndex = 0;
            _stepState = 0;
            _isSorted = false;
            _currentLayer = 0;
            _isFirstStep = false;
            _secondPhase = false;
            _storyboard = new Storyboard();
            _realLayer = 1;
            _leftIndexA = 0;
            _leftIndexB = 0;
            _currentIndex = 0;
            _xPos = 0;
            _leftIsBigger= false;
            PreRun();
        }

        public void Step()
        {
            if (_numbers.Count == 1) _isSorted = true;
            if (!_isSorted)
            {
                if (!_secondPhase)
                {
                    DrawSplitLayers(_currentLayer);
                    if (_currentLayer == splitLayers.Keys.Max())
                    {
                        _secondPhase = true;
                        _currentLayer = mergeLayers.Keys.Max();
                        _xPos = (_canvas.ActualWidth - _ballRadius *
                        (3 * (mergeLayers[_currentLayer].Sum(innerList => innerList.Numbers.Count) * 2
                        - mergeLayers[_currentLayer].Last().Numbers.Count) - 1)) / 2;
                    } else
                    {
                        _currentLayer++;
                    }
                    _realLayer++;
                } else
                {
                    switch (_stepState)
                    {
                        case 0:
                            if (_currentIndex >= mergeLayers[_currentLayer].Count)
                            {
                                _currentLayer--;
                                _currentIndex = 0;
                                _xPos = (_canvas.ActualWidth - _ballRadius *
                                    (3 * (mergeLayers[_currentLayer].Sum(innerList => innerList.Numbers.Count) * 2
                                    - mergeLayers[_currentLayer].Last().Numbers.Count) - 1)) / 2;
                            
                            }
                        
                            //oznaci leveho rodice a praveho rodice
                            if (mergeLayers[_currentLayer][_currentIndex].LeftParent == null || mergeLayers[_currentLayer][_currentIndex].RightParent == null)
                            {
                                if (mergeLayers[_currentLayer][_currentIndex].LeftParent != null)
                                    HighlightSingleParentNode(mergeLayers[_currentLayer][_currentIndex].LeftParent, true);
                                else if (mergeLayers[_currentLayer][_currentIndex].RightParent != null)
                                    HighlightSingleParentNode(mergeLayers[_currentLayer][_currentIndex].RightParent, true);
                                else MessageBox.Show("OUUUUUUUUU :(");
                                _stepState = 5;
                            } else
                            {
                                HighlightParents(mergeLayers[_currentLayer][_currentIndex].LeftParent, mergeLayers[_currentLayer][_currentIndex].RightParent);
                        
                                _stepState = 1;
                            }
                            break;
                        case 1:
                            HighlightTwoBalls(_leftIndexA, _leftIndexB, mergeLayers[_currentLayer][_currentIndex]);
                            //jinak oznaci leftindexA a leftindexB -> stepstate = 2 (porovnani)
                            _stepState = 2;
                            break;
                        case 2:

                            if (mergeLayers[_currentLayer][_currentIndex].LeftParent.Numbers[_leftIndexA] > mergeLayers[_currentLayer][_currentIndex].RightParent.Numbers[_leftIndexB])
                            {
                                ComparisonDraw(_leftIndexA, _leftIndexB, mergeLayers[_currentLayer][_currentIndex], '>');
                                _leftIsBigger = true;

                            } else
                            {
                                ComparisonDraw(_leftIndexA, _leftIndexB, mergeLayers[_currentLayer][_currentIndex], '<');
                                _leftIsBigger = false;
                            }
                            _stepState = 3;
                            break;
                        case 3:
                            if (_leftIsBigger)
                            {
                                MoveSmallerDown(mergeLayers[_currentLayer][_currentIndex], _leftIndexB, mergeLayers[_currentLayer][_currentIndex].RightParent, true);
                                //zsedit
                                _leftIndexB++;
                                _xPos += 3 * _ballRadius;
                                if (_leftIndexB >= mergeLayers[_currentLayer][_currentIndex].RightParent.Numbers.Count)
                                {
                                    _stepState = 4;
                                } else
                                {
                                    _stepState = 1;
                                }
                            } else
                            {
                                MoveSmallerDown(mergeLayers[_currentLayer][_currentIndex], _leftIndexA, mergeLayers[_currentLayer][_currentIndex].LeftParent, true);
                                _leftIndexA++;
                                _xPos += 3 * _ballRadius;
                                if (_leftIndexA >= mergeLayers[_currentLayer][_currentIndex].LeftParent.Numbers.Count)
                                {
                                    _stepState = 4;
                                } else
                                {
                                    _stepState = 1;
                                }
                            
                            }
                            break;  
                        case 4:
                            if (_leftIsBigger)
                            {
                                MoveSmallerDown(mergeLayers[_currentLayer][_currentIndex], _leftIndexA, mergeLayers[_currentLayer][_currentIndex].LeftParent);
                                _leftIndexA++;
                                if (_leftIndexA >= mergeLayers[_currentLayer][_currentIndex].LeftParent.Numbers.Count)
                                {
                                    _storyboard.Children.Clear();
                                    DrawSmoothLine(new Point(mergeLayers[_currentLayer][_currentIndex].middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].yPos),
                                        new Point(mergeLayers[_currentLayer][_currentIndex].LeftParent.middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].LeftParent.yPos + 2 * _ballRadius),
                                        _ballRadius, true, new SolidColorBrush(Color.FromRgb(160, 160, 160)));
                                    DrawSmoothLine(new Point(mergeLayers[_currentLayer][_currentIndex].middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].yPos),
                                        new Point(mergeLayers[_currentLayer][_currentIndex].RightParent.middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].RightParent.yPos + 2 * _ballRadius),
                                        _ballRadius, true, new SolidColorBrush(Color.FromRgb(160, 160, 160)));
                                    _storyboard.Begin();
                                    if (_currentIndex+1 >= mergeLayers[_currentLayer].Count && _currentLayer - 1 < mergeLayers.Keys.Min())
                                    {
                                        DoneDraw(mergeLayers[_currentLayer][_currentIndex].yPos);
                                        _isSorted = true;
                                    }
                                    _xPos += _ballRadius * (3 * mergeLayers[_currentLayer][_currentIndex].Numbers.Count);
                                    _stepState = 0;
                                    _currentIndex++;
                                    FadeOutRectangles();
                                    _leftIndexA = 0;
                                    _leftIndexB = 0;
                                }
                            }
                            else
                            {
                                MoveSmallerDown(mergeLayers[_currentLayer][_currentIndex], _leftIndexB, mergeLayers[_currentLayer][_currentIndex].RightParent);
                                _leftIndexB++;
                                if (_leftIndexB >= mergeLayers[_currentLayer][_currentIndex].RightParent.Numbers.Count)
                                {
                                    _storyboard.Children.Clear();
                                    DrawSmoothLine(new Point(mergeLayers[_currentLayer][_currentIndex].middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].yPos),
                                        new Point(mergeLayers[_currentLayer][_currentIndex].LeftParent.middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].LeftParent.yPos + 2 * _ballRadius),
                                        _ballRadius, true, new SolidColorBrush(Color.FromRgb(160, 160, 160)));
                                    DrawSmoothLine(new Point(mergeLayers[_currentLayer][_currentIndex].middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].yPos),
                                        new Point(mergeLayers[_currentLayer][_currentIndex].RightParent.middleXPos,
                                        mergeLayers[_currentLayer][_currentIndex].RightParent.yPos + 2 * _ballRadius),
                                        _ballRadius, true, new SolidColorBrush(Color.FromRgb(160, 160, 160)));
                                    _storyboard.Begin();
                                    if (_currentIndex+1 >= mergeLayers[_currentLayer].Count && _currentLayer - 1 < mergeLayers.Keys.Min())
                                    {
                                        DoneDraw(mergeLayers[_currentLayer][_currentIndex].yPos);
                                        _isSorted = true;
                                    }
                                    _xPos += _ballRadius * (3 * mergeLayers[_currentLayer][_currentIndex].Numbers.Count);
                                    _stepState = 0;
                                    _currentIndex++;
                                    FadeOutRectangles();
                                    _leftIndexA = 0;
                                    _leftIndexB = 0;
                                }
                            }
                            _xPos += 3 * _ballRadius;
                            //dosune index na posledni misto udelaji se cary
                            //stepstate = 0, currentindex++
                            break;
                        case 5:
                            if (mergeLayers[_currentLayer][_currentIndex].LeftParent != null)
                                SingleParentAnimation(mergeLayers[_currentLayer][_currentIndex].LeftParent, mergeLayers[_currentLayer][_currentIndex]);
                            else if (mergeLayers[_currentLayer][_currentIndex].RightParent != null)
                                SingleParentAnimation(mergeLayers[_currentLayer][_currentIndex].RightParent, mergeLayers[_currentLayer][_currentIndex]);
                        
                            if (_currentIndex+1 >= mergeLayers[_currentLayer].Count && _currentLayer - 1 < mergeLayers.Keys.Min())
                            {
                                DoneDraw(mergeLayers[_currentLayer][_currentIndex].yPos);
                                _isSorted = true;
                            }
                            _stepState = 0;
                            _leftIndexA = 0;
                            _leftIndexB = 0;
                            _currentIndex++;
                            FadeOutRectangles();
                            _xPos += _ballRadius * 6;
                            break;
                    }
                }    
            }
        }

        private void FadeOutRectangles()
        {
            var transparentEllipses = _canvas.Children.OfType<Rectangle>()
                .Where(e => (e.Fill as SolidColorBrush)?.Color == Colors.Transparent)
                .ToList();
            TimeSpan animationSeconds = AnimationSeconds;
            foreach (var e in transparentEllipses)
            {
                DoubleAnimation fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = animationSeconds,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                fadeOutAnimation.Completed += (s, args) =>
                {
                    _canvas.Children.Remove(e);
                };
                Storyboard.SetTarget(fadeOutAnimation, e);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));


                _storyboard.Children.Add(fadeOutAnimation);
            }

            _storyboard.Begin();
        }

        private void DoneDraw(double y)
        {
            var ellipsesAtY = _canvas.Children.OfType<Ellipse>().Where(e => Canvas.GetTop(e) == y).ToList();
            ellipsesAtY.Add(_canvas.Children.OfType<Ellipse>().LastOrDefault());
            Color fromFillColor = Color.FromRgb(216, 122, 86); 
            Color toFillColor = Color.FromRgb(118, 174, 91); 

            Color fromStrokeColor = Color.FromRgb(166, 92, 66); 
            Color toStrokeColor = Color.FromRgb(54, 144, 92);
            _storyboard.Children.Clear();
            TimeSpan animationSeconds = AnimationSeconds;
            foreach (var ellipse in ellipsesAtY)
            {
                ColorAnimation fillColorAnimation = new ColorAnimation
                {
                    From = fromFillColor,
                    To = toFillColor,
                    Duration = animationSeconds, 
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                ColorAnimation strokeColorAnimation = new ColorAnimation
                {
                    From = fromStrokeColor,
                    To = toStrokeColor,
                    Duration = animationSeconds,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(fillColorAnimation, ellipse);
                Storyboard.SetTargetProperty(fillColorAnimation, new PropertyPath("Fill.Color"));

                Storyboard.SetTarget(strokeColorAnimation, ellipse);
                Storyboard.SetTargetProperty(strokeColorAnimation, new PropertyPath("Stroke.Color"));

                _storyboard.Children.Add(fillColorAnimation);
                _storyboard.Children.Add(strokeColorAnimation);
            }

            _storyboard.Begin();
        }
        private void MoveSmallerDown(Node son, int index, Node parent, bool comparisonBefore = false)
        {
            double ballRadius = _ballRadius;
            _storyboard.Children.Clear();
            TimeSpan animationSeconds = AnimationSeconds;
            double yPos = ballRadius * ((son.VerticalLayer + 1) * _verticalGap + 2 * (son.VerticalLayer));

            Ellipse greyEllipse = new Ellipse
            {
                Width = ballRadius * 2,
                Height = ballRadius * 2,
                Fill = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Stroke = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                StrokeThickness = 0.1 * ballRadius
            };

            Canvas.SetLeft(greyEllipse, parent.leftXPos + 3 * ballRadius * index);
            Canvas.SetTop(greyEllipse, parent.yPos);

            TextBlock greyText = new TextBlock
            {
                Text = parent.Numbers[index].ToString(),
                FontSize = ballRadius / 3 * 2,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                TextAlignment = TextAlignment.Center
            };

            greyText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = greyText.DesiredSize.Width;
            double textHeight = greyText.DesiredSize.Height;

            Canvas.SetLeft(greyText, parent.leftXPos + 3 * ballRadius * index + ballRadius - textWidth / 2);
            Canvas.SetTop(greyText, parent.yPos + ballRadius - textHeight / 2);

            _canvas.Children.Add(greyEllipse);
            _canvas.Children.Add(greyText);



            Ellipse ellipse = new Ellipse
            {
                Width = ballRadius * 2,
                Height = ballRadius * 2,
                Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                Stroke = new SolidColorBrush(Color.FromRgb(166, 92, 66)),
                StrokeThickness = 0.1 * ballRadius
            };

            Canvas.SetLeft(ellipse, parent.leftXPos + 3*ballRadius*index);
            Canvas.SetTop(ellipse, parent.yPos);

            TextBlock numberText = new TextBlock
            {
                Text = parent.Numbers[index].ToString(),
                FontSize = ballRadius / 3 * 2,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                TextAlignment = TextAlignment.Center
            };


            Canvas.SetLeft(numberText, parent.leftXPos + 3 * ballRadius * index + ballRadius - textWidth / 2);
            Canvas.SetTop(numberText, parent.yPos + ballRadius - textHeight / 2);

            _canvas.Children.Add(ellipse);
            _canvas.Children.Add(numberText);


            DoubleAnimation moveBallY = new DoubleAnimation
            {
                From = parent.yPos,
                To = yPos,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveBallX = new DoubleAnimation
            {
                From = parent.leftXPos + index * 3 *ballRadius,
                To = _xPos,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(moveBallX, ellipse);
            Storyboard.SetTarget(moveBallY, ellipse);
            Storyboard.SetTargetProperty(moveBallX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(moveBallY, new PropertyPath(Canvas.TopProperty));


            DoubleAnimation moveTextY = new DoubleAnimation
            {
                From = parent.yPos + ballRadius - textHeight / 2,
                To = yPos + ballRadius - textHeight / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveTextX = new DoubleAnimation
            {
                From = parent.leftXPos + index*3*ballRadius + ballRadius - textWidth / 2,
                To = _xPos + ballRadius - textWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(moveTextX, numberText);
            Storyboard.SetTarget(moveTextY, numberText);
            Storyboard.SetTargetProperty(moveTextX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(moveTextY, new PropertyPath(Canvas.TopProperty));

            _storyboard.Children.Add(moveTextX);
            _storyboard.Children.Add(moveTextY);
            _storyboard.Children.Add(moveBallX);
            _storyboard.Children.Add(moveBallY);

            if (comparisonBefore)
            {

                var lastThreePaths = _canvas.Children.OfType<Path>().Reverse().Take(3).ToList();  

                foreach (var path in lastThreePaths)
                {
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = animationSeconds,
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                    };

                    fadeOutAnimation.Completed += (s, e) => _canvas.Children.Remove(path);

                    Storyboard.SetTarget(fadeOutAnimation, path);
                    Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

                    _storyboard.Children.Add(fadeOutAnimation);
                }

                var symbols = _canvas.Children.OfType<TextBlock>()
                    .Where(tb => tb.Text == "<" || tb.Text == ">")
                    .ToList();

                foreach (var symbol in symbols)
                {

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = animationSeconds,  
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                    };

                    fadeOutAnimation.Completed += (s, e) => _canvas.Children.Remove(symbol);

                    Storyboard.SetTarget(fadeOutAnimation, symbol);
                    Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

                    _storyboard.Children.Add(fadeOutAnimation);
                }
            }


            _storyboard.Begin();
        }
        private void ComparisonDraw(int indexA, int indexB, Node a, char symbol)
        {
            _storyboard.Children.Clear();
            double ballRadius = _ballRadius;
            DrawSmoothLine(new Point(a.LeftParent.leftXPos + 3 * ballRadius * indexA+ballRadius, a.LeftParent.yPos + 2 * ballRadius),
                           new Point(a.LeftParent.leftXPos + 3 * ballRadius * indexA+ballRadius, a.LeftParent.yPos + 2 * ballRadius + 0.5 * _verticalGap * ballRadius),
                           ballRadius, fadeIn: true, new SolidColorBrush(Color.FromRgb(100, 150, 255)));
            DrawSmoothLine(new Point(a.RightParent.leftXPos + 3 * ballRadius * indexB+ballRadius, a.RightParent.yPos + 2 * ballRadius),
                           new Point(a.RightParent.leftXPos + 3 * ballRadius * indexB + ballRadius, a.RightParent.yPos + 2 * ballRadius + 0.5 * _verticalGap * ballRadius),
                            ballRadius, fadeIn: true, new SolidColorBrush(Color.FromRgb(100, 150, 255)));
            DrawSmoothLine(new Point(a.LeftParent.leftXPos + 3 * ballRadius * indexA + ballRadius, a.LeftParent.yPos + 2 * ballRadius + 0.5 * _verticalGap * ballRadius),
                           new Point(a.RightParent.leftXPos + 3 * ballRadius * indexB + ballRadius, a.RightParent.yPos + 2 * ballRadius + 0.5 * _verticalGap * ballRadius),
                           ballRadius, fadeIn: true, new SolidColorBrush(Color.FromRgb(100, 150, 255)));


            TextBlock charText = new TextBlock
            {
                Text = symbol.ToString(),
                FontSize = ballRadius / 3 * 2,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                TextAlignment = TextAlignment.Center,
                Opacity = 0
            };

            DoubleAnimation fadeInAnimationText = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = AnimationSeconds / 2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                BeginTime = AnimationSeconds / 2
            };
            Storyboard.SetTarget(fadeInAnimationText, charText);
            Storyboard.SetTargetProperty(fadeInAnimationText, new PropertyPath(UIElement.OpacityProperty));


            charText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = charText.DesiredSize.Width;
            double textHeight = charText.DesiredSize.Height;

            Canvas.SetLeft(charText, (a.LeftParent.leftXPos + a.RightParent.leftXPos + 3 * ballRadius * indexA + 3 * ballRadius * indexB)/2 + ballRadius - textWidth / 2);
            Canvas.SetTop(charText, a.LeftParent.yPos + ballRadius + 0.7 * _verticalGap * ballRadius + ballRadius - textHeight / 2);

            _canvas.Children.Add(charText);
            _storyboard.Children.Add(fadeInAnimationText);
            _storyboard.Begin();
        }
        private void SingleParentAnimation(Node a, Node son)
        {
            _storyboard.Children.Clear();
            double ballRadius = _ballRadius;
            int number = a.Numbers[0];
            double yPos = ballRadius * ((a.VerticalLayer + 1 + 1) * _verticalGap + 2 * (a.VerticalLayer + 1));
            TimeSpan animationSeconds = AnimationSeconds;
            son.yPos = yPos;
            son.middleXPos = _xPos + ballRadius * (3 * son.Numbers.Count - 1) / 2;
            son.leftXPos = _xPos;


            Ellipse ellipse = new Ellipse
            {
                Width = ballRadius * 2,
                Height = ballRadius * 2,
                Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                Stroke = new SolidColorBrush(Color.FromRgb(166, 92, 66)),
                StrokeThickness = 0.1 * ballRadius
            };

            TextBlock numberText = new TextBlock
            {
                Text = number.ToString(),
                FontSize = ballRadius / 3 * 2,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                TextAlignment = TextAlignment.Center
            };

            numberText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = numberText.DesiredSize.Width;
            double textHeight = numberText.DesiredSize.Height;

            Canvas.SetLeft(ellipse, _xPos);
            Canvas.SetTop(ellipse, yPos);

            Canvas.SetLeft(numberText, _xPos + ballRadius - textWidth / 2);
            Canvas.SetTop(numberText, yPos + ballRadius - textHeight / 2);

            _canvas.Children.Add(ellipse);
            _canvas.Children.Add(numberText);

            DoubleAnimation moveBallY = new DoubleAnimation
            {
                From = a.yPos,
                To = yPos,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveBallX = new DoubleAnimation
            {
                From = a.leftXPos,
                To = _xPos,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(moveBallX, ellipse);
            Storyboard.SetTarget(moveBallY, ellipse);
            Storyboard.SetTargetProperty(moveBallX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(moveBallY, new PropertyPath(Canvas.TopProperty));


            DoubleAnimation moveTextY = new DoubleAnimation
            {
                From = a.yPos + ballRadius - textHeight / 2,
                To = yPos + ballRadius - textHeight / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveTextX = new DoubleAnimation
            {
                From = a.leftXPos + ballRadius - textWidth/2,
                To = _xPos + ballRadius - textWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(moveTextX, numberText);
            Storyboard.SetTarget(moveTextY, numberText);
            Storyboard.SetTargetProperty(moveTextX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(moveTextY, new PropertyPath(Canvas.TopProperty));


            var transparentEllipses = _canvas.Children.OfType<Rectangle>()
                .Where(e => (e.Fill as SolidColorBrush)?.Color == Colors.Transparent)
                .ToList();

            foreach (var e in transparentEllipses)
            {
                DoubleAnimation fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = animationSeconds,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(fadeOutAnimation, e);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));
                fadeOutAnimation.Completed += (s, args) =>
                {
                    _canvas.Children.Remove(e); 
                };

                _storyboard.Children.Add(fadeOutAnimation);
            }

            _storyboard.Begin();

            _storyboard.Children.Add(moveTextX);
            _storyboard.Children.Add(moveTextY);
            _storyboard.Children.Add(moveBallX);
            _storyboard.Children.Add(moveBallY);

            DrawSmoothLine(new Point(son.middleXPos, son.yPos), new Point(a.middleXPos, a.yPos + 2 * ballRadius), ballRadius, fadeIn: true, isNotFullLine: true);
            _storyboard.Begin();
        }
        private void HighlightTwoBalls(int indexA, int indexB, Node a)
        {
            double ballRadius = _ballRadius;
            HighlightSingleBall(a.LeftParent.leftXPos + indexA * 3*ballRadius, a.LeftParent.yPos, ballRadius, Brushes.Red);
            HighlightSingleBall(a.RightParent.leftXPos + indexB * 3 * ballRadius, a.RightParent.yPos, ballRadius, Brushes.Red);

        }

        private void HighlightParentNode(double x, double y, int number, double ballRadius)
        {
            Rectangle rect = new Rectangle
            {
                Width = ballRadius * (3 * number - 1),
                Height = 2 * ballRadius,
                Stroke = new SolidColorBrush(Color.FromRgb(255, 210, 100)),
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            _canvas.Children.Add(rect);


        }





        private void HighlightSingleBall(double x, double y, double ballRadius, SolidColorBrush color)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 2 * ballRadius,
                Height = 2 * ballRadius,
                Stroke = color,
                StrokeThickness = 0.1 * ballRadius,
                Fill = Brushes.Transparent
            };

            Canvas.SetTop(ellipse, y);
            Canvas.SetLeft(ellipse, x);

            _canvas.Children.Add(ellipse);
        }

        private void HighlightSingleParentNode(Node a, bool delAll = false)
        {
            if (delAll) _canvas.Children
                    .OfType<Rectangle>()
                    .Where(e => (e.Fill as SolidColorBrush)?.Color == Colors.Transparent)
                    .ToList()
                    .ForEach(e => _canvas.Children
                    .Remove(e));
            double ballRadius = _ballRadius;
            HighlightParentNode(a.leftXPos, a.yPos, a.Numbers.Count, ballRadius);
        }

        private void HighlightParents(Node a, Node b)
        {
            HighlightSingleParentNode(a);
            HighlightSingleParentNode(b);
        }

        private void HighlightSingleNode(Node a, bool delAll = false)
        {
            if (delAll) _canvas.Children
                    .OfType<Ellipse>()
                    .Where(e => (e.Fill as SolidColorBrush)?.Color == Colors.Transparent)
                    .ToList()
                    .ForEach(e => _canvas.Children
                    .Remove(e));
            double ballRadius = _ballRadius;
            double xPos = a.leftXPos;
            for (int i = 0; i < a.Numbers.Count; i++)
            {
                HighlightSingleBall(xPos, a.yPos, ballRadius, new SolidColorBrush(Color.FromRgb(255, 210, 100)));
                xPos += 3 * ballRadius;
            }
        }
        private void Highlight(Node leftParent, Node rightParent)
        {
            HighlightSingleNode(leftParent);
            HighlightSingleNode(rightParent);
        }
        public void PreRun()
        {
            Node firstNode = new Node(_numbers, isFake: false);
            splitLayers[0] = new List<Node> { firstNode };
            MergeSort(firstNode);
            splitLayers.Remove(splitLayers.Keys.Max());

            Queue<Tuple<int, Node>> fakeNodes = new Queue<Tuple<int, Node>>();

            foreach (var kvp in splitLayers)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    Node node = kvp.Value[i];
                    if (node.VerticalLayer != -1)
                    {
                        Node newNode = new Node(node.Numbers, isFake: true);
                        newNode.LeftChild = node.LeftChild;
                        newNode.RightChild = node.RightChild;

                        newNode.LeftParent = node;

                        if (node.LeftChild != null)
                        {
                            if (node.LeftChild.LeftParent == node)
                            {
                                node.LeftChild.LeftParent = newNode;
                            }
                            else if (node.LeftChild.RightParent == node)
                            {
                                node.LeftChild.RightParent = newNode;
                            }
                        }
                        if (node.RightChild != null)
                        {
                            if (node.RightChild.LeftParent == node)
                            {
                                node.RightChild.LeftParent = newNode;
                            }
                            else if (node.RightChild.RightParent == node)
                            {
                                node.RightChild.RightParent = newNode;
                            }
                        }
                        node.LeftChild = newNode;
                        node.RightChild = null;

                        newNode.VerticalLayer = kvp.Key;
                        fakeNodes.Enqueue(new Tuple<int, Node> ( i, newNode ));
                        kvp.Value[i] = newNode;

                    } else
                    {
                      node.VerticalLayer = kvp.Key;
                    }
                }
            }
            //MessageBox.Show(string.Join(',', fakeNodes.Select(tup => $"{tup.Item1} : {string.Join(' ', tup.Item2.Numbers)}")));
            maxSplitLayerIndex = splitLayers.Keys.Max();
            int e = 0;
            int k = 0;
            foreach (var kvp in  mergeLayers.OrderByDescending(kvp => kvp.Key))
            {
                maxSplitLayerIndex++;
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    Node node = kvp.Value[i];
                    while (fakeNodes.Count > 0)
                    {
                        e = 0;
                        k = 0;
                        var currTuple = fakeNodes.Dequeue();
                        for (int j = 0; j < kvp.Value.Count;j++)
                        {
                            if (currTuple.Item1 == e)
                            {
                                //MessageBox.Show($"inserted {string.Join(' ', currTuple.Item2.Numbers)} at {e}");
                                Node fkNode = currTuple.Item2;
                                Node newNode = new Node(fkNode.Numbers, isFake: false);
                                newNode.LeftChild = fkNode.LeftChild;
                                newNode.RightChild = fkNode.RightChild;

                                if (fkNode.LeftChild != null)
                                {
                                    if (fkNode.LeftChild.LeftParent == fkNode)
                                    {
                                        fkNode.LeftChild.LeftParent = newNode;
                                    }
                                    else if (fkNode.LeftChild.RightParent == fkNode)
                                    {
                                        fkNode.LeftChild.RightParent = newNode;
                                    }
                                }
                                if (fkNode.RightChild != null)
                                {
                                    if (fkNode.RightChild.LeftParent == fkNode)
                                    {
                                        fkNode.RightChild.LeftParent = newNode;
                                    }
                                    else if (fkNode.RightChild.RightParent == fkNode)
                                    {
                                        fkNode.RightChild.RightParent = newNode;
                                    }
                                }

                                fkNode.LeftChild = newNode;
                                fkNode.RightChild = null;

                                newNode.LeftParent = fkNode;
                                newNode.VerticalLayer = maxSplitLayerIndex;
                                kvp.Value.Insert(e-k, newNode);
                                //MessageBox.Show($"Changed to: {string.Join(' ', kvp.Value.Select(e => $"({string.Join(' ', e.Numbers)})"))}");
                                e++;
                                break;
                            } else
                            {
                                e += kvp.Value[j].Numbers.Count;
                                k += kvp.Value[j].Numbers.Count - 1;
                            }
                        }
                    }
                    
                    node.VerticalLayer = maxSplitLayerIndex;
                }
            }

            foreach (var kvp in mergeLayers)
            {
                double xPos = (_canvas.ActualWidth - _ballRadius *
                    (3 * (kvp.Value.Sum(innerList => innerList.Numbers.Count) * 2
                    - kvp.Value.Last().Numbers.Count) - 1)) / 2;
                
                double yPos = _ballRadius * ((kvp.Value[0].VerticalLayer + 1) * _verticalGap + 2 * kvp.Value[0].VerticalLayer);

                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    Node currNode = kvp.Value[i];
                    currNode.yPos = yPos;
                    currNode.middleXPos = xPos + _ballRadius * (3 * currNode.Numbers.Count - 1) / 2;
                    currNode.leftXPos = xPos;
                    //MessageBox.Show($"{string.Join(' ', currNode.Numbers)} : {currNode.yPos}");
                    for (int j = 0; j < currNode.Numbers.Count; j++)
                    {
                        xPos += 3 * _ballRadius;
                    }
                    xPos += _ballRadius * (3 * currNode.Numbers.Count);

                }
            }

            //DisplayTreeStructure();
        }
        public void OnSelect()
        {
            PreRun();
            InitDraw();
        }
        private void InitDraw()
        {
            _canvas.Children.Clear();
            double ballRadius = _ballRadius;
            double xPos = (_canvas.ActualWidth - ballRadius *
                    (3 * _numbers.Count - 1)) / 2;
            double yPos = ballRadius * (_verticalGap);

            foreach (int number in _numbers)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = ballRadius * 2,
                    Height = ballRadius * 2,
                    Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                    Stroke = new SolidColorBrush(Color.FromRgb(166, 92, 66)),
                    StrokeThickness = 0.1 * ballRadius
                };

                Canvas.SetLeft(ellipse, xPos);
                Canvas.SetTop(ellipse, yPos);

                TextBlock numberText = new TextBlock
                {
                    Text = number.ToString(),
                    FontSize = ballRadius / 3 * 2,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                    TextAlignment = TextAlignment.Center
                };

                numberText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = numberText.DesiredSize.Width;
                double textHeight = numberText.DesiredSize.Height;

                Canvas.SetLeft(numberText, xPos + ballRadius - textWidth / 2);
                Canvas.SetTop(numberText, yPos + ballRadius - textHeight / 2);

                _canvas.Children.Add(ellipse);
                _canvas.Children.Add(numberText);
                xPos += 3 * ballRadius;
            }
        }
        private void DrawSplitLayers(int layer)
        {

            _canvas.Children.Clear();
            double ballRadius = _ballRadius;
            TimeSpan animationSeconds = AnimationSeconds;

            for (int i = 0; i <= layer; i++)
            {
                List<Node> currentLayerNodes = splitLayers[i];
                double xPos = (_canvas.ActualWidth - ballRadius * 
                    (3 * (currentLayerNodes.Sum(innerList => innerList.Numbers.Count) * 2
                    - currentLayerNodes.Last().Numbers.Count) - 1)) / 2;
                double yPos = ballRadius * ((i+1)*_verticalGap + 2*i);

                _storyboard.Children.Clear();
                for (int j = 0; j < currentLayerNodes.Count; j++)
                {
                    Node currNode = currentLayerNodes[j];
                    currNode.yPos = yPos;
                    currNode.middleXPos = xPos + ballRadius * (3 * currNode.Numbers.Count - 1) / 2;
                    currNode.leftXPos = xPos;

                    if (currNode.LeftParent != null && currNode.VerticalLayer == i)
                    {
                        DrawSmoothLine(new Point(currNode.middleXPos, currNode.yPos),
                            new Point(currNode.LeftParent.middleXPos, currNode.LeftParent.yPos + 2*ballRadius),
                            ballRadius, fadeIn: (i == layer), isNotFullLine: (currNode.IsFake || currNode.LeftParent.IsFake));
                    }

                    for (int k = 0; k < currNode.Numbers.Count; k++)
                    {
                        int number = currNode.Numbers[k];
                        if (currNode.VerticalLayer == i)
                        {
                            
                            Ellipse ellipse = new Ellipse
                            {
                                Width = ballRadius * 2,
                                Height = ballRadius * 2,
                                Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                                Stroke = new SolidColorBrush(Color.FromRgb(166, 92, 66)),
                                StrokeThickness = 0.1 * ballRadius,//zavisle na velikosti r
                                Opacity = (i == layer) ? 0 : 1
                            };

                            Canvas.SetLeft(ellipse, xPos);
                            Canvas.SetTop(ellipse, yPos);

                            TextBlock numberText = new TextBlock
                            {
                                Text = number.ToString(),
                                FontSize = ballRadius / 3 * 2,
                                FontWeight = FontWeights.Bold,
                                Foreground = new SolidColorBrush(Color.FromRgb(216, 216, 216)),
                                TextAlignment = TextAlignment.Center,
                                Opacity = (i == layer) ? 0 : 1
                            };

                            numberText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            double textWidth = numberText.DesiredSize.Width;
                            double textHeight = numberText.DesiredSize.Height;

                            Canvas.SetLeft(numberText, xPos + ballRadius - textWidth / 2);
                            Canvas.SetTop(numberText, yPos + ballRadius - textHeight / 2);

                            _canvas.Children.Add(ellipse);
                            _canvas.Children.Add(numberText);

                            if (i == layer && currNode.LeftParent != null)
                            {
                                DoubleAnimation fadeInAnimationBall = new DoubleAnimation
                                {
                                    From = 0,
                                    To = 1,
                                    Duration = animationSeconds/4 * 3,
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                                    BeginTime = animationSeconds/4
                                };
                                Storyboard.SetTarget(fadeInAnimationBall, ellipse);
                                Storyboard.SetTargetProperty(fadeInAnimationBall, new PropertyPath(UIElement.OpacityProperty));

                                DoubleAnimation fadeInAnimationText = new DoubleAnimation
                                {
                                    From = 0,
                                    To = 1,
                                    Duration = animationSeconds/4 * 3,
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                                    BeginTime = animationSeconds/4
                                };
                                Storyboard.SetTarget(fadeInAnimationText, numberText);
                                Storyboard.SetTargetProperty(fadeInAnimationText, new PropertyPath(UIElement.OpacityProperty));

                                DoubleAnimation moveBallY = new DoubleAnimation
                                {
                                    From = currNode.LeftParent.yPos,
                                    To = currNode.yPos,
                                    Duration = animationSeconds,
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                                };

                                DoubleAnimation moveBallX = new DoubleAnimation
                                {
                                    From = (currNode.LeftParent.LeftChild == currNode) ? 
                                    currNode.LeftParent.leftXPos + ballRadius*(3*k - 1) :
                                    currNode.LeftParent.leftXPos + ballRadius * (3 * Math.Floor((double)currNode.LeftParent.Numbers.Count/2) - 1)  + ballRadius * (3 * k - 1),
                                    To = xPos,
                                    Duration = animationSeconds,
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                                };




                                DoubleAnimation moveTextY = new DoubleAnimation
                                {
                                    From = currNode.LeftParent.yPos + ballRadius - textHeight/2,
                                    To = currNode.yPos + ballRadius - textHeight / 2,
                                    Duration = animationSeconds,
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                                };

                                DoubleAnimation moveTextX = new DoubleAnimation
                                {
                                    From = (currNode.LeftParent.LeftChild == currNode) ?
                                    currNode.LeftParent.leftXPos + ballRadius * (3 * k - 1) + ballRadius - textWidth / 2 :
                                    currNode.LeftParent.leftXPos + ballRadius * (3 * Math.Floor((double)currNode.LeftParent.Numbers.Count / 2) - 1) + ballRadius * (3 * k - 1) + ballRadius - textWidth/2,
                                    To = xPos + ballRadius - textWidth/2,
                                    Duration = animationSeconds,
                                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                                }; 

                                Storyboard.SetTarget(moveBallX, ellipse);
                                Storyboard.SetTarget(moveBallY, ellipse);
                                Storyboard.SetTargetProperty(moveBallX, new PropertyPath(Canvas.LeftProperty));
                                Storyboard.SetTargetProperty(moveBallY, new PropertyPath(Canvas.TopProperty));

                                Storyboard.SetTarget(moveTextX, numberText);
                                Storyboard.SetTarget(moveTextY, numberText);
                                Storyboard.SetTargetProperty(moveTextX, new PropertyPath(Canvas.LeftProperty));
                                Storyboard.SetTargetProperty(moveTextY, new PropertyPath(Canvas.TopProperty));

                                _storyboard.Children.Add(moveBallX);
                                _storyboard.Children.Add(moveBallY);
                                _storyboard.Children.Add(moveTextX);
                                _storyboard.Children.Add(moveTextY);
                                _storyboard.Children.Add(fadeInAnimationText);
                                _storyboard.Children.Add(fadeInAnimationBall);
                            }
                        }
                        xPos += 3 * ballRadius;
                    }
                    xPos += ballRadius * (3 * currentLayerNodes[j].Numbers.Count);

                }
                _storyboard.Begin();
            }
        }
        private void DrawSmoothLine(Point start, Point end, double ballRadius, bool fadeIn, SolidColorBrush color = null, bool isNotFullLine = false)
        {
            color ??= Brushes.White;
            var path = new Path
            {
                Stroke = color,
                StrokeThickness = 0.07 * ballRadius,
                SnapsToDevicePixels = true,
                StrokeLineJoin = PenLineJoin.Round,
                Opacity = fadeIn ? 0 : 1,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
            {
                new PathFigure
                {
                    StartPoint = start,
                    Segments = new PathSegmentCollection
                    {
                        new LineSegment(end, true)
                    }
                }
            }
                }
            };

       
            if (isNotFullLine)
            {
                path.StrokeDashArray = new DoubleCollection { ballRadius/7, ballRadius/5 };
                path.StrokeDashCap = PenLineCap.Round;
            }

            RenderOptions.SetEdgeMode(path, EdgeMode.Unspecified);

            if (fadeIn)
            {
                TimeSpan animationSeconds = AnimationSeconds;
                DoubleAnimation fadeInAnimationPath = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = animationSeconds / 2,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                    BeginTime = animationSeconds / 2
                };
                Storyboard.SetTarget(fadeInAnimationPath, path);
                Storyboard.SetTargetProperty(fadeInAnimationPath, new PropertyPath(UIElement.OpacityProperty));
                _storyboard.Children.Add(fadeInAnimationPath);
            }

            _canvas.Children.Add(path);
        }

        private void DisplayTreeStructure()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("Split Layers:");
            foreach (var layer in splitLayers.OrderBy(l => l.Key))
            {
                result.AppendLine($"Level {layer.Key}:");
                foreach (var node in layer.Value)
                {
                    result.AppendLine($"  Node: {node}");
                }
            }

            result.AppendLine("\nMerge Layers:");
            foreach (var layer in mergeLayers.OrderBy(l => l.Key))
            {
                result.AppendLine($"Level {layer.Key}:");
                foreach (var node in layer.Value)
                {
                    result.AppendLine($"  Node: {node}");
                }
            }

            MessageBox.Show(result.ToString(), "Tree Structure");
        }
       

        private Node MergeSort(Node currNode, int level = 1)
        {
            if (currNode.Numbers.Count <= 1)
            {
                if (!splitLayers.ContainsKey(level))
                    splitLayers[level] = new List<Node>();
                splitLayers[level].Add(currNode);
                return currNode;
            }

            int mid = currNode.Numbers.Count / 2;
            List<int> listOne = currNode.Numbers.GetRange(0, mid);
            List<int> listTwo = currNode.Numbers.GetRange(mid, currNode.Numbers.Count - mid);

            Node leftChild = new Node(listOne, isFake: false);
            Node rightChild = new Node(listTwo, isFake: false);

            leftChild.LeftParent = currNode;
            rightChild.LeftParent = currNode;
            currNode.LeftChild = leftChild;
            currNode.RightChild = rightChild;

            if (!splitLayers.ContainsKey(level))
                splitLayers[level] = new List<Node>();

            splitLayers[level].Add(leftChild);
            splitLayers[level].Add(rightChild);

            leftChild = MergeSort(leftChild, level + 1);
            rightChild = MergeSort(rightChild, level + 1);

            return Merge(leftChild, rightChild, currNode, level);
        }
        private Node Merge(Node a, Node b, Node parent, int level)
        {
            List<int> mergedNumbers = new List<int>();
            int aIndex = 0, bIndex = 0;

            while (aIndex < a.Numbers.Count && bIndex < b.Numbers.Count)
            {
                if (a.Numbers[aIndex] > b.Numbers[bIndex])
                {
                    mergedNumbers.Add(b.Numbers[bIndex]);
                    bIndex++;
                }
                else
                {
                    mergedNumbers.Add(a.Numbers[aIndex]);
                    aIndex++;
                }
            }
            mergedNumbers.AddRange(a.Numbers.Skip(aIndex));
            mergedNumbers.AddRange(b.Numbers.Skip(bIndex));

            Node mergedNode = new Node(mergedNumbers, isFake: false);
            mergedNode.LeftParent = a;
            mergedNode.RightParent = b;
            
            a.LeftChild = mergedNode;
            b.LeftChild = mergedNode;

            if (!mergeLayers.ContainsKey(level))
                mergeLayers[level] = new List<Node>();
            mergeLayers[level].Add(mergedNode);

            return mergedNode;
        }

        private class Node
        {
            public int Id;
            public List<int> Numbers;
            public Node? LeftChild;
            public Node? RightChild;
            public Node? LeftParent;
            public Node? RightParent;
            public bool IsFake; //useless
            public int VerticalLayer;
            public double middleXPos;
            public double leftXPos;
            public double yPos;

            public Node(List<int> numbers, bool isFake)
            {
                Numbers = numbers; IsFake = isFake; VerticalLayer = -1;
            }

            public override string ToString()
            {
                string leftChildNumbers = LeftChild != null ? string.Join(' ', LeftChild.Numbers) : "----";
                string rightChildNumbers = RightChild != null ? string.Join(' ', RightChild.Numbers) : "----";
                string leftParentNumbers = LeftParent != null ? string.Join(' ', LeftParent.Numbers) : "----";
                string rightParentNumbers = RightParent != null ? string.Join(' ', RightParent.Numbers) : "----";

                return $"(Left Parent: {leftParentNumbers}), (Right Parent: {rightParentNumbers}), " +
                       $"(Left Child: {leftChildNumbers}), (Right Child: {rightChildNumbers}) -> {string.Join(' ', Numbers)} (level {VerticalLayer})";
            }


        }


    }
}
