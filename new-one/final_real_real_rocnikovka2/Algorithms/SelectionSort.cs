using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Graphics.Rendering;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_real_real_rocnikovka2.Algorithms
{
    public class SelectionSort : SortingAlgorithm
    {
        private static readonly SelectionSort instance = new();
        public static new SelectionSort Instance => instance;

        // STEP VARIABLES
        private int CurrentIndex;
        private int MinIndex;
        private int ComparisonIndex;
        private int LastMinIndex;

        private SelectionSort()
        {
            Name = AlgorithmName.SELECTION_SORT_NAME;
        }

        public override void Reset(List<int> numbers, List<Box> boxes)
        {
            Numbers = numbers;
            Boxes = boxes;
        }
        public override void Reset(List<int> numbers, List<Ball> balls, List<GraphicElement> graphicElements)
        {
            Numbers = numbers;
            Balls = balls;
            GraphicElements = graphicElements;
            IsSortedBool = false;
            N = Numbers.Count;
            CurrentIndex = 0;
            StepState = 0;
            MinIndex = 0;
            ComparisonIndex = 0;
        }

        public override async Task Sort()
        {
            if (IsSorted()) return;
            int n = Numbers.Count;
            for (int i = 0; i < n - 1; i++)
            {
                Boxes[i].ChangeColor(ColorPalette.PivotBarFill);
                int minNumberIndex = i;

                for (int j = i + 1; j < n; j++)
                {
                    if (Globals.Stop) return;
                    if (IsSorted()) // Ano, toto pridava casovou komplexitu, ale toto vizualni znazorneni to nejak neefektuje
                    {
                        Draw.DrawDone(Boxes, ColorPalette.SelectedBarFill);
                        return;
                    };

                    Boxes[j].ChangeColor(ColorPalette.SelectedBarFill);
                    
                    await Wait(Globals.AnimationMs, j);

                    if (Numbers[j] < Numbers[minNumberIndex])
                    {
                        if (minNumberIndex != i)
                            Boxes[minNumberIndex].ChangeColor(ColorPalette.DefaultBarFill);

                        minNumberIndex = j;
                        Boxes[j].ChangeColor(ColorPalette.MinimumBarFill);
                    } else
                    {
                        Boxes[j].ChangeColor(ColorPalette.DefaultBarFill);
                    }
                }

                Boxes[i].ChangeColor(ColorPalette.DefaultBarFill); 
                Boxes[minNumberIndex].ChangeColor(ColorPalette.SelectedBarFill);

                if (minNumberIndex != i)
                {
                    SwapInList(Numbers, i, minNumberIndex);
                    SwapInList(Boxes, i, minNumberIndex);

                    Draw.SwapXPos(Boxes[i], Boxes[minNumberIndex]);
                }
            }
            Boxes[n-1].ChangeColor(ColorPalette.SelectedBarFill);


        }

        public override void Step()
        {
            if (IsSortedBool) return;

            switch (StepState)
            {
                case 0:
                    Animate.AnimationClear();
                    Animate.BallStrokeColorChange(Balls[CurrentIndex], ColorPalette.SelectStroke, 0.2, 0);
                    Animate.AnimationRun();
                    MinIndex = CurrentIndex;
                    ComparisonIndex = CurrentIndex + 1;
                    StepState = 1;
                    break;
                case 1:
                    
                    Animate.AnimationClear();
                    if (LastMinIndex >= 0)
                        Animate.BallStrokeColorChange(Balls[LastMinIndex], ColorPalette.DefaultStroke, 0.2, 0);
                    if (ComparisonIndex < N)
                        Animate.BallStrokeColorChange(Balls[ComparisonIndex], ColorPalette.SelectStroke, 0.2, 0);
                    Animate.BallStrokeColorChange(Balls[ComparisonIndex - 1], ColorPalette.DefaultStroke, 0.2, 0);
                    Animate.BallStrokeColorChange(Balls[MinIndex], ColorPalette.SoftBlueStroke, 0.2, 0);
                    if (ComparisonIndex < N)
                    {
                        if (Numbers[ComparisonIndex] < Numbers[MinIndex])
                        {
                            LastMinIndex = MinIndex;
                            MinIndex = ComparisonIndex;
                        } 
                        ComparisonIndex++;
                    } 
                    else
                    {
                        StepState = 2;
                    }
                    Animate.AnimationRun();
                    break;
                case 2:
                    Animate.AnimationClear();
                    Animate.BallFillColorChange(Balls[MinIndex], ColorPalette.GreenFill, 1, 0.5);
                    Animate.BallStrokeColorChange(Balls[MinIndex], ColorPalette.GreenStroke, 1, 0.5);
                    if (MinIndex != CurrentIndex)
                    {
                        Animate.BallSwap(Balls[CurrentIndex], Balls[MinIndex], 1, 0, 1.5);
                        SwapInList(Numbers, MinIndex, CurrentIndex);
                        SwapInList(Balls, MinIndex, CurrentIndex);
                        Draw.SwapXPos(Balls[CurrentIndex], Balls[MinIndex]);
                    }
                    CurrentIndex++;
                    if (CurrentIndex >= N - 1)
                    {
                        IsSortedBool = true;
                        Animate.BallFillColorChange(Balls[CurrentIndex], ColorPalette.GreenFill, 1, 1);
                        Animate.BallStrokeColorChange(Balls[CurrentIndex], ColorPalette.GreenStroke, 1, 1);
                    }
                    StepState = 0;
                    LastMinIndex = -1;
                    Animate.AnimationRun();
                    break;
            }
        }
    }
}
