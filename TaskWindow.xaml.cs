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


        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            // О задачах детальнее (Demo 1)
            // Задача представляет собой одно выполнение одного метода/функции
            Task task1 = new Task(proc1);   // создаем объект, выполнение не запускается
            // task1.RunSynchronously();    // запуск в синхронном контексте
            task1.Start();                  // запуск асинхронно (здесь - в новом потоке)
            Task.Run(proc1);                // тоже асинхронный запуск - параллельно с предыдущим
        }
        private void proc1()
        {
            ConsoleWrite("proc1 started\n");
            Thread.Sleep(1000);
            ConsoleWrite("proc1 finished\n");
        }
        private void ConsoleWrite(Object item)
        {
            this.Dispatcher.Invoke( () => 
                ConsoleBlock.Text += item is null ? "NULL" : item.ToString());
        }
        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {
            // О задачах детальнее (Demo 2)
            // Варианты запуска задач по очереди:
            // - синхронный запуск
            // - ожидание
            // - продолжение
            Task task1 = new Task(procN, 1);   // параметр для метода - в конструкторе
            Task task2 = new Task(procN, 2);

            // task1.Start();  // Такой запуск - параллельный, обе сразу начинают работать
            // task2.Start();  // Причем иногда надпись task2 появляется раньше, чем task1

            task1.RunSynchronously();  // Работают последовательно, но первая задача
            task2.Start();             // блокирует UI, вторая - нет

            /* Click - task1.RS[-proc(1)-] - task2.Start() - end (UI свободен)
             *                                         \
             *                                          proc(2)......
             */
        }
        private void procN(object? item)
        {
            ConsoleWrite($"proc{item?.ToString()} started\n");
            Thread.Sleep(1000);
            ConsoleWrite($"proc{item?.ToString()} finished\n");
        }
        private async void ButtonDemo3_Click(object sender, RoutedEventArgs e)
        {
            // О задачах детальнее (Demo 3)
            // Варианты запуска задач по очереди: ожидание
            Task task1 = new Task(procN, 1); 
            Task task2 = new Task(procN, 2);
            task1.Start();
            // task1.Wait();   // Зависание: см Task.txt - UI vs Dispatcher
            await task1;       // без зависания (в сигнатуру метода добавляем async)
            task2.Start();
            /* Button (UI) - UI свободен     Suspending mode - поток блокируется и ожидает
             *    \                         ___|___
             *    async Click  task1.Start()       await task1; task2.Start() - end
             *                   \               /                     \
             *                     proc(1)......                        proc(2)......
             */
        }
        private void ButtonDemo4_Click(object sender, RoutedEventArgs e)
        {
            // О задачах детальнее (Demo 3)
            // Варианты запуска задач по очереди: продолжение
            Task task1 = new Task(procN, 1);
            Task task2 = new Task(procN, 2);

            task1.ContinueWith(_ => task2.Start())  // указываем, что после окончания task1 нужно выполнить доп.действие
            .ContinueWith(_ => new Task(procN, 3).Start());

            task1.Start();

            /* Эта схема также называется "ниткой"
             * Click -  task1.Start() - end ( UI свободен )
             *                       \ 
             *                        proc(1) -- task2.Start() -- new Task(procN, 3).Start()
             */

        }

        private void ButtonDemo1_2_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWrite("funcN(1) started");

            var task1 = funcN(1);   // вызов, но возрат - Task, отвечающий за исполнение
                                    // аналог task1 = new(funcN); task1.Start()

            ConsoleWrite(           // .Result также комбинация - .Wait() - .GetResult()
                task1.Result);      // .Wait приводит к зависанию из-за Dispatcher
        }
        private async Task<String> funcN(int N)  // тип функции Task<String>
        {                                        // 
            await Task.Delay(1000);              // 
            return $"funcN({N}) result\n";       // возврат - просто String
        }
        private async void ButtonDemo2_2_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWrite("funcN(2) started\n");   // funcN(2) - возврат Task<String>
            ConsoleWrite(await funcN(2));         // await - "извлекает" String (.Result)
        }

        private async void ButtonDemo3_2_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWrite("funcN(1) started\n");  // Выполнение последовательное
            ConsoleWrite(await funcN(1));        // await - ожидает завершения

            ConsoleWrite("funcN(2) started\n");  // эти команды выполнятся после
            ConsoleWrite(await funcN(2));        // окончания funcN(1)
        }

        private async void ButtonDemo4_2_Click(object sender, RoutedEventArgs e)
        {
            // указание await funcN(1) не дает их запускать параллельно
            Task<String> task1 = funcN(1);   // запуск + возврат задачи
            Task<String> task2 = funcN(2);   // запуск параллельно с task1
            ConsoleWrite("funcN(1) started\n");  // этот код выполняется во время работы
            ConsoleWrite("funcN(2) started\n");  // запущенных выше задач
            // String res1 = task1.Result; - зависание (при синхронном методе)
            ConsoleWrite(await task1);
            ConsoleWrite(await task2);
        }

        private async void ButtonDemo5_2_Click(object sender, RoutedEventArgs e)
        {
            // работа с множеством задач
            Task<String>[] tasks = new Task<String>[7];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = funcN(i);   // запуск 7 задач параллельно
            }
            ConsoleWrite("funcs started\n");
            // Task.WaitAll(tasks);  // ожидание окончания всех задач
            // await Task.WhenAll(tasks);

            // Task.WaitAny(tasks);  // ожидание окончания любой одной 
            // await Task.WhenAny(tasks);

            foreach (var task in tasks)
            {
                ConsoleWrite(await task);
            }
        }
    }
}
/* Д.З. Реализовать решение задачи рассчета годовых процентов при помощи многозадачности
 * Использовать случайную задержку 250-350 мс
 * Отображать номер месяца, процент и полученный результат, а также двигать индикатор прогресса
 * !! Реализовать функцию получения процента в асинхронном виде
 *     получаем номер месяца, случайная задержка, возврат процента, зависящего от месяца
 */