using Winebotv2.MemoryTools;
using Reloaded.Injector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winebotv2
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void Start_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (var client in System.Diagnostics.Process.GetProcessesByName("trose"))
            {
                bool alreadyInjected = false;
               foreach(ProcessModule module  in client.Modules)
                {
                    if (module.ModuleName.Contains("SharpNativeDLL"))
                    {
                        alreadyInjected = true;
                        break;
                    }
                }

               if(alreadyInjected==false)
                listBox1.Items.Add(client.Id);
            }
        }
        long addr = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            Injector inject = new Injector(System.Diagnostics.Process.GetProcessById(int.Parse(listBox1.SelectedItem.ToString())));
        
            Thread th = new Thread(new ThreadStart(() => {
                 addr = inject.Inject(System.AppDomain.CurrentDomain.BaseDirectory + "\\SharpNativeDLL.dll");
            }));
            th.Start();
            if (addr != 0)
            {
                Rudy.Instance.OpenProcess(int.Parse(listBox1.SelectedItem.ToString()));
                LuigiPipe.Instance.OpenPipe(int.Parse(listBox1.SelectedItem.ToString()));
                this.Close();
            }
        }

        private void Start_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(addr==0)
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
