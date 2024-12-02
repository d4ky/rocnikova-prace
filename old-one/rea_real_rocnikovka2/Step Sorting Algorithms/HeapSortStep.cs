using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace rea_real_rocnikovka2.Step_Sorting_Algorithms
{
    public class HeapSortStep
    {
        //fixnout drawcomp
        private List<int> _numbers;
        private Canvas _canvas;
        private Func<double> _getAnimationSeconds;
        private TimeSpan AnimationSeconds => TimeSpan.FromMilliseconds(_getAnimationSeconds());
        private int NumOfLayers;
        private double _verticalGap = 2;
        private double _ballRadius => Math.Min(_canvas.ActualWidth / ((3 * 2* Math.Pow(2, NumOfLayers -1)) + 1), _canvas.ActualHeight / (2 * NumOfLayers + 2 + _verticalGap * (NumOfLayers + 2)));
        private double _fakeBallRadius => _ballRadius * 0.7;
        private Dictionary<int, Ellipse> TopLineBalls = new Dictionary<int, Ellipse>();
        private Dictionary<int, TextBlock> TopLineText = new Dictionary<int, TextBlock>();
        private Dictionary<int, Point> TreeBallPos = new Dictionary<int, Point>();
        private Dictionary<int, Ellipse> TreeBall = new Dictionary<int, Ellipse>();
        private Dictionary<int, TextBlock> TreeText = new Dictionary<int, TextBlock>();
        private int numDone;
        public Storyboard _storyboard;
        private int _stepState;
        private bool _isSorted;
        public bool IsSorted => _isSorted;
        public bool midWayAnimationIsRunning;
        public bool neverAgain;

        public HeapSortStep(List<int> numbers, Canvas canvas, Func<double> getAnimationStepSpeed)
        {
            _numbers = numbers;
            _canvas = canvas;
            _getAnimationSeconds = getAnimationStepSpeed;
            _currIndex = 0;
            numDone = 0;
            _firstTime = true;
            _isBuildingTree = true;
            _isSorted = false;
            _storyboard = new Storyboard();
            NumOfLayers = (int)Math.Ceiling(Math.Log2(_numbers.Count+1));
        }

        public void Reset(List<int> numbers)
        {
            _numbers = numbers;
            TopLineBalls.Clear();
            TopLineText.Clear();
            TopLineBalls.Clear();
        }

        public void OnSelect()
        {
            DrawComponenets();
        }

        private bool _isBuildingTree;
        private int _currIndex;
        private bool _firstTime;
        private bool _performedSwap;
        public async void Step()
        {

            DrawComponenets();
            if (!_isSorted)
            {
                if (_isBuildingTree)
                {
                    switch (_stepState)
                    {
                        case 0:
                            SelectTopLineBall(_currIndex, Color.FromRgb(255, 210, 100));
                            //selectnu currindex-ty prvek bravou
                            _stepState = 1;
                            //_stepState= 1

                            break;
                        case 1:
                            SendCopyDown(_currIndex);
                            //poslu ho na sve misto
                            _stepState = 0;
                            _currIndex++;
                            if (_currIndex >= _numbers.Count)
                            {
                                await Task.Delay(AnimationSeconds);
                                AnimateTopLineLeaving();
                                midWayAnimationIsRunning = true;
                                _isBuildingTree = false;
                                _currIndex = _numbers.Count - 1;
                            }
                            //_stepstate = 0
                            // curr index ++
                            break;
                    }
                }
                else
                {
                    switch (_stepState)
                    {
                        case 0:
                            //pokud currindex je mensi nez 0, tak oznacim last a first, stepState = 2 (swap) s true takze pujde se ve strome odstrani a v topline obarvi zelene
                            //oznacim child a parenta jak ve strome, tak v topline
                            SelectTopLineBall(_currIndex, Color.FromRgb(255, 210, 100));
                            if (((int)Math.Floor((double)(_currIndex - 1) / 2) + 1) * 2 == _currIndex || _performedSwap)
                                SelectTopLineBall((int)Math.Floor((double)(_currIndex - 1) / 2), Color.FromRgb(255, 210, 100));
                            else
                                SelectTopLineBall((int)Math.Floor((double)(_currIndex - 1) / 2), Color.FromRgb(255, 210, 100), false);
                            if (_performedSwap)
                                _performedSwap = false;
                            _stepState = 1;
                            break;
                        case 1:
                            SelectTopLineBall(_currIndex, Color.FromRgb(255, 210, 100), fadeIn: false);
                            SelectTopLineBall((int)Math.Floor((double)(_currIndex - 1) / 2), Color.FromRgb(255, 210, 100), fadeIn: false);
                            //furt budou oznaceni (oznacim znovu ale bez animace)
                            //parent vetsi nez dite = false
                            if (_numbers[_currIndex] > _numbers[(int)Math.Floor((double)(_currIndex - 1) / 2)])
                            {
                                DrawComparison(_currIndex, (int)Math.Floor((double)(_currIndex - 1) / 2), false);
                                _stepState = 2;
                                //swap
                            }
                            else
                            {
                                if (_numbers[_currIndex] == _numbers[(int)Math.Floor((double)(_currIndex - 1) / 2)])
                                {
                                    DrawComparison(_currIndex, (int)Math.Floor((double)(_currIndex - 1) / 2), true, "=");
                                }
                                else
                                {
                                    DrawComparison(_currIndex, (int)Math.Floor((double)(_currIndex - 1) / 2), true);
                                }
                                _stepState = 0;
                                if (_currIndex - 1 < 1)
                                {
                                    _stepState = 3;
                                }
                                else
                                {
                                    _currIndex--; //pokud curr index je mensi nez 0 tak jdeme swapovat hlavni s zadnim currstate = 3
                                }


                            }

                            //da se mezi ne porovnavatko a cara (jen ve strome)
                            //pokud je parent mensi nez dite tak _stepState = 2 (swap)
                            //pokud je parent vetsi nez dite tak _StepState = 1, _currindex --;
                            break;
                        case 2:
                            //SwapInTopLine(_currIndex, (int)Math.Floor((double)(_currIndex - 1) / 2));
                            SwapInTree(_currIndex, (int)Math.Floor((double)(_currIndex - 1) / 2));
                            _performedSwap = true;
                            //swapne jak na strome tak v topline

                            _stepState = 1;
                            if (_currIndex - 1 < 1)
                            {
                                _stepState = 3;

                            }
                            else
                            {
                                _currIndex--; //pokud curr index je mensi nez 0 tak jdeme swapovat hlavni s zadnim currstate = 3
                            } //pokud curr index je mensi nez 0 tak jdeme swapovat hlavni s zadnim currstate = 3
                            break;
                        case 3:
                            //swapne horni s dolnim ve strome i topline
                            //vybarvi v topline horni
                            if (_firstTime)
                            {
                                SelectTopLineBall(0, Color.FromRgb(255, 0, 0));
                                SelectTopLineBall(_numbers.Count - 1 - numDone, Color.FromRgb(255, 0, 0));
                                _firstTime = false;
                            }
                            else
                            {
                                SelectTopLineBall(0, Color.FromRgb(255, 0, 0), false);
                                SelectTopLineBall(_numbers.Count - 1 - numDone, Color.FromRgb(255, 0, 0), false);
                                RemoveFromTree(0, _numbers.Count - 1 - numDone);
                                _firstTime = true;
                                _stepState = 0;
                                numDone++;
                                if (_numbers.Count - numDone == 1)
                                {
                                    //last one hopity hop
                                    _stepState = 4;
                                }
                                else
                                {
                                    _currIndex = _numbers.Count - 1 - numDone;
                                    _stepState = 0;
                                }
                            }
                            //mezistep oznaceni nejdriv
                            //ve strome odstrani dolni s animaci
                            //_stepstate = 0
                            //_currindex--
                            break;
                        case 4:
                            _isSorted = true;
                            SendLastBall();
                            numDone++;
                            break;

                    }
                }
            }
            
        }
        private void SendLastBall()
        {
            Ellipse ball = TreeBall[0];
            TextBlock textBlock = TreeText[0];
            double ballRadius = _ballRadius;
            TimeSpan animationSeconds = AnimationSeconds;

            DoubleAnimation moveBallDown = new DoubleAnimation
            {
                From = Canvas.GetTop(ball),
                To = TreeBallPos[TreeBallPos.Keys.Max()].Y + 2* ballRadius * (_verticalGap + 2),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveBallHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(ball),
                To = (_canvas.ActualWidth - ballRadius * (3 * _numbers.Count - 1)) / 2 + ballRadius * (3 * (_numbers.Count - numDone - 1)),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveBallDown, ball);
            Storyboard.SetTargetProperty(moveBallDown, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveBallHorizontal, ball);
            Storyboard.SetTargetProperty(moveBallHorizontal, new PropertyPath(Canvas.LeftProperty));


            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = textBlock.DesiredSize.Width;
            double textHeight = textBlock.DesiredSize.Height;

            DoubleAnimation moveTextDown = new DoubleAnimation
            {
                From = Canvas.GetTop(textBlock),
                To = TreeBallPos[TreeBallPos.Keys.Max()].Y + 2*ballRadius * (_verticalGap + 2) + ballRadius - textHeight/2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveTextHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(textBlock),
                To = (_canvas.ActualWidth - ballRadius * (3 * _numbers.Count - 1)) / 2 + ballRadius * (3 * (_numbers.Count - numDone - 1)) + ballRadius - textWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveTextDown, textBlock);
            Storyboard.SetTargetProperty(moveTextDown, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveTextHorizontal, textBlock);
            Storyboard.SetTargetProperty(moveTextHorizontal, new PropertyPath(Canvas.LeftProperty));

            ColorAnimation strokeColorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(255, 0, 0),
                To = Color.FromRgb(54, 144, 92),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(strokeColorAnimation, ball);
            Storyboard.SetTargetProperty(strokeColorAnimation, new PropertyPath("Stroke.Color"));

            ColorAnimation fillColorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(216, 122, 86),
                To = Color.FromRgb(118, 174, 91),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(fillColorAnimation, ball);
            Storyboard.SetTargetProperty(fillColorAnimation, new PropertyPath("Fill.Color"));

            _storyboard.Children.Add(strokeColorAnimation);
            _storyboard.Children.Add(fillColorAnimation);
            _storyboard.Children.Add(moveTextDown);
            _storyboard.Children.Add(moveTextHorizontal);
            _storyboard.Children.Add(moveBallDown);
            _storyboard.Children.Add(moveBallHorizontal);
            _storyboard.Begin();
        }

        private void RemoveFromTree(int parentIndex, int childIndex)
        {
            Ellipse child = TreeBall[childIndex];
            Ellipse parent = TreeBall[parentIndex];

            TextBlock childText = TreeText[childIndex];
            TextBlock parentText = TreeText[parentIndex];

            TimeSpan animationSeconds = AnimationSeconds;
            double ballRadius = _ballRadius;

            DoubleAnimation moveChildUp = new DoubleAnimation
            {
                From = Canvas.GetTop(child),
                To = Canvas.GetTop(parent),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveChildHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(child),
                To = Canvas.GetLeft(parent),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveChildUp, child);
            Storyboard.SetTargetProperty(moveChildUp, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveChildHorizontal, child);
            Storyboard.SetTargetProperty(moveChildHorizontal, new PropertyPath(Canvas.LeftProperty));


            DoubleAnimation moveParentDown = new DoubleAnimation
            {
                From = Canvas.GetTop(parent),
                To = TreeBallPos[TreeBallPos.Keys.Max()].Y + 2*ballRadius * (_verticalGap + 2),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveParentHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(parent),
                To = (_canvas.ActualWidth - ballRadius * (3 * _numbers.Count - 1)) / 2 + ballRadius *(3* (_numbers.Count-numDone-1)),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveParentDown, parent);
            Storyboard.SetTargetProperty(moveParentDown, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveParentHorizontal, parent);
            Storyboard.SetTargetProperty(moveParentHorizontal, new PropertyPath(Canvas.LeftProperty));



            childText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double childTextWidth = childText.DesiredSize.Width;
            double childTextheight = childText.DesiredSize.Height;
            parentText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double parentTextWidth = parentText.DesiredSize.Width;
            double parentTextHeight = parentText.DesiredSize.Height;

            DoubleAnimation moveChildTextUp = new DoubleAnimation
            {
                From = Canvas.GetTop(childText),
                To = Canvas.GetTop(parent) + ballRadius - childTextheight / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveChildTextHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(childText),
                To = Canvas.GetLeft(parent) + ballRadius - childTextWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveChildTextUp, childText);
            Storyboard.SetTargetProperty(moveChildTextUp, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveChildTextHorizontal, childText);
            Storyboard.SetTargetProperty(moveChildTextHorizontal, new PropertyPath(Canvas.LeftProperty));

            DoubleAnimation moveParentTextDown = new DoubleAnimation
            {
                From = Canvas.GetTop(parentText),
                To = TreeBallPos[TreeBallPos.Keys.Max()].Y + 2*ballRadius * (_verticalGap + 2) + ballRadius - parentTextHeight / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveParentTextHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(parentText),
                To = (_canvas.ActualWidth - ballRadius * (3 * _numbers.Count - 1)) / 2 + ballRadius * (3 * (_numbers.Count - numDone - 1)) + ballRadius - parentTextWidth / 2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveParentTextDown, parentText);
            Storyboard.SetTargetProperty(moveParentTextDown, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveParentTextHorizontal, parentText);
            Storyboard.SetTargetProperty(moveParentTextHorizontal, new PropertyPath(Canvas.LeftProperty));


            ColorAnimation strokeColorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(255, 0, 0),
                To = Color.FromRgb(54, 144, 92),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(strokeColorAnimation, parent);
            Storyboard.SetTargetProperty(strokeColorAnimation, new PropertyPath("Stroke.Color"));

            ColorAnimation strokeColorAnimationChild = new ColorAnimation
            {
                From = Color.FromRgb(255, 0, 0),
                To = Color.FromRgb(166, 92, 66),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(strokeColorAnimationChild, child);
            Storyboard.SetTargetProperty(strokeColorAnimationChild, new PropertyPath("Stroke.Color"));

            ColorAnimation fillColorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(216, 122, 86),
                To = Color.FromRgb(118, 174, 91),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(fillColorAnimation, parent);
            Storyboard.SetTargetProperty(fillColorAnimation, new PropertyPath("Fill.Color"));

            _storyboard.Children.Add(strokeColorAnimationChild);
            _storyboard.Children.Add(strokeColorAnimation);
            _storyboard.Children.Add(fillColorAnimation);
            _storyboard.Children.Add(moveParentTextDown);
            _storyboard.Children.Add(moveParentTextHorizontal);
            _storyboard.Children.Add(moveChildTextUp);
            _storyboard.Children.Add(moveChildTextHorizontal);
            _storyboard.Children.Add(moveParentDown);
            _storyboard.Children.Add(moveParentHorizontal);
            _storyboard.Children.Add(moveChildUp);
            _storyboard.Children.Add(moveChildHorizontal);

            int temp = _numbers[childIndex];
            _numbers[childIndex] = _numbers[parentIndex];
            _numbers[parentIndex] = temp;

            _storyboard.Begin();
        }

        private void AnimateTopLineLeaving()
        {
            TimeSpan animationSeconds = AnimationSeconds;
            double ballRadius = _ballRadius;
            _storyboard.Children.Clear();
            for (int i = 0; i < _numbers.Count;  i++)
            {
                DoubleAnimation fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = animationSeconds,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(fadeOutAnimation, TopLineBalls[i]);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));

                DoubleAnimation fadeOutAnimationText = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = animationSeconds,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(fadeOutAnimationText, TopLineText[i]);
                Storyboard.SetTargetProperty(fadeOutAnimationText, new PropertyPath(UIElement.OpacityProperty));
                _storyboard.Children.Add(fadeOutAnimation);
                _storyboard.Children.Add(fadeOutAnimationText);
            }

            foreach (UIElement thing in _canvas.Children)
            {
                DoubleAnimation goUpMyLittleStar = new DoubleAnimation
                {
                    From = Canvas.GetTop(thing),
                    To = Canvas.GetTop(thing) - 2 * ballRadius - ballRadius * _verticalGap,
                    Duration = animationSeconds,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                    BeginTime = animationSeconds
                };

                Storyboard.SetTarget(goUpMyLittleStar, thing);
                Storyboard.SetTargetProperty(goUpMyLittleStar, new PropertyPath(Canvas.TopProperty));
                _storyboard.Children.Add(goUpMyLittleStar);
            }
            

            DrawBottomLine(true);
            _storyboard.Begin();
        }

        private void SwapInTree(int childIndex, int parentIndex)
        {
                //_numbers swap taky
            Ellipse child = TreeBall[childIndex];
            Ellipse parent = TreeBall[parentIndex];

            TextBlock childText = TreeText[childIndex];
            TextBlock parentText = TreeText[parentIndex];

            TimeSpan animationSeconds = AnimationSeconds;
            double ballRadius = _ballRadius;

            DoubleAnimation moveChildUp = new DoubleAnimation
            {
                From = Canvas.GetTop(child),
                To = Canvas.GetTop(parent),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveChildHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(child),
                To = Canvas.GetLeft(parent),
                Duration = animationSeconds*2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            

            Storyboard.SetTarget(moveChildUp, child);
            Storyboard.SetTargetProperty(moveChildUp, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveChildHorizontal, child);
            Storyboard.SetTargetProperty(moveChildHorizontal, new PropertyPath(Canvas.LeftProperty));


            DoubleAnimation moveParentDown = new DoubleAnimation
            {
                From = Canvas.GetTop(parent),
                To = Canvas.GetTop(child),
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveParentHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(parent),
                To = Canvas.GetLeft(child),
                Duration = animationSeconds*2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
           

            Storyboard.SetTarget(moveParentDown, parent);
            Storyboard.SetTargetProperty(moveParentDown, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveParentHorizontal, parent);
            Storyboard.SetTargetProperty(moveParentHorizontal, new PropertyPath(Canvas.LeftProperty));



            childText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double childTextWidth = childText.DesiredSize.Width;
            double childTextheight = childText.DesiredSize.Height;
            parentText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double parentTextWidth = parentText.DesiredSize.Width;
            double parentTextHeight = parentText.DesiredSize.Height;

            DoubleAnimation moveChildTextUp = new DoubleAnimation
            {
                From = Canvas.GetTop(childText),
                To = Canvas.GetTop(parent) + ballRadius - childTextheight/2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveChildTextHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(childText),
                To = Canvas.GetLeft(parent) + ballRadius - childTextWidth / 2,
                Duration = animationSeconds*2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveChildTextUp, childText);
            Storyboard.SetTargetProperty(moveChildTextUp, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveChildTextHorizontal, childText);
            Storyboard.SetTargetProperty(moveChildTextHorizontal, new PropertyPath(Canvas.LeftProperty));

            DoubleAnimation moveParentTextDown = new DoubleAnimation
            {
                From = Canvas.GetTop(parentText),
                To = Canvas.GetTop(child) + ballRadius - parentTextHeight/2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DoubleAnimation moveParentTextHorizontal = new DoubleAnimation
            {
                From = Canvas.GetLeft(parentText),
                To = Canvas.GetLeft(child) + ballRadius - parentTextWidth / 2,
                Duration = animationSeconds*2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveParentTextDown, parentText);
            Storyboard.SetTargetProperty(moveParentTextDown, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(moveParentTextHorizontal, parentText);
            Storyboard.SetTargetProperty(moveParentTextHorizontal, new PropertyPath(Canvas.LeftProperty));


            _storyboard.Children.Add(moveParentTextDown);
            _storyboard.Children.Add(moveParentTextHorizontal);
            _storyboard.Children.Add(moveChildTextUp);
            _storyboard.Children.Add(moveChildTextHorizontal);
            _storyboard.Children.Add(moveParentDown);
            _storyboard.Children.Add(moveParentHorizontal);
            _storyboard.Children.Add(moveChildUp);
            _storyboard.Children.Add(moveChildHorizontal);

            int temp = _numbers[childIndex];
            _numbers[childIndex] = _numbers[parentIndex];
            _numbers[parentIndex] = temp;

            _storyboard.Begin();
        }

        //private void SwapInTopLine(int childIndex, int parentIndex)
        //{
        //    //_numbers swap taky
        //    _storyboard.Children.Clear();
        //    Ellipse child = TopLineBalls[childIndex];
        //    Ellipse parent = TopLineBalls[parentIndex];

        //    TextBlock childText = TopLineText[childIndex];
        //    TextBlock parentText = TopLineText[parentIndex];

        //    TimeSpan animationSeconds = AnimationSeconds;
        //    double ballRadius = _ballRadius;

        //    DoubleAnimation moveChildUp = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(child),
        //        To = Canvas.GetTop(child) - 2.5 * ballRadius,
        //        Duration = animationSeconds/2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        //    };
        //    DoubleAnimation moveChildLeft = new DoubleAnimation
        //    {
        //        From = Canvas.GetLeft(child),
        //        To = Canvas.GetLeft(parent),
        //        Duration = animationSeconds,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds/2
        //    };
        //    DoubleAnimation moveChildDown = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(child) - 2.5 * ballRadius,
        //        To = Canvas.GetTop(child),
        //        Duration = animationSeconds/2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds/2 * 3
        //    };

        //    Storyboard.SetTarget(moveChildUp, child);
        //    Storyboard.SetTargetProperty(moveChildUp, new PropertyPath(Canvas.TopProperty));
        //    Storyboard.SetTarget(moveChildLeft, child);
        //    Storyboard.SetTargetProperty(moveChildLeft, new PropertyPath(Canvas.LeftProperty));
        //    Storyboard.SetTarget(moveChildDown, child);
        //    Storyboard.SetTargetProperty(moveChildDown, new PropertyPath(Canvas.TopProperty));

        //    DoubleAnimation moveParentDown = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(parent),
        //        To = Canvas.GetTop(child) + 2.5 * ballRadius,
        //        Duration = animationSeconds / 2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        //    };
        //    DoubleAnimation moveParentRight = new DoubleAnimation
        //    {
        //        From = Canvas.GetLeft(parent),
        //        To = Canvas.GetLeft(child),
        //        Duration = animationSeconds,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds / 2
        //    };
        //    DoubleAnimation moveParentUp = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(parent) + 2.5 * ballRadius,
        //        To = Canvas.GetTop(parent),
        //        Duration = animationSeconds / 2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds / 2 * 3
        //    };

        //    Storyboard.SetTarget(moveParentDown, parent);
        //    Storyboard.SetTargetProperty(moveParentDown, new PropertyPath(Canvas.TopProperty));
        //    Storyboard.SetTarget(moveParentRight, parent);
        //    Storyboard.SetTargetProperty(moveParentRight, new PropertyPath(Canvas.LeftProperty));
        //    Storyboard.SetTarget(moveParentUp, parent);
        //    Storyboard.SetTargetProperty(moveParentUp, new PropertyPath(Canvas.TopProperty));


        //    childText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //    double childTextWidth = childText.DesiredSize.Width;
        //    parentText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //    double parentTextWidth = parentText.DesiredSize.Width;

        //    DoubleAnimation moveChildTextUp = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(childText),
        //        To = Canvas.GetTop(childText) - 2.5 * ballRadius,
        //        Duration = animationSeconds / 2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        //    };
        //    DoubleAnimation moveChildTextLeft = new DoubleAnimation
        //    {
        //        From = Canvas.GetLeft(childText),
        //        To = Canvas.GetLeft(parent) + ballRadius - childTextWidth/2,
        //        Duration = animationSeconds,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds / 2
        //    };
        //    DoubleAnimation moveChildTextDown = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(childText) - 2.5 * ballRadius,
        //        To = Canvas.GetTop(childText),
        //        Duration = animationSeconds / 2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds / 2 * 3
        //    };

        //    Storyboard.SetTarget(moveChildTextUp, childText);
        //    Storyboard.SetTargetProperty(moveChildTextUp, new PropertyPath(Canvas.TopProperty));
        //    Storyboard.SetTarget(moveChildTextLeft, childText);
        //    Storyboard.SetTargetProperty(moveChildTextLeft, new PropertyPath(Canvas.LeftProperty));
        //    Storyboard.SetTarget(moveChildTextDown, childText);
        //    Storyboard.SetTargetProperty(moveChildTextDown, new PropertyPath(Canvas.TopProperty));

        //    DoubleAnimation moveParentTextDown = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(parentText),
        //        To = Canvas.GetTop(childText) + 2.5 * ballRadius,
        //        Duration = animationSeconds / 2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        //    };
        //    DoubleAnimation moveParentTextRight = new DoubleAnimation
        //    {
        //        From = Canvas.GetLeft(parentText),
        //        To = Canvas.GetLeft(child) + ballRadius - parentTextWidth/2,
        //        Duration = animationSeconds,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds / 2
        //    };
        //    DoubleAnimation moveParentTextUp = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(parentText) + 2.5 * ballRadius,
        //        To = Canvas.GetTop(parentText),
        //        Duration = animationSeconds / 2,
        //        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        //        BeginTime = animationSeconds / 2 * 3
        //    };

        //    Storyboard.SetTarget(moveParentTextDown, parentText);
        //    Storyboard.SetTargetProperty(moveParentTextDown, new PropertyPath(Canvas.TopProperty));
        //    Storyboard.SetTarget(moveParentTextRight, parentText);
        //    Storyboard.SetTargetProperty(moveParentTextRight, new PropertyPath(Canvas.LeftProperty));
        //    Storyboard.SetTarget(moveParentTextUp, parentText);
        //    Storyboard.SetTargetProperty(moveParentTextUp, new PropertyPath(Canvas.TopProperty));

        //    _storyboard.Children.Add(moveParentTextDown);
        //    _storyboard.Children.Add(moveParentTextRight);
        //    _storyboard.Children.Add(moveParentTextUp);
        //    _storyboard.Children.Add(moveChildTextUp);
        //    _storyboard.Children.Add(moveChildTextLeft);
        //    _storyboard.Children.Add(moveChildTextDown);
        //    _storyboard.Children.Add(moveParentDown);
        //    _storyboard.Children.Add(moveParentRight);
        //    _storyboard.Children.Add(moveParentUp);
        //    _storyboard.Children.Add(moveChildUp);
        //    _storyboard.Children.Add(moveChildLeft);
        //    _storyboard.Children.Add(moveChildDown);


        //    int temp = _numbers[childIndex];
        //    _numbers[childIndex] = _numbers[parentIndex];
        //    _numbers[parentIndex] = temp;
        //    _storyboard.Begin();
        //}

        private void DrawComparison(int indexChild, int indexParent, bool truth, string comparison = ">")
        {
            double ballRadius = _ballRadius;
            DrawSmoothLine(new Point(TreeBallPos[indexChild].X + ballRadius, TreeBallPos[indexChild].Y),
                new Point(TreeBallPos[indexParent].X + ballRadius, TreeBallPos[indexParent].Y + 2 * ballRadius),
                ballRadius, true);

            SolidColorBrush color;

            if (truth)
            {
                color = Brushes.Green;
            }else
            {
                color = Brushes.Red;
            }

            TextBlock textBlock = new TextBlock
            {
                Text = comparison,
                FontSize = ballRadius,
                FontWeight = FontWeights.Bold,
                Foreground = color,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = textBlock.DesiredSize.Width;
            double textHeight = textBlock.DesiredSize.Height;

            double xPos = (TreeBallPos[indexParent].X + 2 * ballRadius + TreeBallPos[indexChild].X) / 2; 
            double yPos = (TreeBallPos[indexParent].Y + 2 * ballRadius + TreeBallPos[indexChild].Y) / 2;

            double deltaX = TreeBallPos[indexChild].X - TreeBallPos[indexParent].X;
            double deltaY = TreeBallPos[indexChild].Y - TreeBallPos[indexParent].Y;
            double angleInRadians = Math.Atan2(deltaY, deltaX);
            double angleInDegrees = angleInRadians * (180 / Math.PI);



            Canvas.SetLeft(textBlock, xPos - textWidth/4);
            Canvas.SetTop(textBlock, yPos - textHeight/2 - textHeight/4 + textHeight/8 + textHeight/12);


            RotateTransform rotateTransform = new RotateTransform
            {
                Angle = angleInDegrees,
                CenterX = textWidth / 2,
                CenterY = textHeight / 2
            };
            textBlock.RenderTransform = rotateTransform;

            _canvas.Children.Add(textBlock);
        }

        public void DrawComponenets()
        {
            TopLineBalls.Clear();
            TopLineText.Clear();
            TreeBallPos.Clear();
            DrawTopLine(); // musi byt kopirovano z minulych topline balls(aby zustali vlastnosti)
            DrawTreePattern();
            if (!_isBuildingTree)
            {
                DrawBottomLine();
            }
            DrawTree(); // taky kopie minulych (aby zustali vlastnosti)
        }

        private void SendCopyDown(int index)
        {
            Ellipse selectedBall = TopLineBalls[index];
            Ellipse newTreeBall = CopyEllipse(selectedBall);

            double currentX = Canvas.GetLeft(newTreeBall);
            double currentY = Canvas.GetTop(newTreeBall);

            double ballRadius = _ballRadius;

            TimeSpan animationSeconds = AnimationSeconds;

            ColorAnimation strokeColorAnimation = new ColorAnimation
            {
                From = (selectedBall.Stroke as SolidColorBrush)?.Color,
                To = Color.FromRgb(166, 92, 66),
                Duration = animationSeconds / 2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            
            Storyboard.SetTarget(strokeColorAnimation, selectedBall);
            Storyboard.SetTargetProperty(strokeColorAnimation, new PropertyPath("Stroke.Color"));
            _storyboard.Children.Add(strokeColorAnimation);

            DoubleAnimation moveXAnimation = new DoubleAnimation
            {
                From = currentX,              
                To = TreeBallPos[index].X,           
                Duration = animationSeconds,  
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }  
            };

            DoubleAnimation moveYAnimation = new DoubleAnimation
            {
                From = currentY,              
                To = TreeBallPos[index].Y,           
                Duration = animationSeconds, 
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }  
            };


            Storyboard.SetTarget(moveXAnimation, newTreeBall);
            Storyboard.SetTargetProperty(moveXAnimation, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTarget(moveYAnimation, newTreeBall);
            Storyboard.SetTargetProperty(moveYAnimation, new PropertyPath(Canvas.TopProperty));


            TextBlock selectedTextBlock = TopLineText[index];
            TextBlock newTreeTextBlock = CopyTextBlock(selectedTextBlock);

            currentX = Canvas.GetLeft(newTreeTextBlock);
            currentY = Canvas.GetTop(newTreeTextBlock);

            newTreeTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = newTreeTextBlock.DesiredSize.Width;
            double textHeight = newTreeTextBlock.DesiredSize.Height;

            DoubleAnimation moveXAnimation1 = new DoubleAnimation
            {
                From = currentX,
                To = TreeBallPos[index].X + ballRadius - textWidth/2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveYAnimation1 = new DoubleAnimation
            {
                From = currentY,
                To = TreeBallPos[index].Y + ballRadius - textHeight/2,
                Duration = animationSeconds,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };


            Storyboard.SetTarget(moveXAnimation1, newTreeTextBlock);
            Storyboard.SetTargetProperty(moveXAnimation1, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTarget(moveYAnimation1, newTreeTextBlock);
            Storyboard.SetTargetProperty(moveYAnimation1, new PropertyPath(Canvas.TopProperty));

            TreeBall[index] = newTreeBall;
            TreeText[index] = newTreeTextBlock;

            _storyboard.Children.Add(moveXAnimation1);
            _storyboard.Children.Add(moveYAnimation1);
            _storyboard.Children.Add(moveXAnimation);
            _storyboard.Children.Add(moveYAnimation);
            _canvas.Children.Add(newTreeBall);
            _canvas.Children.Add(newTreeTextBlock);
            
            _storyboard.Begin();

        }

        private void SelectTopLineBall(int index, Color color, bool fadeIn = true)
        {
            _storyboard.Children.Clear();
            Ellipse selectedBall = TopLineBalls[index];
            TimeSpan animationSeconds = AnimationSeconds;
            if (!fadeIn)
            {
                animationSeconds = TimeSpan.FromSeconds(0);
            }
            ColorAnimation strokeColorAnimation = new ColorAnimation
            {
                From = (selectedBall.Stroke as SolidColorBrush)?.Color,
                To = color,
                Duration = animationSeconds/2,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(strokeColorAnimation, selectedBall);
            Storyboard.SetTargetProperty(strokeColorAnimation, new PropertyPath("Stroke.Color"));
            _storyboard.Children.Add(strokeColorAnimation);
            _storyboard.Begin();
        }

        private void DrawBottomLine(bool animate = false)
        {
            double ballRadius = _ballRadius;
            double fakeBallRadius = _fakeBallRadius;
            double yPos = TreeBallPos[TreeBallPos.Keys.Max()].Y + 2*ballRadius* ( _verticalGap + 2);
            double xPos = (_canvas.ActualWidth - ballRadius * (3 * _numbers.Count - 1))/2;

            for (int i = 0; i < _numbers.Count; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = fakeBallRadius * 2,
                    Height = fakeBallRadius * 2,
                    Fill = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                    Stroke = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    StrokeThickness = 0.1 * fakeBallRadius,
                    Opacity = (animate) ? 0 : 1
                };

                Canvas.SetLeft(ellipse, xPos + (ballRadius - fakeBallRadius));
                Canvas.SetTop(ellipse, yPos + (ballRadius - fakeBallRadius));
                
                if (animate)
                {
                    TimeSpan animationSeconds = AnimationSeconds;
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = animationSeconds*2,
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                        BeginTime = animationSeconds
                    };
                    Storyboard.SetTarget(fadeOutAnimation, ellipse);
                    Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));


                    DoubleAnimation moveUp = new DoubleAnimation
                    {
                        From = yPos,
                        To = TreeBallPos[TreeBallPos.Keys.Max()].Y - 2 * ballRadius - ballRadius * _verticalGap + 2 * ballRadius * (_verticalGap + 2) + (ballRadius - fakeBallRadius),
                        Duration = animationSeconds,
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                        BeginTime = animationSeconds
                    };


                    Storyboard.SetTarget(moveUp, ellipse);
                    Storyboard.SetTargetProperty(moveUp, new PropertyPath(Canvas.TopProperty));
                    _storyboard.Children.Add(fadeOutAnimation);
                    _storyboard.Children.Add(moveUp);
          
                }
                _canvas.Children.Add(ellipse);
                if (i > (_numbers.Count - 1 - numDone))
                {
                    Ellipse elli = new Ellipse
                    {
                        Width = ballRadius * 2,
                        Height = ballRadius * 2,
                        Fill = new SolidColorBrush(Color.FromRgb(118, 174, 91)),
                        Stroke = new SolidColorBrush(Color.FromRgb(54, 144, 92)),
                        StrokeThickness = 0.1 * ballRadius
                    };

                    Canvas.SetLeft(elli, xPos);
                    Canvas.SetTop(elli, yPos);



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

                    Canvas.SetLeft(number, xPos + ballRadius - textWidth / 2);
                    Canvas.SetTop(number, yPos + ballRadius - textHeight / 2);

                    _canvas.Children.Add(elli);
                    _canvas.Children.Add(number);

                    

                }
                xPos += 3 * ballRadius;
            }

        }

        private void DrawTree()
        {
            double ballRadius = _ballRadius;

            for (int i = 0; i < TreeBall.Count - numDone; i++)
            {

                Ellipse ellipse = CopyEllipse(TopLineBalls[i]);
                TextBlock textBlock = CopyTextBlock(TopLineText[i]);

                TreeBall[i] = ellipse;
                TreeText[i] = textBlock;

                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = textBlock.DesiredSize.Width;
                double textHeight = textBlock.DesiredSize.Height;


                Canvas.SetLeft(ellipse, TreeBallPos[i].X);
                Canvas.SetTop(ellipse, TreeBallPos[i].Y);

                Canvas.SetLeft(textBlock, TreeBallPos[i].X + ballRadius - textWidth / 2);
                Canvas.SetTop(textBlock, TreeBallPos[i].Y + ballRadius - textHeight / 2);
               //MessageBox.Show($"Ball {i} Position: X={TreeBallPos[i].X}, Y={TreeBallPos[i].Y}");

                _canvas.Children.Add(ellipse);
                _canvas.Children.Add(textBlock);
            }
        }

        private void DrawTreePattern()
        {
            double ballRadius = _ballRadius;
            double fakeBallRadius = _fakeBallRadius;
            double xPos;
            double yPos = (_isBuildingTree) ? 2 * ballRadius * (_verticalGap + 1) : _verticalGap * ballRadius;
            double numOfBallsInGap;
            double gapSize;
            int realIndex;

            for (int i = 0; i < NumOfLayers; i++) //2 na i je pocet na jednom layer
            {
                numOfBallsInGap = Math.Pow(2, NumOfLayers - i) - 1;
                gapSize = ballRadius * (3 * numOfBallsInGap + 1);
                xPos = (_canvas.ActualWidth - (2*ballRadius*Math.Pow(2, i) + gapSize*(Math.Pow(2, i)- 1))) / 2;
                
                for (int j = 0; j < Math.Pow(2, i); j++)
                {
                    realIndex = (int)Math.Pow(2, i) - 1 + j;
                    TreeBallPos[realIndex] = new Point(xPos, yPos);

                    Ellipse ellipse = new Ellipse
                    {
                        Width = fakeBallRadius * 2,
                        Height = fakeBallRadius * 2,
                        Fill = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                        Stroke = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                        StrokeThickness = 0.1 * fakeBallRadius
                    };

                    Canvas.SetLeft(ellipse, xPos + (ballRadius-fakeBallRadius));
                    Canvas.SetTop(ellipse, yPos + (ballRadius - fakeBallRadius));

                    _canvas.Children.Add(ellipse);
                    //if (realIndex < _numbers.Count)
                    //{
                    //    TextBlock number = new TextBlock
                    //    {
                    //        Text = realIndex.ToString(),
                    //        FontSize = fakeBallRadius / 3 * 2,
                    //        FontWeight = FontWeights.Bold,
                    //        Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                    //        TextAlignment = TextAlignment.Center
                    //    };

                    //    number.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    //    double textWidth = number.DesiredSize.Width;
                    //    double textHeight = number.DesiredSize.Height;

                    //    Canvas.SetLeft(number, xPos + (ballRadius - fakeBallRadius) + fakeBallRadius - textWidth / 2);
                    //    Canvas.SetTop(number, yPos - 0.8 * fakeBallRadius - textHeight / 2);

                    //    _canvas.Children.Add(number);
                    //}

                    xPos += gapSize + 2 * ballRadius;
                }
                yPos += ballRadius * (2 + _verticalGap);
            }
        }

        private void DrawTopLine()
        {
            _canvas.Children.Clear();
            TopLineBalls.Clear();
            double ballRadius = _ballRadius;
            
            double currentXPos = (_canvas.ActualWidth - ballRadius*(3*_numbers.Count - 1))/2;
            double yPos = ballRadius * _verticalGap;
            for (int i = 0; i <  _numbers.Count; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = ballRadius * 2,
                    Height = ballRadius * 2,
                    Fill = new SolidColorBrush(Color.FromRgb(216, 122, 86)),
                    Stroke = new SolidColorBrush(Color.FromRgb(166, 92, 66)),
                    StrokeThickness = 0.1 * ballRadius
                };

                Canvas.SetLeft(ellipse, currentXPos);
                Canvas.SetTop(ellipse, yPos);

                

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
                Canvas.SetTop(number, yPos + ballRadius - textHeight / 2);

                TopLineBalls[i] = ellipse;
                TopLineText[i] = number;

                if (_isBuildingTree)
                {
                    _canvas.Children.Add(ellipse);
                    _canvas.Children.Add(number);
                }
               

                //TextBlock index = new TextBlock
                //{
                //    Text = i.ToString(),
                //    FontSize = ballRadius * 0.7 / 3 * 2,
                //    FontWeight = FontWeights.Bold,
                //    Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                //    TextAlignment = TextAlignment.Center
                //};

                //index.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //textWidth = index.DesiredSize.Width;
                //textHeight = index.DesiredSize.Height;

                //Canvas.SetLeft(index, currentXPos + ballRadius - textWidth / 2);
                //Canvas.SetTop(index, yPos - 0.7 * 0.8 * ballRadius - textHeight / 2);

                //_canvas.Children.Add(index);


                currentXPos += 3 * ballRadius;
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
                path.StrokeDashArray = new DoubleCollection { ballRadius / 7, ballRadius / 5 };
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

        private Ellipse CopyEllipse(Ellipse originalEllipse)
        {
            Ellipse copiedEllipse = new Ellipse
            {
                Width = originalEllipse.Width,
                Height = originalEllipse.Height,
                Fill = originalEllipse.Fill,
                Stroke = originalEllipse.Stroke,
                StrokeThickness = originalEllipse.StrokeThickness,
                Opacity = originalEllipse.Opacity
            };

            Canvas.SetLeft(copiedEllipse, Canvas.GetLeft(originalEllipse));
            Canvas.SetTop(copiedEllipse, Canvas.GetTop(originalEllipse));

            return copiedEllipse;
        }
        private TextBlock CopyTextBlock(TextBlock originalTextBlock)
        {
            TextBlock copiedTextBlock = new TextBlock
            {
                Text = originalTextBlock.Text,
                FontSize = originalTextBlock.FontSize,
                FontWeight = originalTextBlock.FontWeight,
                Foreground = originalTextBlock.Foreground,
                TextAlignment = originalTextBlock.TextAlignment,
                Opacity = originalTextBlock.Opacity
            };

            Canvas.SetLeft(copiedTextBlock, Canvas.GetLeft(originalTextBlock));
            Canvas.SetTop(copiedTextBlock, Canvas.GetTop(originalTextBlock));

            return copiedTextBlock;
        }


    }

}
