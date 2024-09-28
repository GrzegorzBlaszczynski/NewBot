using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Winebotv2.Actors;
using Winebotv2.MemoryTools;

namespace CodeInject.BrainlessScript.Commands
{
    public class GoCommand : Command,ICommand, ICommandUpdater
    {
        
        public float DestinationX, DestinationY;
        Timer retyWatch = new Timer(1000.0f);

        public GoCommand():base("GoCommand")
        {
            retyWatch.Stop();
            retyWatch.Elapsed += (e, o) =>
            {
                Execute();
            };
          
        }

        public void Execute()
        {

            retyWatch.AutoReset = true;

            GameHackFunc.Game.Actions.MoveToPoint(new System.Numerics.Vector2(DestinationX / 100, DestinationY / 100));
            base.IsFinished = false;
            retyWatch.Start();
        }

        public override string ToString()
        {
            return $"GO TO {DestinationX},{DestinationY}";
        }

        public void Update()
        {
            Player pl = GameHackFunc.Game.ClientData.GetPlayer();
            if (Vector2.Distance(new Vector2(pl.X,pl.Y),new Vector2(DestinationX,DestinationY))<3)
            {
                base.IsFinished = true;
                retyWatch.Stop();
            }
        }
    }
}
