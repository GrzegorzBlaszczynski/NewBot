using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winebotv2.Modules;
using Winebotv2;
using Winebotv2.MemoryTools;

namespace CodeInject.Modules.Mods
{
    public class AutoRepairModule : IModule
    {
        List<InvItem> _items2Repair = new List<InvItem>();
        InvItem _repairingItem;
        float _repairWhenProc = 50;

        public string Name { get; set; } = "AUTOREPAIR";


        public AutoRepairModule(List<InvItem> items2Repair, InvItem repairingItem, float repairWhenProc)
        {
            this._items2Repair = items2Repair;
            this._repairWhenProc = repairWhenProc;
            this._repairingItem = repairingItem;
        }

        public void Update()
        {
            foreach (var item in _items2Repair)
            {
                if(item.DurabilityProcentage < _repairWhenProc)
                {
                    int t = item.DurabilityProcentage;
                    GameHackFunc.Game.Actions.RepairItem(_repairingItem, item);
                }
            }
        }


    }
}
