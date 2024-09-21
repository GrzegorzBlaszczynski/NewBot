using Winebotv2.Actors;

namespace Winebotv2.PickupFilters
{
    public interface IFilter
    {
        bool CanPickup(IObject item);
    }
}
