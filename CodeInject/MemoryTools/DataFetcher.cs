using Winebotv2.Actors;
using Winebotv2.Party;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Winebotv2.MemoryTools
{
    public unsafe class DataFetcher
    {
        public delegate Int64 GetItemAdr(long arg1, int index);
        public delegate long GetInventoryItemDetailsAdr(long cItemAddr);
        public delegate long GetPartyMemberDetailsAdr(long arg0,short npcID);
        private GetInventoryItemDetailsAdr getInventoryItemDetailsFunc;
        private GetPartyMemberDetailsAdr getPartyMemberDetailsFunc;

        public long BaseAddres;
        public long GameBaseAddres;

        public long PlayerAddres;
        public long[] Addreses = new long[3]; 

        public DataFetcher()
        {
            Init();

        }

        public Player GetPlayerFromStaticADDr
        {

            get
            {
                return new Player(Rudy.Instance.ReadULong(new UIntPtr((ulong)BaseAddres + 0x17BE6F0)));
            }
        }


        private void Init()
        {
           Process _proc = Process.GetCurrentProcess();

     
        }
        public List<InvItem> GetAllItemsFromInventory(List<InvItem> currentList)
        {
            if (currentList == null) currentList = new List<InvItem>();

            List<UIntPtr> itemsAddrs = GameHackFunc.Game.ClientData.getInventoryItems();
            List<InvItem> invDescriptions = new List<InvItem>();

            foreach (UIntPtr item in itemsAddrs)
            {
                if (item.ToUInt64() != 0x0)
                {
                    InvItem inv = new InvItem((long)GameHackFunc.Game.ClientData.GetInventoryItemDetails(item.ToUInt64()), (long)item.ToUInt64());
                    invDescriptions.Add(inv);
                }
            }

            //ADD NEW
            foreach (InvItem item in invDescriptions)
            {
                if (!currentList.Any(x => (long)x.ObjectPointer == (long)item.ObjectPointer))
                {
                    currentList.Add(item);
                }
            }
            //REMOVE OLD
            currentList.RemoveAll(a => !invDescriptions.Any(b => (long)b.ObjectPointer == (long)a.ObjectPointer));
            return currentList;
        }

        public List<InvItem> GetConsumableItemsFromInventory(List<InvItem> currentList)
        {
            if (currentList==null) currentList = new List<InvItem>();


            List<UIntPtr> itemsAddrs = GameHackFunc.Game.ClientData.getInventoryItems();
            List<InvItem> invDescriptions = new List<InvItem>();

            foreach (UIntPtr item in itemsAddrs)
            {
                if (item.ToUInt64() != 0x0)
                {
                    InvItem inv = new InvItem((long)GameHackFunc.Game.ClientData.GetInventoryItemDetails((item.ToUInt64())), (long)item.ToUInt64());
                    invDescriptions.Add(inv);
                }
            }

            //ADD NEW
            foreach (InvItem item in invDescriptions)
            {
                //if (item.ItemType == 0xA && !currentList.Any(x => (long)x.ObjectPointer == (long)item.ObjectPointer))
                if (!currentList.Any(x => (long)x.ObjectPointer == (long)item.ObjectPointer))
                {
                    currentList.Add(item);
                }
            }
            //REMOVE OLD
            currentList.RemoveAll(a => !invDescriptions.Any(b => (long)b.ObjectPointer == (long)a.ObjectPointer));
            return currentList;
        }
        public IObject GetPartyMemberDetails(PartyMember member)
        {
            int partyMemberId = *(int*)(member.MemberAddres + 0x08);

            long rcx = *(long*)(BaseAddres + 0x1217268);
            short edx = *(short*)(rcx + partyMemberId * 2 + 0x0c);

            long addr = getPartyMemberDetailsFunc(rcx, edx);


            return new NPC((ulong)addr);
        }
        public List<PartyMember> GetPartyMembersList()
        {
            long* PartyMemberDataAddres = (long*)*(long*)MemoryTools.GetInt64(GameHackFunc.Game.ClientData.BaseAddres + 0x0121A130, new short[] { 0x0, 0x10, 0x08 });

            int partyMemberCount = *(int*)(GameHackFunc.Game.ClientData.BaseAddres + 0x121A170);

            List<PartyMember> PartyMemberList = new List<PartyMember>();

            for (int i = 0; i < partyMemberCount; i++)
            {
                long* currentMember = (long*)*PartyMemberDataAddres; //selecting member

                PartyMember member = new PartyMember()
                {
                    MemberAddres = (long)currentMember,
                    MemberName = Marshal.PtrToStringAnsi(new IntPtr((long)currentMember + 0x10)),
                };


                member.PartyMemberObject = GetPartyMemberDetails(member);

                if((long)member.PartyMemberObject.ObjectPointer!=0x0)
                PartyMemberList.Add(member);


                PartyMemberDataAddres++; //move to next member
            }

            return PartyMemberList;
        }
        public List<Skills> GetPlayerSkills()
        {
            List<Skills> skillList = new List<Skills>();

            ulong adrPtr1 = Rudy.Instance.ReadULong(new UIntPtr((ulong)PlayerAddres)); //2023.10.04

           
            int index = 0;


            int skillId = Rudy.Instance.ReadUShort(new UIntPtr(adrPtr1) + 0x50 + 0xb68);

            while (skillId != 0)//OBS#S2
            {
                SkillInfo skill = DataBase.GameDataBase.SkillDatabase.FirstOrDefault(x => x.ID == skillId);
                if (skill == null)
                {
                    skill = new SkillInfo()
                    {
                        ID = skillId,
                        Name = "Unknow"
                    };
                }
                skillList.Add(new Skills(skill, SkillTypes.Unknow) { SkillIndex = index});
                index++;
                skillId = Rudy.Instance.ReadUShort(new UIntPtr(adrPtr1) + 0x50 + 0xb68 + index * 2);
            }

            adrPtr1 = Rudy.Instance.ReadULong(new UIntPtr((ulong)PlayerAddres)); //2023.10.04
            ulong uniqueskillsAddr = adrPtr1+ 0x50 + 0xb68;
            uniqueskillsAddr += 0x1F8;

            index = 0x1F8;

            ushort uniqueSkillID = Rudy.Instance.ReadUShort(new UIntPtr(uniqueskillsAddr));

            while (uniqueSkillID != 0)
            {
                SkillInfo skill = DataBase.GameDataBase.SkillDatabase.FirstOrDefault(x => x.ID == uniqueSkillID);
                if (skill == null)
                {
                    skill = new SkillInfo()
                    {
                        ID = uniqueSkillID,
                        Name = "Unknow"
                    };
                }
                skillList.Add(new Skills(skill, SkillTypes.Unknow) { SkillIndex = index });
                index++;
                uniqueskillsAddr+=2;
                uniqueSkillID = Rudy.Instance.ReadUShort(new UIntPtr(uniqueskillsAddr));
            }

            // ushort* uniqueSkill = (ushort*)(*adrPtr1 + 0xFC * 2 + 0xBB8);
            /*
                index = 0xFC;

                while (*uniqueSkill!=0)
                {
                    SkillInfo skill = DataBase.GameDataBase.SkillDatabase.FirstOrDefault(x => x.ID == *uniqueSkill);
                    if (skill == null)
                    {
                        skill = new SkillInfo()
                        {
                            ID = *uniqueSkill,
                            Name = "Unknow"
                        };
                    }
                    skillList.Add(new Skills(skill, SkillTypes.Unknow) { SkillIndex = index });
                    index++;
                    uniqueSkill++;
                }
            */

            return skillList;
        }
        public IObject GetObject(int id)
        {
           // long* wskObj = (long*)((*(long*)(GameBaseAddres)) + (id * 8) + 0x22088+0x8); //OBS#N3
            ulong wskObj = (Rudy.Instance.ReadULong(new UIntPtr((ulong)GameBaseAddres)) + 0x22088 + 0x8 +((ulong)id*8));

            wskObj = Rudy.Instance.ReadULong(new UIntPtr(wskObj));

  

            ulong ObjectTypeFuncTable = Rudy.Instance.ReadULong(new UIntPtr(wskObj));


            if ((ulong)GameHackFunc.Game.ClientData.BaseAddres+ 0x11C6F98 == ObjectTypeFuncTable) // player avatar
                     return new Player(wskObj);
            //0x11C 4D58 11C 45C0
            if ((ulong)GameHackFunc.Game.ClientData.BaseAddres + 0x11C45C0 == ObjectTypeFuncTable) 
                  return new NPC(wskObj);
            if ((ulong)GameHackFunc.Game.ClientData.BaseAddres + 0x11C4D58 == ObjectTypeFuncTable) 
                return new NPC(wskObj);


            return new OtherPlayer(wskObj);
        }


        public Player GetPlayer()
        {
            // ulong wsp = (Rudy.Instance.ReadULong(new UIntPtr((ulong)GameBaseAddres)) + 0x22058 + 0x8);
            // ulong aadr = Rudy.Instance.ReadULong(new UIntPtr(wsp));
            return GetPlayerFromStaticADDr;// (Player)GetObject(Rudy.Instance.ReadInt(new UIntPtr(aadr)));
        }
        public List<IObject> GetNPCs()
        {

                List<IObject> wholeNpcList = new List<IObject>();

                ulong wsp = (Rudy.Instance.ReadULong(new UIntPtr((ulong)GameBaseAddres)) + 0x22058 + 0x8);
                ulong aadr = Rudy.Instance.ReadULong(new UIntPtr(wsp));



                int cout = Rudy.Instance.ReadInt(new UIntPtr(Rudy.Instance.ReadULong(new UIntPtr((ulong)GameBaseAddres)) + 0x2A090));

                for (int i = 0; i < cout; i++)
                {

                    int id = Rudy.Instance.ReadInt(new UIntPtr(aadr + (ulong)i*4));
       
                    IObject obj = GetObject(id);
                    if(obj != null)
                    wholeNpcList.Add(obj);

                }

                var sortedList = wholeNpcList.OrderBy(x => x.CalcDistance(wholeNpcList[0]));
                return sortedList.ToList();
     
  
      
        }

        public ulong GetInventoryItemDetails(ulong cItemAddres)
        {
            string result =LuigiPipe.Instance.SendFunction($"GETITEMDETAILS;{cItemAddres}");

            string[] separatedData = result.Split(';');

            return ulong.Parse(separatedData[1]);
        }



        public List<UIntPtr> getInventoryItems()
        {
            List<UIntPtr> inventorySlotAddrs = new List<UIntPtr>();


            long playerAddr = GetPlayer().ObjectPointer;//MSG#INV4

            ulong startList = Rudy.Instance.ReadULong(new UIntPtr( (ulong)(GetPlayer().ObjectPointer + 0x6968)));

            for (long itemIndex = 0; itemIndex < 139; itemIndex++)
            {
                ulong slotAddres = Rudy.Instance.ReadULong(new UIntPtr(startList));
                inventorySlotAddrs.Add(new UIntPtr(slotAddres));
                startList+=8;
            }
            return inventorySlotAddrs;
        }

        public List<IntPtr> getInventorySlots(int page,long dialogBoxAdr)
        {

            List<IntPtr> inventorySlotAddrs = new List<IntPtr>();

            int itemIndexStart = page * 0x1e; 

            long i = 0;
            for (long itemIndex = itemIndexStart; itemIndex < itemIndexStart + 0x1e; itemIndex++)
            {
                long slotAddres = (itemIndexStart + i) * 0x168;
                slotAddres += 0x2d78;
                slotAddres += dialogBoxAdr;
                Console.WriteLine($"{itemIndex.ToString("X")} {slotAddres.ToString("X")}");
                inventorySlotAddrs.Add(new IntPtr(slotAddres));
                i++;
            }
            return inventorySlotAddrs;
        }

        public List<IObject> GetItemsAroundPlayerV2()
        {
            List<IObject> itemList = new List<IObject>();

            Process _proc = Process.GetCurrentProcess();

            ulong RDI = Rudy.Instance.ReadULong(new UIntPtr((ulong)GameBaseAddres));
            ulong RBX = (Rudy.Instance.ReadULong(new UIntPtr((ulong)GameBaseAddres)) + 0x2a0c8);

            RBX = (Rudy.Instance.ReadULong(new UIntPtr(RBX))); 

            while ((long)RBX != 0x0)
            {
                ulong itemAddr = Rudy.Instance.ReadULong(new UIntPtr((RDI + (ulong)(Rudy.Instance.ReadUShort(new UIntPtr(RBX))) * 8) + 0x22090));
                Item nearItem = new Item(itemAddr);
                itemList.Add(nearItem);

                RBX = Rudy.Instance.ReadULong(new UIntPtr(RBX+0x8));
            }

            return itemList;
        }
    }
}
