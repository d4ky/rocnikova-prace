using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Text;
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
    //smazani vseho
    //user input
    //title + icon
    //step

    public partial class MainWindow : Window
    {
        public AlgorithmShowcase _algorithmShowcase;
        public Comparison _comparison;
        public Explanation _explanation;

        public MainWindow()
        {
            InitializeComponent();

            _comparison = new Comparison();
            _algorithmShowcase = new AlgorithmShowcase();
            _explanation = new Explanation();

            Main.Content = _algorithmShowcase;
        }

        private void AlgorithmShowcaseBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = _algorithmShowcase;
        }
        private void ComparisonBtn_Click(Object sender, RoutedEventArgs e)
        {
            Main.Content = _comparison;
        }
        private void ExplanationBtn_Click(Object obj, RoutedEventArgs e) 
        {
            Main.Content = _explanation;
        }
    }
}