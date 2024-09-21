using CodeInject.Actors;
using CodeInject.Party;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CodeInject.MemoryTools
{
    public unsafe class DataFetcher
    {
        public delegate Int64 GetItemAdr(long arg1, int index);
        public delegate long GetInventoryItemDetailsAdr(long cItemAddr);
        public delegate long GetPartyMemberDetailsAdr(long arg0,short npcID);
        public GetInventoryItemDetailsAdr getInventoryItemDetailsFunc;
        public GetPartyMemberDetailsAdr getPartyMemberDetailsFunc;

        public long BaseAddres;
        public long GameBaseAddres;

        private long PlayerAddres;
        private long[] Addreses = new long[3]; 

        public DataFetcher()
        {
            Init();
        }


        public override string ToString()
        {
            return $"BaseAddres:{BaseAddres.ToString()};GameBaseAddres:{GameBaseAddres.ToString()};PlayerAddres:{PlayerAddres};YouCanEnter:YouCanEnter;";
        }
        private void Init()
        {
           Process _proc = Process.GetCurrentProcess();

           BaseAddres = _proc.MainModule.BaseAddress.ToInt64();
           GameBaseAddres = MemoryTools.GetVariableAddres("83 f8 07 0f 8f ?? ?? ?? ?? 48 63 0f 48 8b 05 ?? ?? ?? ??").ToInt64(); //UOB#U6
           getInventoryItemDetailsFunc = (GetInventoryItemDetailsAdr)Marshal.GetDelegateForFunctionPointer((IntPtr)MemoryTools.GetFunctionAddress("48 89 5c 24 08 57 48 83 ec ?? 48 8b f9 48 8d 59 50 e8 ?? ?? ?? ?? 83 f8 ?? 75 ?? 48 8b 0d ?? ?? ?? ??"), typeof(GetInventoryItemDetailsAdr)); //MSG#INV8
           PlayerAddres = MemoryTools.GetVariableAddres("74 ?? b0 ?? EB ?? 32 c0 45 84 f6 75 ?? 84 c0 75 ?? 48 8b 0d ?? ?? ?? ??").ToInt64();
        }
        public List<InvItem> GetAllItemsFromInventory(List<InvItem> currentList)
        {
            if (currentList == null) currentList = new List<InvItem>();

            List<IntPtr> itemsAddrs = GameHackFunc.Game.ClientData.getInventoryItems();
            List<InvItem> invDescriptions = new List<InvItem>();

            foreach (IntPtr item in itemsAddrs)
            {
                if (item.ToInt64() != 0x0)
                {
                    InvItem inv = new InvItem((long*)GameHackFunc.Game.ClientData.GetInventoryItemDetails((item.ToInt64())), (long*)item.ToInt64());
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


            List<IntPtr> itemsAddrs = GameHackFunc.Game.ClientData.getInventoryItems();
            List<InvItem> invDescriptions = new List<InvItem>();

            foreach (IntPtr item in itemsAddrs)
            {
                if (item.ToInt64() != 0x0)
                {
                    InvItem inv = new InvItem((long*)GameHackFunc.Game.ClientData.GetInventoryItemDetails((item.ToInt64())), (long*)item.ToInt64());
                    invDescriptions.Add(inv);
                }
            }

            //ADD NEW
            foreach (InvItem item in invDescriptions)
            {
                if (*item.ItemType == 0xA && !currentList.Any(x => (long)x.ObjectPointer == (long)item.ObjectPointer))
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


            return new NPC(&addr);
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

        public IObject GetObject(int id)
        {
            long* wskObj = (long*)((*(long*)(GameBaseAddres)) + (id * 8) + 0x22088+0x08); //OBS#N3
            long ObjectTypeFuncTable = *(long*)*wskObj;

            //0x1059060
            if (GameHackFunc.Game.ClientData.BaseAddres + 0x11C45C0 == ObjectTypeFuncTable)
                    return new NPC(wskObj);
            //0x105AB00
            if (GameHackFunc.Game.ClientData.BaseAddres + 0x11C6F98 == ObjectTypeFuncTable) // player avatar
                   return new Player(wskObj);
            //0x1057130
            if (GameHackFunc.Game.ClientData.BaseAddres + 0x11C4D58 == ObjectTypeFuncTable) // mobs and npcs
                return new NPC(wskObj);

            return new OtherPlayer(wskObj);
        }
        public Player GetPlayer()
        {
            long* wsp = (long*)(*(long*)(GameBaseAddres) + 0x22058+0x08);
            int * monsterIDList = (int*)*wsp;

            return (Player)GetObject(*monsterIDList);
        }
        public List<IObject> GetNPCs()
        {
            try
            {
                List<IObject> wholeNpcList = new List<IObject>();
                long* wsp = (long*)(*(long*)(GameBaseAddres) + 0x22058+0x08);//OBS#S3  teraz to ma przesunięcie o 0x8
                int* monsterIDList = (int*)*wsp;
                int* count = (int*)(*(long*)(GameBaseAddres) + 0x2A090);//OBS#S4

                for (int i = 0; i < *count; i++)
                {
                    IObject obj = GetObject(*monsterIDList);
                    if (obj != null)
                    {
                        wholeNpcList.Add(GetObject(*monsterIDList));
                    }
                    monsterIDList++;
                }

                var sortedList = wholeNpcList.OrderBy(x => x.CalcDistance(wholeNpcList[0]));
                return sortedList.ToList();
            }
            catch(Exception e)
            {
                return new List<IObject>();
            }
      
        }
        public long GetInventoryItemDetails(long cItemAddres)
        {
            return getInventoryItemDetailsFunc(cItemAddres);
        }
        public List<IntPtr> getInventoryItems()
        {
            List<IntPtr> inventorySlotAddrs = new List<IntPtr>();

            long* startList = (long*)(*(long*)((long)GameHackFunc.Game.ClientData.GetPlayer().ObjectPointer + 0x6908 + 0x20));//MSG#INV4



            for (long itemIndex = 0; itemIndex < 139; itemIndex++)
            {
                long slotAddres = *startList;
                inventorySlotAddrs.Add(new IntPtr(slotAddres));
                startList++;
            }
            return inventorySlotAddrs;
        }
        public List<IntPtr> getInventorySlots(int page,long dialogBoxAdr)
        {

            List<IntPtr> inventorySlotAddrs = new List<IntPtr>();
            //movsxd rax, dword ptr [rdi+000001B0]
            int itemIndexStart = page * 0x1e; //0x1e max page Size

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

            long* RDI = (long*)*(long*)(GameBaseAddres);
            long * RBX = (long*)(*(long*)(GameBaseAddres) + 0x2a0c0);

            RBX = (long*)*RBX; 

            while ((long)RBX != 0x0)
            {
                long* itemAddr = (long*)*(long*)((long)RDI + ((*(short*)RBX) * 8) + 0x22088);
                Item nearItem = new Item(itemAddr);
                itemList.Add(nearItem);
                RBX = (long*)*((long*)((long)RBX+0x8));
            }

            return itemList;
        }
    }
}
