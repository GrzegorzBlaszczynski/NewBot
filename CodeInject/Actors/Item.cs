using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Winebotv2.MemoryTools;

namespace Winebotv2.Actors
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



        uint networkID { get; set; }
        private void Init(ulong entry)
        {
            ObjectPointer = (long)entry;
            X = Rudy.Instance.ReadFloat(new UIntPtr(entry) + 0x10);
            Y = Rudy.Instance.ReadFloat(new UIntPtr(entry) + 0x14);
            Z =  Rudy.Instance.ReadFloat(new UIntPtr(entry) + 0x18);
            ID = Rudy.Instance.ReadUShort(new UIntPtr(entry) + 0x1c);
            ItemData = (short)Rudy.Instance.ReadUShort(new UIntPtr(entry) + 0x6c);
            ItemType = (short)Rudy.Instance.ReadUShort(new UIntPtr(entry) + 0x68);


            networkID = Rudy.Instance.ReadUInt(new UIntPtr((Rudy.Instance.ReadULong(new UIntPtr((ulong)GameHackFunc.Game.ClientData.BaseAddres + 0x17c0508))) + (ulong)(ID * 2) + 0x2000A));
        }

        public Item(ulong Entry)
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


        public void PickUp()
        {
            GameHackFunc.Game.Actions.PickUp(this);
        }
        public override string ToString()
        {
            IBasicInfo temp;


            switch (ItemType)
            {
                case 0x08:
                    {
                        temp = DataBase.GameDataBase.WeaponItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + " " + temp.Name : "Unknow")}";
                    }

                case 0x0A:
                    {
                        temp = DataBase.GameDataBase.UsableItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((UsableItemsInfo)temp).DisplayName : "Unknow")}";
                    }

                case 0x03:
                    {
                        temp = DataBase.GameDataBase.BodyItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? temp.Name : "Unknow")}";
                    }

                case 0x05:
                    {
                        temp = DataBase.GameDataBase.FootItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + temp.Name : "Unknow")}";
                    }

                case 0x04:
                    {
                        temp = DataBase.GameDataBase.ArmItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + temp.Name : "Unknow")}";
                    }
                case 0x07:
                    {
                        temp = DataBase.GameDataBase.AccesoriesItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + temp.Name : "Unknow")}";
                    }
                case 0x09:
                    {
                        temp = DataBase.GameDataBase.SheildItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + temp.Name : "Unknow")}";
                    }

                case 0x0B:
                    {
                        temp = DataBase.GameDataBase.GemItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + temp.Name : "Unknow")}";
                    }
                case 0x0C:
                    {
                        temp = DataBase.GameDataBase.MaterialItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") + temp.Name : "Unknow")}";
                    }
                case 0x0F:
                    {
                        temp = DataBase.GameDataBase.MaterialItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{networkID.ToString("X")}  {ID} {(temp != null ? ((long)ObjectPointer).ToString("X") +  temp.Name : "Unknow")}";
                    }

                default:
                    {
                        return $"Unknow type:{ID} {(ItemType).ToString("X")} id:{(ItemData).ToString("X")}";
                    }
            }
        }
    }
}
