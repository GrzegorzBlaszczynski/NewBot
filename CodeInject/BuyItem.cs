using CodeInject.BrainlessScript.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeInject
{
    public partial class BuyItem : Form
    {
        BuyCommand command;

        public BuyItem(BuyCommand command)
        {
            InitializeComponent();
            this.command = command;
        }

        private void BuyItem_Load(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            command.Count = int.Parse(textBox1.Text);
            this.Close();
        }
    }
}
