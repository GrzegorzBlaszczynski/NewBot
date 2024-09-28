using CodeInject.BrainlessScript;
using CodeInject.BrainlessScript.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winebotv2.Actors;
using Winebotv2.MemoryTools;

namespace CodeInject
{
    public partial class ScriptRecorder : Form
    {

        BrainlessEngine _engine = new BrainlessEngine();


        public ScriptRecorder()
        {
            InitializeComponent();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            comboBox1.Items.AddRange(GameHackFunc.Game.ClientData.GetNPCs().Where(x => x.GetType() == typeof(NPC)).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            listBox1.Items.Add(new OpenNpcCommand()
            {
                NPCName = (comboBox1.SelectedItem as NPC).Info.Name
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player player = GameHackFunc.Game.ClientData.GetPlayer();
            listBox1.Items.Add(new GoCommand()
            {
                DestinationX = player.X,
                DestinationY = player.Y
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Stop();

            } else
            {
                _engine.LoadInstructions(listBox1.Items.OfType<ICommand>().ToList());
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _engine.Update();
        }

        private void AddItemToBut(object sender, EventArgs e)
        {
            BuyCommand cmd = new BuyCommand();
            cmd.Index =int.Parse(((Button)sender).Name.Substring(1));

            BuyItem buyingDialogBox = new BuyItem(cmd);
            buyingDialogBox.ShowDialog();

            listBox1.Items.Add(cmd);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(new ConfirmshopingCommand());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem == null) return;

            listBox1.Items.Add(new RepairCommand()
            {
                NPCName = (comboBox1.SelectedItem as NPC).Info.Name
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string serializedCommands = JsonConvert.SerializeObject(listBox1.Items.OfType<Command>(), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto 
            });

            File.WriteAllText("commands.json", serializedCommands);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            string jsonFromFile = File.ReadAllText("commands.json");

            var deserializedCommands = JsonConvert.DeserializeObject<List<Command>>(jsonFromFile, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            listBox1.Items.Clear();

            foreach (var command in deserializedCommands)
            {
               listBox1.Items.Add((Command)command);
            }
        }
    }
}
