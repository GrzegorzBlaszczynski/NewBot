using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Winebotv2.Modules
{
    unsafe class BackToCenterModule : IModule
    {
        public string Name { get; set; } = "BACKTOCENTER";

        protected Vector3 CenterPosition {  get; set; }
        protected float Radius { get; set; }
        protected List<MobInfo> MonstersToAttackList { get; set; }

        public BackToCenterModule(List<MobInfo> monstersToAttackList ,Vector3 position,float radius)
        {
            CenterPosition = position;  
            MonstersToAttackList = monstersToAttackList;
            Radius = radius;
        }

        public  void Update()
        { 
            if (!NPC.GetNPCsList().Where(x => x.GetType() == typeof(NPC))
                .Where(x => MonstersToAttackList.Cast<MobInfo>().Any(y => ((NPC) x).Info != null && y.ID == ((NPC)x).Info.ID))
                .Where(x => ((NPC) x).CalcDistance(CenterPosition.X, CenterPosition.Y, CenterPosition.Z) < Radius).Any(x => (((NPC) x).Hp) > 0))
            {
                GoToHuntingAreaCenter();
            }
        }

        private void GoToHuntingAreaCenter()
        {
            if (((int)Player.GetPlayer.X) != (int)CenterPosition.X &&
                 ((int)Player.GetPlayer.Y) != (int)CenterPosition.Y)
            {
                Player.GetPlayer.WalkToPoint(new Vector2(CenterPosition.X / 100, CenterPosition.Y / 100));
            }
        }


        public override string ToString()
        {
            return "Back To Center";
        }
    }
}
