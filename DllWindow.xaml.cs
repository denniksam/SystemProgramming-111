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

        /* Д.З. Реализовать несколько кнопок, вызывающих функцию Beep
         * из "Kernel32.dll" с разной частотой звука.
         * Запускать проигрывание в отдельной задаче/потоке
         */


        public DllWindow()
        {
            InitializeComponent();
        }

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
    }
}
// Создать метод  bool Ask(String message): пиктограмма вопроса и две кнопки YES-NO
// Вывести сообщение "Подтверждаете действие?" и по результатам выбора вывести
// обычное сообщение "Действие подтверждено" (!)  либо "Действие отменено" (х)