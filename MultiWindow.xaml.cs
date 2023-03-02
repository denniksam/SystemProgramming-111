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
    /// Interaction logic for MultiWindow.xaml
    /// </summary>
    public partial class MultiWindow : Window
    {
        private readonly Random random = new();
        public MultiWindow()
        {
            InitializeComponent();
        }

        #region variant 1
        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            progressBar1.Value = 0;
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent).Start();
            }
        }

        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }

        private double sum;

        private void plusPercent()
        {
            // в первом варианте каждый месяц 10%
            double val = sum;   // получаем предыдущие данные
            Thread.Sleep(random.Next(250, 350));  // имитируем длительный запрос данных
            double percent = 10;
            // рассчитываем итог
            val *= 1 + percent / 100;
            // сохраняем изменения в общей сумме
            sum = val;
            // выводим данные о своей работе
            Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += sum + "\n";
                progressBar1.Value += 100.0 / 12;
            });
        }
        #endregion

        #region variant 2
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            sum2 = 100;
            progressBar2.Value = 0;
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent2).Start();
            }
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {

        }

        private double sum2;
        private readonly object locker2 = new();     // объект для синхронизации

        private void plusPercent2()
        {
            double val;
            lock (locker2)                           // синхро-блок
            {                                        // поток, который первым входит в
                val = sum2;                          // блок, закрывает locker2 и открывает
                Thread.Sleep(random.Next(250, 350)); // его по выходу из блока.
                double percent = 10;                 // Другие потоки, дойдя до lock
                val *= 1 + percent / 100;            // видят, что объект закрыт и переходят
                sum2 = val;                          // в ждущее состояние до его открытия.
            }                                        // Первый из дождавшихся снова его закроет и т.д.

            Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += val + "\n";
                progressBar2.Value += 100.0 / 12;
            });
        }
        #endregion
    }
}
