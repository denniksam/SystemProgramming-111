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
    /// Interaction logic for SynchroWindow.xaml
    /// </summary>
    public partial class SynchroWindow : Window
    {
        public SynchroWindow()
        {
            InitializeComponent();
        }

        #region 1. lock

        private void StartLock_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork1).Start(i);
            }
        }
        private readonly object locker = new();   // Вместо системного ресурса синхронизации
                                                  // мы используем возможности ссылочных типов -
                                                  // встроенное наличие "критической секции"
        private void DoWork1(object? state)
        {
            lock (locker)
            {
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            }
        }

        #endregion

        #region 2. Monitor
        private void StartMonitor_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork2).Start(i);
            }
        }
        private readonly object monitor = new();
        private void DoWork2(object? state)
        {
            try
            {
                Monitor.Enter(monitor);  // Вход в монитор == блокирование объекта monitor

                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");

            }
            finally
            {
                Monitor.Exit(monitor);  // Выход == разблокирование
            }
        }
        #endregion

        #region 3. Mutex
        private void StartMutex_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork3).Start(i);
            }
        }

        private Mutex mutex = new();

        private void DoWork3(object? state)
        {
            mutex.WaitOne();
            try
            {
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        #endregion

        #region 4. EventWaitHandle
        private void StartEWH_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork4).Start(i);
            }
            // тут можно сделать работу до начала работы потоков
            gates.Set();  // подать сигнал первого открытия
        }

        private EventWaitHandle gates = new AutoResetEvent(false);  // объект с авто-разблокировкой по событию, true - изначально открытый
        
        private void DoWork4(object? state)
        {
            gates.WaitOne();  // ожидание открытия "ворот"

            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");

            gates.Set();  // подать сигнал следующего открытия
        }
        #endregion

        #region 5. Semaphore
        private void StartSemaphore_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; ++i)
            {
                new Thread(DoWork5).Start(i);
            }
            semaphore.Release(2);   // изначально открыли 2 свободных места

            Task.Run(async () =>
            {
                await Task.Delay(200);  // Через паузу - открыли еще одно 
                semaphore.Release(1);   // свободное место
            });
        }

        // private Semaphore semaphore = new(3, 3);   // первая 3 - свободные места, вторая 3 - максимальное кол-во
        private Semaphore semaphore = new(0, 3);  // изначально нет свободных мест
        
        private void DoWork5(object? state)
        {
            semaphore.WaitOne();
            try
            {
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
            }
            finally
            {
                semaphore.Release(1);
            }
        }
        #endregion
    }
}
