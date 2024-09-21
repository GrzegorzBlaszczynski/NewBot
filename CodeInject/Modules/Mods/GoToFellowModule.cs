using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.MonthCalendar;

namespace Winebotv2.Modules.Mods
{
    /// <summary>
    /// Go to other player if no monster around
    /// </summary>
    internal class GoToFellowModule : BackToCenterModule, IModule
    {
        public string Name { get; set; } = "GoToFellow";
        public string FollowPlayerName { get; set; }

        public GoToFellowModule(List<MobInfo> monstersToAttackList, string followPlayerName, Vector3 huntArea, float radius) :base(monstersToAttackList,huntArea,radius)
        {
            FollowPlayerName = followPlayerName;
        }

        public unsafe void Update()
        {
            IPlayer fPlayer = (IPlayer)NPC.GetNPCsList().FirstOrDefault(x => x.GetType() == typeof(OtherPlayer) && ((IPlayer)x).Name == FollowPlayerName);

            if (fPlayer == null) return;

            if (!NPC.GetNPCsList().Where(x => x.GetType() == typeof(NPC))
                  .Where(x => base.MonstersToAttackList.Cast<MobInfo>().Any(y => ((NPC)x).Info != null && y.ID == ((NPC)x).Info.ID))
                  .Where(x => ((NPC)x).CalcDistance(CenterPosition.X, CenterPosition.Y, CenterPosition.Z) < Radius).Any(x => (((NPC)x).Hp) > 0)) //Walk to character if there is no more monsters
                    {
                            Player.GetPlayer.WalkToPoint(new System.Numerics.Vector2(fPlayer.X / 100 + new Random().Next(-2,2), fPlayer.Y / 100 + new Random().Next(-2, 2)));
                    }
        }
    }
}
