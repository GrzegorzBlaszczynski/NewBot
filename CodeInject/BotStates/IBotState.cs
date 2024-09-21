using System;
using System.Collections.Generic;

namespace Winebotv2.BotStates
{
    public interface IBotState
    {
        void Work(BotContext context);
    }
}
