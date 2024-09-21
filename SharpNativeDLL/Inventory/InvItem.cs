using CodeInject.MemoryTools;
using System.Linq;


namespace CodeInject
{
    public unsafe class InvItem
    {
        public long* ObjectPointer { get; set; }
        public long* CItemAddr { get; set; }
        public short* ItemData { get; set; }

        /// <summary>
        /// 0x08 Weapon
        /// 0x09 Shield
        /// 0xA - usable items potions etc
        /// 0x02 - hat
        /// 0x03 - chest armor
        /// 0x04 - gloves
        /// 0x05 - shoes
        /// 0x0C - Material
        /// </summary>
        public short* ItemType { get; set; }

        public InvItem(long* ItemDBAddr, long* cItemAddr)
        {
            ObjectPointer = ItemDBAddr;
            CItemAddr = cItemAddr;

            ItemData = (short*)((long)ItemDBAddr + 0x0c);
            ItemType = (short*)((long)ItemDBAddr + 0x08);
            CItemAddr = cItemAddr;
        }

    

        public void UseItem()
        {
            GameHackFunc.Game.Actions.ItemUse((long)CItemAddr);
        }


    }
}
