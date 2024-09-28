using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInject.Modules.Mods.ReturningConditions
{
    internal interface ICondition
    {
        bool IsFulfilled { get; set; }
        void Update();
    }
}
