using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml.Linq;

namespace SystemProgramming_111
{
    /// <summary>
    /// Interaction logic for ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private Dictionary<string, List<Process>> processDict = new();

        public ProcessWindow()
        {
            InitializeComponent();
        }

        private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        {
            ShowProcesses.IsEnabled = false;
            new Thread(UpdateProcesses).Start();
        }
        private void UpdateProcesses()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                List<Process> list;
                if(processDict.ContainsKey(process.ProcessName))  // процесс с этим именем уже в словаре
                {
                    list = processDict[process.ProcessName];                    
                    list.Add(process);
                }
                else   // нет такого имени в словаре 
                {
                    list = new List<Process>();
                    list.Add(process);
                    processDict[process.ProcessName] = list;
                }
            }
            sw.Stop();

            Dispatcher.Invoke(() =>
            {
                timeElapsed.Content = sw.ElapsedTicks + " tck";
                treeView.Items.Clear();
                foreach (var pair in processDict)
                {
                    TreeViewItem node = new() { Header = pair.Key };

                    foreach (Process process in pair.Value)
                    {
                        TreeViewItem subnode = new() { Header = process.Id };
                        node.Items.Add(subnode);
                    }

                    treeView.Items.Add(node);
                }

                ShowProcesses.IsEnabled = true;
            });
        }
        private Process notepadProcess;
        private void StartNotepad_Click(object sender, RoutedEventArgs e)
        {
            notepadProcess = Process.Start("C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe",
                "-url itstep.org");
            // notepadProcess = Process.Start("notepad.exe", 
            //     @"C:\Users\_dns_\source\repos\SystemProgramming-111\Notes\Processes.txt");
            
            if(notepadProcess is not null)
            {
                StopNotepad.IsEnabled = true;
                StartNotepad.IsEnabled = false;
            }
        }

        private void StopNotepad_Click(object sender, RoutedEventArgs e)
        {
            if (notepadProcess is not null)
            {
                notepadProcess.CloseMainWindow();
                notepadProcess.Kill();
                notepadProcess.WaitForExit();

                StopNotepad.IsEnabled = false;
                StartNotepad.IsEnabled = true;

                notepadProcess = null!;
            }
        }
    }
}
/* Д.З. Процессы.
 * Запуск блокнота: Добавить кнопку выбора файла, при запуске блокнота
 *  передавать ему выбранный файл.
 * Запуск браузера: добавить поле ввода URL, запускать браузер на данной странице 
 */
/* О проверках и исключениях:
* проверить наличие ключа в словаре можно двумя путями
* а) if(processDict.ContainsKey(...))
* б) try {processDict[...]} catch{...}
* Проверка времени их работы показала, что 
* а) 94 ns 
* б) 3 300 000 ns
* Вывод: использовать исключения в только самых необходимых случаях
* Избегать использования в циклах
*/

/*
 * svchost
 *  - svchost 14
 *  - svchost 21
 *  - svchost 112
 * msedge
 *  - msedge 231
 *  - msedge 193
 *  
 */