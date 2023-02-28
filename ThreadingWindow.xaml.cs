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
        #region part 1 - "зависание интерфейса"
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
        #endregion

        #region part 2 - Исключение (обращение к другому потоку)
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            new Thread( Start1 ).Start();  //  - исключение (изменения из другого потока)
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region part 3 - Решение
        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        {
            isStopped = false;
            new Thread( Start3 ).Start();
        }

        private void ButtonStop3_Click(object sender, RoutedEventArgs e)
        {
            isStopped = true;
        }
        private bool isStopped;  // "общая" переменная, доступная всем потокам
        private void Start3()
        {
            for (int i = 0; i < 10 && !isStopped; i++)
            {
                this.Dispatcher.Invoke(() =>  // делегирование работы потоку окна
                {
                    progressBar3.Value = (i + 1) * 10;
                    ConsoleBlock.Text += i.ToString() + "\n";  // Console.WriteLine(i);
                });
                
                Thread.Sleep(300);
            }
        }
        #endregion

        #region part 4 - Взаимодействие потоков (продолжение прогресса)
        private void ButtonStart4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = false;
            // параметр для Start4 передается в вызов .Start(0)
            new Thread( Start4 ).Start(savedIndex4);
            if(savedIndex4 == 0)
            {
                ConsoleBlock.Text = "";
            }
        }
        private void ButtonStop4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = true;
        }
        private bool isStopped4;
        private int savedIndex4;  // при прерывании потока - храним последний i
        private void Start4(object? startIndex)
        {
            // при приеме параметра обязательно преобразовать его к
            // ожидаемому типу (и проверить на валидность)
            if(startIndex is int startFrom)
            {
                for (int i = startFrom; i < 10; i++)
                {
                    if(isStopped4)
                    {
                        savedIndex4 = i;
                        return;
                    }
                    this.Dispatcher.Invoke(() =>  // делегирование работы потоку окна
                    {
                        progressBar4.Value = (i + 1) * 10;
                        ConsoleBlock.Text += i.ToString() + "\n";
                    });

                    Thread.Sleep(300);
                }
                // если поток закончился (без прерываний) 
                savedIndex4 = 0;
            }
        }
        #endregion
        /* Д.З. Реализовать блокироку возможности запустить новый поток
         * пока не закончит работу предыдущий (в т.ч. прерыванием)
         * а) disable кнопки
         *   либо
         * б) сохранять ссылку на поток:  thread = new Thread(...)
         *    и проверять ее на null
         * Если поток прерывается (не закончившись) менять надпись
         * кнопки Start на Resume. По окончанию - Start
         */
    }
}
