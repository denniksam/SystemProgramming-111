using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for HookWindow.xaml
    /// </summary>
    public partial class HookWindow : Window
    {
        #region API and DLL

        // сигнатура для хук-процедуры: нашего обработчика системных событий
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        // установщик хука - вызов ф-ции встроит наш обработчик в систему
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(
            int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        // отмена хука - исключение нашего обработчика из системы
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        // передача управления следующему хуку (перед которым мы встроили свой код)
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(
            IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

        // определение адреса модуля - исполнимого кода нашей программы
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(String? lpModuleName);

        private const int
            WH_KEYBOARD_LL = 13,    // Номер "ячейки" прерывания клавиатуры
            WM_KEYDOWN = 0x0100;    // Номер сообщения нажатия кнопки

        #endregion

        private IntPtr kbHook;   // сохраненный адрес старого обработчика

        private IntPtr kbHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                int vkCode =             // Маршализация - передача данных (Int32)
                    Marshal              // из адреса (lParam) в управляемый код
                    .ReadInt32(lParam);  // (int vkCode)

                HookLogs.Text += vkCode.ToString();
            }
            return CallNextHookEx(kbHook, nCode, wParam, lParam);
        }


        public HookWindow()
        {
            InitializeComponent();
        }

        private void StartKbHook_Click(object sender, RoutedEventArgs e)
        {
            using Process thisProcess = Process.GetCurrentProcess();
            using ProcessModule? thisModule = thisProcess.MainModule;
            if(thisModule is null)
            {
                HookLogs.Text += "Error in MainModule\n";
                return;
            }
            kbHook = SetWindowsHookEx(     // Принцип выталкивания - новый адрес
                WH_KEYBOARD_LL,            // устанавливается, а старый возвращается
                kbHookCallback,            // и сохраняется в kbHook
                GetModuleHandle(thisModule.ModuleName),
                0);
            HookLogs.Text += "Hook Activated\n";
        }

        private void StopKbHook_Click(object sender, RoutedEventArgs e)
        {
            UnhookWindowsHookEx(kbHook);
            HookLogs.Text += "Hook Deactivated\n";
        }
    }
}
