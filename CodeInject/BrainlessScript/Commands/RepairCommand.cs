using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Winebotv2;
using Winebotv2.Actors;
using Winebotv2.MemoryTools;

namespace CodeInject.BrainlessScript.Commands
{
    public class RepairCommand : Command, ICommand, ICommandUpdater
    {
        public string NPCName {  get; set; }
        public RepairCommand() : base("RepairCommand")
        {
        }

        public void Execute()
        {
            IObject temp = GameHackFunc.Game.ClientData.GetNPCs().Where(x => x.GetType() == typeof(NPC)).FirstOrDefault(x => ((NPC)x).Info.Name == NPCName);
            List<InvItem> inventory = GameHackFunc.Game.ClientData.GetFullInventoryItemsWithSlots();

            for (int i = 1; i < 13; i++)
            {
                if (inventory[i].ItemData == 0) continue;

                GameHackFunc.Game.Actions.RepairItemWithNPC(temp.ID, inventory[i].NetworkID);
            }

            base.IsFinished = true;
        }

        public override string ToString()
        {
            return $"Repair EQ in {NPCName}";
        }

        public void Update()
        {

        }
    }
}
