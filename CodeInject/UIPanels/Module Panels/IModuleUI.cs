using Winebotv2.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winebotv2.UIPanels.Module_Panels
{
    internal interface IModuleUI
    {
        string DisplayName { get; }
        IModule GetModule();
    }
}
