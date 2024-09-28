using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInject.BrainlessScript.Commands
{
    internal interface ICommand
    {
         string Type { get; set; }
         void Execute();
    }
}
