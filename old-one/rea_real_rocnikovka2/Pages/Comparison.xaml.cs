using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace rea_real_rocnikovka2
{
    public partial class Comparison : Page
    {
        public ObservableCollection<string> AlgorithmOptions { get; set; }
        public string[] SelectedAlgorithm { get; set; }
        public List<int> numbers { get; set; }
        public bool[] scriptIsRunning { get; set; }
        public bool[] arrIsSorted { get; set; }
        private bool cancel;
        private Canvas[] canvas { get; set; }
        private object lockObject = new object();  // Lock object to synchronize access to shared state


        public Comparison()
        {
            InitializeComponent();

            AlgorithmOptions = new ObservableCollection<string>(){
                "Bubble Sort",
                "Insertion Sort",
                "Selection Sort",
                "Quick Sort",
                "Heap Sort",
                "Merge Sort"
            };
            SelectedAlgorithm = new string[4];
            arrIsSorted = new bool[4];
            scriptIsRunning = new bool[4];
            cancel = false;
            StopButton.IsEnabled = false;
            canvas = new Canvas[4]{ Canvas1, Canvas2, Canvas3, Canvas4 };

            numbers = new List<int>() { 15, 10, 13, 4, 9, 3, 6, 9, 8, 14, 21, 17, 15, 20, 25, 31, 22 };
            DataContext = this;

        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        { 
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
