﻿using Winebotv2.MemoryTools;
using Winebotv2.WebServ.Models;
using System.Linq;

namespace Winebotv2
{
    public unsafe class InvItem
    {
        public long ObjectPointer { get; set; }
        public long CItemAddr { get; set; }
        public short ItemData { get; set; }
        public int NetworkID { get; set; }
        public int Dubaribility
        {
            get
            {
                return (int)((float)Rudy.Instance.ReadUShort(new System.UIntPtr((ulong)ObjectPointer) + 0x1A) / 1000) * MaxDubaribility;
            }
        }
        public int DurabilityProcentage
        {
            get
            {
                return (int)((float)Rudy.Instance.ReadUShort(new System.UIntPtr((ulong)ObjectPointer) + 0x1A) / 1000 * 100);
            }
        }
        public int MaxDubaribility { get; set; }

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
        public short ItemType { get; set; }

        public InvItem(long ItemDBAddr, long cItemAddr)
        {
            ObjectPointer = ItemDBAddr;
            CItemAddr = cItemAddr;
            NetworkID = Rudy.Instance.ReadInt(new System.UIntPtr((ulong)ObjectPointer + 0x48));

            MaxDubaribility = (short)Rudy.Instance.ReadUShort(new System.UIntPtr((ulong)ItemDBAddr + 0x18));

            ItemData = (short)Rudy.Instance.ReadUShort(new System.UIntPtr((ulong)ItemDBAddr + 0x0c));
            ItemType = (short)Rudy.Instance.ReadUShort(new System.UIntPtr((ulong)ItemDBAddr + 0x08));
            CItemAddr = cItemAddr;
        }

        public ItemModel ToWSObject()
        {
            return new ItemModel()
            {
                Id = (long)ObjectPointer,
                Name = ToString()
            };
        }


        public void UseItem()
        {
            GameHackFunc.Game.Actions.ItemUse((long)CItemAddr);
        }

        public override string ToString()
        {
            IBasicInfo temp;


            switch (ItemType)
            {
                case 0x08:
                    {
                        temp = DataBase.GameDataBase.WeaponItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID+" "+ " "+ temp.Name : "Unknow")}";
                    }

                case 0x0A:
                    {
                        temp = DataBase.GameDataBase.UsableItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $" {(temp != null ? temp.ID +  " " + ((UsableItemsInfo)temp).DisplayName : " Unknow")}";
                    }

                case 0x03:
                    {
                        temp = DataBase.GameDataBase.BodyItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID  + temp.Name : temp.ID  + " Unknow")}";
                    }

                case 0x05:
                    {
                        temp = DataBase.GameDataBase.FootItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID + " "  + temp.Name : "Unknow")}";
                    }

                case 0x04:
                    {
                        temp = DataBase.GameDataBase.ArmItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID + " "  + temp.Name : "Unknow")}";
                    }
                case 0x09:
                    {
                        temp = DataBase.GameDataBase.SheildItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID +" " + temp.Name : "Unknow")}";
                    }
                case 0x0B:
                    {
                        temp = DataBase.GameDataBase.GemItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID + " " + temp.Name : "Unknow")}";
                    }

                case 0x0C:
                    {
                        temp = DataBase.GameDataBase.MaterialItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID  + " " + temp.Name : "Unknow")}";
                    }

                case 0x02:
                    {
                        temp = DataBase.GameDataBase.HeadItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID + " " + temp.Name : "Unknow")}";
                    }
                case 0x07:
                    {
                        temp = DataBase.GameDataBase.AccesoriesItemsDatabase.FirstOrDefault(x => x.ID == ItemData);
                        return $"{(temp != null ? temp.ID + " "  + temp.Name : "Unknow")}";
                    }
                default:
                    {
                        return $"Unknow type:{(ItemType).ToString("X")} id:{(ItemData).ToString("X")}";
                    }
            }
        }
    }
}
