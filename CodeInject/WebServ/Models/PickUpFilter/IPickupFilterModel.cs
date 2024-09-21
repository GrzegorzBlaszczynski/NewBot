using System.Collections.Generic;

namespace Winebotv2.WebServ.Models.PickUpFilter
{
    public  interface IPickupFilterModel
    {
        string Name { get; set; }
        List<ItemType> Filter { get; set; }
    }
}
