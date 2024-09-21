using System.Collections.Generic;

namespace Winebotv2.WebServ.Models
{
    public class AutoPotionSettingsModel
    {
        public List<ItemModel> ItemsList = new List<ItemModel>();
        public int MinHelath;
        public int MinMana;
        public int HealthItemIndex;
        public int ManaItemIndex;
        public int HelathDurration;
        public int ManaDurration;
    }
}
