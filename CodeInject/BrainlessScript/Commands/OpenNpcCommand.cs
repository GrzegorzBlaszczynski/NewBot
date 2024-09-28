using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winebotv2.Actors;
using Winebotv2.MemoryTools;

namespace CodeInject.BrainlessScript.Commands
{
    public class OpenNpcCommand:Command,ICommand,ICommandUpdater
    {
        public string NPCName { get; set; }

        public OpenNpcCommand():base("OpenNpcCommand")
        {

        }

        public void Execute()
        {
            IObject temp = GameHackFunc.Game.ClientData.GetNPCs().Where(x => x.GetType() == typeof(NPC)).FirstOrDefault(x => ((NPC)x).Info.Name == NPCName);
            GameHackFunc.Game.Actions.OpenShop(temp.ID);

            base.IsFinished = true;
        }

        public override string ToString()
        {
            return $"OPEN SHOP {NPCName}";
        }

        public void Update()
        {

        }
    }
}
