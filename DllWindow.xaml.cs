using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for DllWindow.xaml
    /// </summary>
    public partial class DllWindow : Window
    {
        #region Basics
        [DllImport("User32.dll")]
        public static extern
            int MessageBoxA(
                IntPtr hWnd,        // HWND  
                String lpText,      // LPCSTR
                String lpCaption,   // LPCSTR
                uint   uType        // UINT  
            );

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBoxW(IntPtr hWnd, String lpText, String lpCaption, uint uType);

        
        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        public static extern bool Sound(uint dwFreq, uint dwDuration );

        /* 
         * HANDLE CreateThread(
              [in, optional]  LPSECURITY_ATTRIBUTES   lpThreadAttributes,
              [in]            SIZE_T                  dwStackSize,
              [in]            LPTHREAD_START_ROUTINE  lpStartAddress,
              [in, optional]  __drv_aliasesMem LPVOID lpParameter,
              [in]            DWORD                   dwCreationFlags,
              [out, optional] LPDWORD                 lpThreadId
            );
         */
        [DllImport("Kernel32.dll", EntryPoint = "CreateThread")]
        public static extern
            IntPtr CreateThread(
                    IntPtr lpThreadAttributes,  // указатель на структуру с параметрами безопасности (NULL)
                    uint   dwStackSize,         // граничный размер стека - 0 (по умолчанию)
              ThreadMethod lpStartAddress,      // указатель на стартовый адрес (функции)
                    IntPtr lpParameter,         // указатель на объект с параметрами для ф-ции
                    uint   dwCreationFlags,     // флаги запуска - 0 (по умолчанию)
                    IntPtr lpThreadId           // возврат id потока (NULL - не возвращать)
            );
        // главный вопрос - как получить адрес метода в .NET и передать его в неуправляемый код
        // 1. Описываем делегат по документации на функцию (CreateThread)
        public delegate void ThreadMethod();
        // 2. Заменяем IntPtr в декларации ф-ции на делегат ThreadMethod
        // 3. Описываем метод с сигнатурой делегата
        public void SayHello()
        {
            Dispatcher.Invoke(() => SayHelloLabel.Content = "Hello");
            sayHelloHandle.Free();  // по окончанию работы - освобождаем (расфиксируем) объект
        }
        // 4. При вызове ф-ции CreateThread в параметре lpStartAddress указываем SayHello
        //    (см. SayHelloButton_Click)
        #endregion

        #region MM Timer
        // делегат для передачи адреса периодически вызываемого метода
        delegate void TimerMethod(uint uTimerID, uint uMsg, ref uint dwUser,
            uint dw1, uint dw2);

        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        static extern uint TimeSetEvent(
            uint uDelay,
            uint uResolution,
            TimerMethod lpTimeProc,
            ref uint dwUser,
            uint eventType
            );

        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        static extern void TimeKillEvent(uint uTimerID);

        const uint TIME_ONESHOT = 0;  // eventType
        const uint TIME_PERIODIC = 1;

        uint uDelay;
        uint uResolution;
        uint timerId;
        uint dwUser = 0;
        TimerMethod timerMethod = null!;
        GCHandle timerHandle;

        int ticks;
        void TimerTick(uint uTimerID, uint uMsg, ref uint dwUser, uint dw1, uint dw2)
        {
            ticks++;
            Dispatcher.Invoke(() => { TicksLabel.Content = ticks.ToString(); });
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            uDelay = 100;      // задержка между вызовами 100 ms (10 Hz)
            uResolution = 10;  // допустимое отклонение (погрешность) для uDelay
            ticks = 0;
            timerMethod = new TimerMethod(TimerTick);
            timerHandle = GCHandle.Alloc(timerMethod);
            timerId = TimeSetEvent(uDelay, uResolution, timerMethod, ref dwUser, TIME_PERIODIC);
            if(timerId != 0)
            {
                StopTimer.IsEnabled = true;
                StartTimer.IsEnabled = false;
            }
            else
            {
                timerHandle.Free();
                timerMethod = null!;
            }
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            TimeKillEvent(timerId);
            timerHandle.Free();
            StopTimer.IsEnabled = false;
            StartTimer.IsEnabled = true;
        }
        /* Мультимедийный таймер:
         * Добавить событие закрытия окна, в нем проверить активность таймера - 
         *  если активный, то выключить (и освободить объект)
         * Реализовать "часы-таймер" в форме 00:00:00.00 (чч:мм:сс.дс)
         * дс - две цифры для сотых долей секунды
         * Запустить таймер с интервалом 10 мс, обработать его тики в виде времени
         */

        #endregion

        public DllWindow()
        {
            InitializeComponent();
        }

        #region Basics methods
        private void MsgA_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(
                IntPtr.Zero,  // NULL
                "Message",    // неявная маршализация - передача строки
                "Title",      // из управляемого кода в неуправляемый
                1
            );
        }

        private void MsgW_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxW(IntPtr.Zero, "Message","Title", 1);
        }

        private void Msg1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(IntPtr.Zero, "Повторить попытку соединения", 
                "Соединение не установлено", 0x35);
        }

        private void ErrorAlert(String message)
        {
            MessageBoxW(IntPtr.Zero, message, "ErrorAlert", 0x10);
        }

        private void MsgError_Click(object sender, RoutedEventArgs e)
        {
            ErrorAlert("Ошибка");
        }
        private bool? ConfirmMessage(String message)
        {
            int res = MessageBoxW(IntPtr.Zero, message, "", 0x46);
            return res switch
            {
                11 => true,  // Continue
                10 => false, // Try Again
                _ => null    // Cancel (res=2)
            };
        }

        private void MsgConfirm_Click(object sender, RoutedEventArgs e)
        {
            ConfirmMessage("Процесс занимает много времени");
        }

        private void Beep1_Click(object sender, RoutedEventArgs e)
        {
            Sound(420, 250);
        }

        GCHandle sayHelloHandle;
        private void SayHelloButton_Click(object sender, RoutedEventArgs e)
        {
            // CreateThread(IntPtr.Zero, 0, SayHello, IntPtr.Zero, 0, IntPtr.Zero);
            // Потенциальная проблема - сборщик мусора. При работе он дефрагментирует
            //  память, перенося объекты между поколениями
            // [.][..][.][.x.][..][.] ==> [.][..][.]     [..][.] ==> [.][..][.][..][.]
            //                                                                 эти два
            // объекта поменяют свой адрес в памяти
            // Необходимо "сказать" сборщику мусора о том, что объект не нужно перемещать
            // Для того чтобы не "фиксировать" целое окно, отделим метод в новый объект
            var sayHelloObject = new ThreadMethod(SayHello);
            // и укажем сборщику мусора (GC) разместить этот объект на постоянном месте
            sayHelloHandle = GCHandle.Alloc(sayHelloObject);
            // передаем в неуправляемый код ссылку на объект sayHelloObject
            CreateThread(IntPtr.Zero, 0, sayHelloObject, IntPtr.Zero, 0, IntPtr.Zero);
            // долго удерживать объекты на одном месте нежелательно, после использования
            // нужно их "расфиксировать" - см. SayHello()
        }
        #endregion

       
    }
}
// Создать метод  bool Ask(String message): пиктограмма вопроса и две кнопки YES-NO
// Вывести сообщение "Подтверждаете действие?" и по результатам выбора вывести
// обычное сообщение "Действие подтверждено" (!)  либо "Действие отменено" (х)