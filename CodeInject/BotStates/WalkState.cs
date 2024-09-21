using Winebotv2.AutoReStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winebotv2.BotStates
{
    internal class WalkState : IBotState
    {
        int point = 0;
        public List<Points> Path;
        public WalkState(List<Points> path)
        { 

        }

        public void Work(BotContext context)
        {
           
        }

        private void Finish()
        {
            point = 0;
        }
    }
}
