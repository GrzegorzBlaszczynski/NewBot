using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winebotv2.MemoryTools;

namespace CodeInject.BrainlessScript.Commands
{
    public class BuyCommand : Command, ICommand, ICommandUpdater
    {
        public int Index { get; set; }
        public int Count { get; set; }


        public BuyCommand():base("BuyCommand")
        {

        }

        public void Execute()
        {
            GameHackFunc.Game.Actions.PutItemToBuy(Index, Count);
            base.IsFinished = true;
        }

        public override string ToString()
        {
            return $"BUY ITEM AT {Index} COUNT:{Count}";
        }

        public void Update()
        {

        }
    }
}
