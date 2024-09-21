using Winebotv2.Actors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Winebotv2.BotStates
{
    public class PickUpState : IBotState
    {
        public PickUpState()
        {
            
        }

        public void Work(BotContext context)
        {
            List<IObject> itemsAround = context.GetItemsNearby();
            if (itemsAround.Count > 0)
                ((Item)itemsAround[0]).PickUp();
            else
                context.SetState("HUNT");
        }
    }
}
