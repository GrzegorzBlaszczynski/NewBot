using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winebotv2.Modules
{
    unsafe class FollowModule : IModule
    {
        public string Name { get; set; } = "FOLLOW";

        public string FollowPlayerName {  get; set; }

        public FollowModule(string folowPlayerName) {
            FollowPlayerName = folowPlayerName;
        }

        public void Update()
        {
            IPlayer fPlayer = (IPlayer)NPC.GetNPCsList().FirstOrDefault(x => x.GetType() == typeof(OtherPlayer) && ((IPlayer)x).Name == FollowPlayerName);

            if(fPlayer != null)
            {
               Player.GetPlayer.WalkToPoint(new System.Numerics.Vector2(fPlayer.X/100, fPlayer.Y/100));
            }
        }
    }
}
