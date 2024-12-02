using final_real_real_rocnikovka2.Graphics.Objects;
using final_real_real_rocnikovka2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace final_real_real_rocnikovka2.Algorithms
{
    public abstract class SortingAlgorithm
    {
        public String Name { get; protected set; }
        public static SortingAlgorithm Instance { get; }
        protected List<int> Numbers { get; set; }
        protected List<Box> Boxes { get; set; }
        protected List<Ball> Balls { get; set; }
        protected int StepState { get; set; }
        protected int N { get; set; }
        protected List<GraphicElement> GraphicElements { get; set; }
        public bool IsSortedBool { get; set; }

        public abstract void Step();
        public abstract Task Sort();
        public abstract void Reset(List<int> numbers, List<Box> boxes);
        public abstract void Reset(List<int> numbers, List<Ball> balls, List<GraphicElement> graphicElements);

        protected static Task Wait(int delay, int iteratorNumber)
        {
            if (delay > 14) return Task.Delay(delay);
            else
            {
                if (iteratorNumber % (15 - delay) == 0 || Globals.MultiIsChecked) return Task.Delay(1); // Globals.MulitIsChecked protoze jinak se mohou algoritmy zkazit, kdyz jich jede vic najednou a skippuji delay
                else return Task.CompletedTask;
            }
        }

        public bool IsSorted()
        {
            if (Numbers == null) return false;
            for (int i = 0; i < Numbers.Count - 1; i++)
            {
                if (Numbers[i] > Numbers[i + 1])
                {
                    return false;
                }
            }
            return true;
        }

        protected static void SwapInList<T>(List<T> list, int indexA, int indexB)
        {
            (list[indexB], list[indexA]) = (list[indexA], list[indexB]);
        }
    }
}
