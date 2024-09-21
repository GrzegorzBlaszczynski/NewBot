using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using CodeInject.MemoryTools;

namespace CodeInject.Actors
{
    public unsafe class Item : IObject
    {
        #region Properties
        public long ObjectPointer { get; set; }
        public ushort ID { get; set; }
        public short ItemData { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

  
        /// <summary>
        /// 0x08 Weapon
        /// 0x09 Shield
        /// 0xA - usable items potions etc
        /// 0x02 - hat
        /// 0x03 - chest armor
        /// 0x07 - Accesories
        /// 0x04 - gloves
        /// 0x05 - shoes
        /// 0x0B - Gems
        /// 0x0C - Material
        /// </summary>
        public short ItemType { get;set; }
        #endregion

        private void Init(long* entry)
        {
            ObjectPointer = (long)entry;
            X = *(float*)((long)entry + 0x10);
            Y = *(float*)((long)entry + 0x14);
            Z = *(float*)((long)entry + 0x18);
            ID = *(ushort*)((long)entry + 0x1c);
            ItemData = *(short*)((long)entry + 0x6c);
            ItemType = *(short*)((long)entry + 0x68);
        }

        public Item(long* Entry)
        {
            Init(Entry);
        }

        public double CalcDistance(IObject targetObject)
        {
            return Math.Sqrt(Math.Pow((targetObject.X/100) - (this.X / 100), 2) + Math.Pow((targetObject.Y / 100) - (this.Y / 100), 2) + Math.Pow((targetObject.Z / 100) - (this.Z / 100), 2));
        }
        public double CalcDistance(float x, float y, float z)
        {
            return Math.Sqrt(
                  Math.Pow((x / 100) - (this.X / 100), 2)
                + Math.Pow((y / 100) - (this.Y / 100), 2)
                + Math.Pow((z / 100) - (this.Z / 100), 2));
        }


    }
}
