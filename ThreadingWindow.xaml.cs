using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SystemProgramming_111
{
    /// <summary>
    /// Interaction logic for ThreadingWindow.xaml
    /// </summary>
    public partial class ThreadingWindow : Window
    {
        public ThreadingWindow()
        {
            InitializeComponent();
        }

        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            Start1();
        }

        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Start1()
        {
            for (int i = 0; i < 10; i++)
            {
                progressBar1.Value = (i + 1) * 10;
                ConsoleBlock.Text += i.ToString() + "\n";  // Console.WriteLine(i);
                Thread.Sleep(300);
            }
        }

        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            new Thread( Start1 ).Start();  //  - исключение (изменения из другого потока)
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {

        }
        

        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        {
            new Thread( Start3 ).Start();
        }

        private void ButtonStop3_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Start3()
        {
            for (int i = 0; i < 10; i++)
            {
                this.Dispatcher.Invoke(() =>  // делегирование работы потоку окна
                {
                    progressBar3.Value = (i + 1) * 10;
                    ConsoleBlock.Text += i.ToString() + "\n";  // Console.WriteLine(i);
                });
                
                Thread.Sleep(300);
            }
        }
    }
}
