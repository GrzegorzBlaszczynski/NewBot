using CodeInject.MemoryTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace CodeInject.Actors
{
    public unsafe class NPC : IObject
    {
        
        #region Properties
        public long ObjectPointer { get; set; }
        public ushort ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Hp
        {
            get
            {
                return *(int*)(ObjectPointer + 0xF0);
            }
        }
        public int MaxHp { get; set; }
        public string* Name { get; set; }
        #endregion


        public NPC(long* Entry)
        {

            ObjectPointer = (long)((long*)*Entry);

            X = *(float*)(*Entry + 0x10);
            Y = *(float*)(*Entry + 0x14);
            Z = *(float*)(*Entry + 0x18);

            if ((long*)(*Entry + 0x28) != null)
                ID = *(ushort*)(*((long*)(*Entry + 0x20)));

            MaxHp = *(int*)(*Entry + 0xF8);

        }
        public static List<IObject> GetNPCsList()
        {
            return GameHackFunc.Game.ClientData.GetNPCs();
        }
        public double CalcDistance(IObject targetObject)
        {
            return Math.Sqrt(Math.Pow((targetObject.X / 100) - (this.X / 100), 2) + Math.Pow((targetObject.Y / 100) - (this.Y / 100), 2) + Math.Pow((targetObject.Z / 100) - (this.Z / 100), 2));
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