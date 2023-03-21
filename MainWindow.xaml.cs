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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SystemProgramming_111
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Threading_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new ThreadingWindow().ShowDialog();
            this.Show();
        }

        private void Multi_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MultiWindow().ShowDialog();
            this.Show();
        }

        private void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new TaskWindow().ShowDialog();
            this.Show();
        }

        private void SynchroButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new SynchroWindow().ShowDialog();
            this.Show();
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new ProcessWindow().ShowDialog();
            this.Show();
        }

        private void DllButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new DllWindow().ShowDialog();
            this.Show();
        }
    }
}
