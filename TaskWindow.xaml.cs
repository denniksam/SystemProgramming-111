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
using System.Windows.Shapes;

namespace SystemProgramming_111
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public TaskWindow()
        {
            InitializeComponent();
        }

        private async void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            ConsoleBlock.Text = "";
            for (int i = 0; i < 12; i++)
            {
                // Task.Run(PlusPercent).Wait();
                await PlusPercent();
            }
        }
        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }
        private double sum;
        private async Task PlusPercent()
        {
            await Task.Delay(300);
            sum *= 1.1;
            ConsoleBlock.Text += $"{sum}\n";
            // Dispatcher.Invoke(() => ConsoleBlock.Text += $"{sum}\n");
        }
    }
}
/* Д.З. Реализовать решение задачи рассчета годовых процентов при помощи многозадачности
 * Использовать случайную задержку 250-350 мс
 * Отображать номер месяца, процент и полученный результат, а также двигать индикатор прогресса
 */