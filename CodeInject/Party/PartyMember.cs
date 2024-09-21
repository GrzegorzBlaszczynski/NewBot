using Winebotv2.Actors;
using Winebotv2.MemoryTools;

namespace Winebotv2.Party
{
    public unsafe class PartyMember
    {
        public long MemberAddres { get; set; }

        /// <summary>
        /// Everything what got NPC definition
        /// </summary>
        public IObject PartyMemberObject { get; set; }
        public string MemberName { get; set;}

 

        public override string ToString()
        {
            NPC npce = (NPC)GameHackFunc.Game.ClientData.GetPartyMemberDetails(this);
            return (((NPC)PartyMemberObject).Hp) +" "+ MemberName;
        }
    }
}
