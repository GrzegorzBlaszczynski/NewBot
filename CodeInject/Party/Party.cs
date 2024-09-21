using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using System.Collections.Generic;

namespace Winebotv2.Party
{
    public unsafe class Party
    {
        public List<PartyMember> PartyMemberList { get; set; }
        private long* PartyMemberDataAddres;

        public void Update()
        {
            PartyMemberList = Player.GetPlayer.GetPartyMembersList();
        }

        public Party() {

        }

    }
}
