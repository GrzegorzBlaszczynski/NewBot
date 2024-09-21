using Winebotv2.MemoryTools;
using Winebotv2.WebServ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace Winebotv2.Actors
{
    public unsafe class NPC : IObject
    {
        
        #region Properties
        public long ObjectPointer { get; set; }
        public ushort ID { get; set; }
        public MobInfo Info { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Hp
        {
            get
            {
                return Rudy.Instance.ReadInt(new UIntPtr((ulong)ObjectPointer) + 0xF0);
            }
        }
        public int MaxHp { get; set; }
        public string* Name { get; set; }
        #endregion


        public NPC(ulong Entry)
        {

            ObjectPointer =(long)Entry;

            X =  Rudy.Instance.ReadFloat(new UIntPtr(Entry) + 0x10);
            Y =  Rudy.Instance.ReadFloat(new UIntPtr(Entry) + 0x14);
            Z = Rudy.Instance.ReadFloat(new UIntPtr(Entry) + 0x18);

            ID = Rudy.Instance.ReadUShort(new UIntPtr(Entry) + 0x1c);

            MaxHp = Rudy.Instance.ReadInt(new UIntPtr(Entry) + 0xF8);


            ushort  monsterType = Rudy.Instance.ReadUShort(new UIntPtr(Entry)+ 0x368);
            Info = DataBase.GameDataBase.MonsterDatabase.FirstOrDefault(x => x.ID == monsterType);
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
        public override string ToString()
        {
            if (Info != null)
                return $"[{(ID).ToString("X")}] {((long)ObjectPointer).ToString("X")}  {Hp}/{MaxHp} {Info.Name}";

            return $"[{(ID).ToString("X")}] {((long)ObjectPointer).ToString("X")} Unknow Object";
        }
        public NPCModel ToWSObject()
        {
            return new NPCModel
            {
                Hp = Hp,
                MaxHp = MaxHp,
                X = X,
                Y = Y,
                Z = Z,
                Name = ToString()
            };
        }
    }
}