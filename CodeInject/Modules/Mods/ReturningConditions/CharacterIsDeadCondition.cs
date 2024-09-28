using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winebotv2.Actors;

namespace CodeInject.Modules.Mods.ReturningConditions
{
    internal class CharacterIsDeadCondition : ICondition
    {
        public bool IsFulfilled { get; set; }

        public void Update()
        {
            if (Player.GetPlayer.Hp < 0)
                IsFulfilled = true;
            else
                IsFulfilled = false;
        }
    }
}
