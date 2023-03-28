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

        private IntPtr   kbHook;   // сохраненный адрес старого обработчика
        private HookProc kbHookProc;  // объект, который будет зафиксирован от перемещений
        private GCHandle kbHookHandle;  // дескриптор зафисиксированного объекта

        [StructLayout(LayoutKind.Sequential)]
        struct KBDLLHOOKSTRUCT  // структура, по которой передаются данные от kbd - клавиатуры
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        private IntPtr kbHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                int vkCode =               // Маршализация - передача данных (Int32)
                    Marshal                // из адреса (lParam) в управляемый код
                    .ReadInt32(lParam);    // (int vkCode)

                KBDLLHOOKSTRUCT keyData =  // Полная версия - маршализация структуры
                    Marshal                // Краткая форма ReadInt32(lParam) возможна
                    .PtrToStructure        // потому, что vkCode идет первым в структуре
                    <KBDLLHOOKSTRUCT>      // То есть считать 32 бита от начала структуры
                    (lParam);              // это и есть считать vkCode

                Key key = KeyInterop.KeyFromVirtualKey(vkCode);
                HookLogs.Text += key + " ";
                if(key == Key.LWin || key == Key.Space)
                {
                    HookLogs.Text += "(block)";
                    return (IntPtr)1;  // возврат "1" свидетельствует об окончании обработки - 
                    // следующие хуки не запускаются. Как результат нажатие кнопки игнорируется
                    // причем во всех окнах
                }                
            }

            return CallNextHookEx(kbHook, nCode, wParam, lParam);  // переход к следующему хуку
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

            kbHookProc = new HookProc(kbHookCallback);  // "отделяем" метод от окна в новый объект
            kbHookHandle = GCHandle.Alloc(kbHookProc);  // закрепляем - GC не будет перемещать объект

            kbHook = SetWindowsHookEx(     // Принцип выталкивания - новый адрес
                WH_KEYBOARD_LL,            // устанавливается, а старый возвращается
                kbHookProc,                // и сохраняется в kbHook
                GetModuleHandle(thisModule.ModuleName),
                0);
            HookLogs.Text += "Hook Activated\n";
        }

        private void StopKbHook_Click(object sender, RoutedEventArgs e)
        {
            UnhookWindowsHookEx(kbHook);
            kbHookHandle.Free();
            kbHookProc = null!;
            HookLogs.Text += "Hook Deactivated\n";
        }
    }
}
