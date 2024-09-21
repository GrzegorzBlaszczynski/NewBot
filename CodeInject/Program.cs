using Winebotv2;
using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using Reloaded.Injector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace ISpace
{

    public class IClass
    {
        public unsafe static int Main()
        {
          Start startDialog = new Start();
          startDialog.ShowDialog();

          LuigiPipe p= LuigiPipe.Instance;

           while (!p.isReady) { }
           cBot cBot = new cBot();
           cBot.ShowDialog();
 
            return 0;
        }

        [DllImport("kernel32")]
        static extern bool AllocConsole();
    }
}
