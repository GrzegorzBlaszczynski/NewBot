using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInject.BrainlessScript.Commands
{
    public class Command
    {
       public string Type { get; set; }
       public bool IsFinished  = false;

        public Command(string type)
        {
            Type = type;
        }
    }
}
