using Winebotv2.Actors;
using Winebotv2.Hunt;
using Winebotv2.MemoryTools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace Winebotv2.BotStates
{
    public class HuntState : IBotState
    {
        public IHuntSetting HuntInstance;
  
        public HuntState(IHuntSetting huntInstance)
        {
            HuntInstance = huntInstance;
        }


        public unsafe void Work(BotContext context)
        {
            if(Player.GetPlayer.Hp <=0)
            {
                context.SetState("STANDBY");
            }


            if(context.GetItemsNearby().Count > 0)
            {
                context.SetState("PICK");
                return;
            }
            HuntInstance.Update();
        }
    }
}
