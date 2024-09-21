using System.Collections.Generic;

namespace Winebotv2.WebServ.Models.PickUpFilter
{
    public class SimpleFilterModel : IPickupFilterModel
    {
        public string Name { get; set; } = "Simple";
        public List<ItemType> Filter { get; set; }
    }
}
