using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winebotv2.MemoryTools;

namespace CodeInject.BrainlessScript.Commands
{
    public class ConfirmshopingCommand : Command, ICommand, ICommandUpdater
    {
        public ConfirmshopingCommand():base("ConfirmshopingCommand")
        {

        }

        public void Execute()
        {
            GameHackFunc.Game.Actions.ConfirmBuyingStack();
            base.IsFinished = true;
        }

        public override string ToString()
        {
            return $"CONFIRM SHOP LIST";
        }

        public void Update()
        {

        }
    }
}
