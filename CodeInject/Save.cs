using Winebotv2.Modules;
using Winebotv2.WebServ.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winebotv2
{
    public class Save
    {

        private unsafe string SaveAutopotion(BotContext contex)
        {
            AutoPotionModule module = contex.GetModule<AutoPotionModule>("AUTOPOTION");

            AutoPotionSettingsModel model = new AutoPotionSettingsModel();
            model.HealthItemIndex = (int)module.AutoHp.Item2Cast.ItemData;
            model.ManaItemIndex = (int)module.AutoMp.Item2Cast.ItemData;

            model.MinHelath = module.AutoHp.MinValueToExecute;
            model.MinMana = module.AutoMp.MinValueToExecute;

            model.HelathDurration = module.AutoHp.CooldDown;
            model.ManaDurration = module.AutoMp.CooldDown;

            return JsonConvert.SerializeObject(model);
        }
    }
}
