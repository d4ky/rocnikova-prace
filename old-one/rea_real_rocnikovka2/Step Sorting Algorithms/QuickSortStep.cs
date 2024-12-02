using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace rea_real_rocnikovka2.Step_Sorting_Algorithms
{
    public class QuickSortStep
    {
        private List<int> numbers; // nemenim

        private List<List<int>> parts;
        private List<List<int>> newParts;
        private Dictionary<int, Ellipse> indexToBall;
        private Dictionary<int, TextBlock> indexToText;
        private int stepState;

        public QuickSortStep(List<int> numbers, Canvas canvas, Func<double> getAnimationStepSpeed)
        {
            this.numbers = new List<int>(numbers); // Create a copy of the list
            parts = new List<List<int>>();
            parts.Add(new List<int>(this.numbers));
            newParts = new List<List<int>>();
            newParts.Add(new List<int>());
            newParts.Add(new List<int>());

        }

        private int indexStart;
        private int indexEnd;
        private int currentBigIndex;
        private int currentIndex;

        public void Step()
        {
            switch (stepState) 
            {
                case 1:
                    currentBigIndex = 0;

                    for (int i = 0; i < parts.Count; i++)
                    {
                        //pokud parts[i].count != 1
                            //oznacim barevne parts[i].last
                        //jinak oznacim parts[i][0] šedě
                    }
                    stepState = 2; ;
                    //oznacim vsechny pivoty (posledni v kazdem listu v _parts
                    //poslu na _stepstate = 2 pro proovnavani
                    break;
                case 2:
                    if (currentIndex == parts[currentBigIndex].Count - 1)
                    {
                        while (currentIndex <= parts.Max(lst => lst.Count) - 1)
                        {
                            currentBigIndex++;
                            while (parts[currentBigIndex].Count - 2 < currentIndex && currentBigIndex <= parts.Count)
                                currentBigIndex++;
                            if (currentBigIndex == parts.Count - 1 && parts[currentBigIndex].Count - 2 < currentIndex)
                            {
                                currentBigIndex = 0;
                                currentIndex++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (parts[currentBigIndex][currentIndex] > parts[currentBigIndex].Last())
                    {
                        newParts[currentBigIndex * 2 + 1].Add(parts[currentBigIndex][currentIndex]);

                    } else
                    {
                        newParts[currentBigIndex * 2].Add(parts[currentBigIndex][currentIndex]);
                    }
                    if (currentBigIndex < parts.Count - 1)
                    {
                        currentBigIndex++;
                        while (currentIndex <= parts.Max(lst => lst.Count)-1)
                        {
                            while (parts[currentBigIndex].Count - 2 < currentIndex && currentBigIndex <= parts.Count)
                                currentBigIndex++;
                            if (currentBigIndex == parts.Count - 1  && parts[currentBigIndex].Count -2 < currentIndex)
                            {
                                currentBigIndex = 0;
                                currentIndex++;
                            } else
                            {
                                break;
                            }
                        }
                        if (currentIndex > parts.Max(lst => lst.Count) - 2)
                        {
                            //donzo
                        }
                    } else
                    {
                        currentBigIndex = 0;
                        currentIndex++;
                    }
                    //v kazdem porovnam ity index s pivotem
                    //pokud je vetsi tak case 3, pokud mensi tak case 4
                    // kdyz nemam co delat tak case 4
                    break;
                case 3:
                    //je mensi, hodim do leveho listu
                    //je vetsi, hodim do praveho listu
                    //cas 2 jen o list dal (curr index ++)
                    break;
                case 4:
                    //hodim vsechny pivoty na sve mista, zanechaji stopu a animaci jdou na sva mista v dolnim listu
                    // case 1
                    break;
                    
            }
        }
    }
}
