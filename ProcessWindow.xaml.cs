using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                List<Process> list;
                if(processDict.ContainsKey(process.ProcessName))
                {
                    list = processDict[process.ProcessName];
                    // если нет исключения, то процесс с этим именем уже в словаре
                    list.Add(process);
                }
                else   // исключение - если нет такого имени в словаре
                {
                    list = new List<Process>();
                    list.Add(process);
                    processDict[process.ProcessName] = list;
                }
            }

            foreach(var pair in processDict)
            {
                TreeViewItem node = new() { Header = pair.Key };

                foreach(Process process in pair.Value)
                {
                    TreeViewItem subnode = new() { Header = process.Id };
                    node.Items.Add(subnode);
                }

                treeView.Items.Add(node);
            }
            /*
             foreach (Process process in processes)
            {
                TreeViewItem node = new TreeViewItem();
                node.Header = process.ProcessName;
                node.Tag = process;

                TreeViewItem subnode = new TreeViewItem();
                subnode.Header = process.Id;

                node.Items.Add(subnode);

                treeView.Items.Add(node);
            }*/
        }
    }
}
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