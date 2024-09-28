using Winebotv2.Actors;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;



namespace Winebotv2.MemoryTools
{
    public unsafe class GameActions
    {
        private delegate Int64 AttackWithSkillAction(int skill, int enemy, float* arg0);
        private delegate Int64 PickUpAction(long networkClass, int itemIndex, long playerObjectAdr);
        private delegate void UseIcoItemAction(long* IcoAdr);
        private delegate void UseQuickAction(long cQuickBarAddr, int key);
        private delegate void UseItemAction(long itemAdr);
        private delegate void NormalAttackAction(long networkClass, int enemy);
        private delegate void MoveToAction(long networkClass, int unknow0,float* destinationPoint);
        private delegate void TalkWithNPC(long arg0, ushort npcIndex);

        private delegate void PacketSendDelegate(long arg0, byte* packet);


        public delegate void Log(long staticAddr, string stringPointer, int cType, int color); 

        private UseIcoItemAction UseIcpItemFunc;
        private UseItemAction UseItemFunc;
        private PickUpAction PickUpFunc;
        private AttackWithSkillAction AttackWithSkillFunc;
        private NormalAttackAction NormalAttackFunc;
        private UseQuickAction QuickActionFunc;
        private MoveToAction MoveToPointFunc;
        private TalkWithNPC TalkToNPCFunc;
        private PacketSendDelegate SendPacketFunc;

        public Log LoggerFunc;
        private long BaseAddres;
        private long BaseNetworkClass;
        private long BaseOfDialogBoxes;

        private long ChatBaseAddres;

        public GameActions()
        {
            Init();
        }
        public void Init()
        {
            BaseAddres = Process.GetCurrentProcess().MainModule.BaseAddress.ToInt64();
        }


        public void Logger(string text, int chatType = 5)
        {
           
        }

        public void RepairItemWithNPC(int npcGameId, int networkItemID)
        {
            LuigiPipe.Instance.SendFunction($"RAPAIRWITHNPC;{npcGameId};{networkItemID}");
        }

        public void PickUp(Item item)
        {
            LuigiPipe.Instance.SendProcedure($"PICKUP;{item.ObjectPointer};{item.ID}");
        }

        public void RepairItem(InvItem RepairingItem, InvItem Item2Repair)
        {
            LuigiPipe.Instance.SendProcedure($"REPAIR;{RepairingItem.NetworkID};{Item2Repair.NetworkID}");
        }

        public string ConfirmBuyingStack(uint unknowArg = 0xFFFFFFFF)
        {
           return LuigiPipe.Instance.SendFunction($"CONFIRMBUY");
        }

        public string PutItemToBuy(int slot, int count)
        {
           return LuigiPipe.Instance.SendFunction($"ADDTOSTACK;{slot};{count}");
        }

        public string OpenShop(ushort npcId)
        {
            return LuigiPipe.Instance.SendFunction($"OPENSHOP;{npcId}");
        }


        public void CastSpell(IObject target, int skillIndex)
        {
            LuigiPipe.Instance.SendProcedure($"CASTSPELLONTARGET;{target.ID};{skillIndex}");
        }


        public void MoveToPoint(Vector2 position)
        {
            LuigiPipe.Instance.SendProcedure($"MOVETO;{position.X};{position.Y}");
        }

        public void CastSpell(int skillIndex)
        {
            LuigiPipe.Instance.SendProcedure($"CASTBUFF;{skillIndex}");
        }

        public void Attack(int targedID)
        {
            // NormalAttackFunc((*(long*)(BaseNetworkClass) + 0x16b8), targedID);

            LuigiPipe.Instance.SendProcedure($"ATTACKTARGET;{targedID}");
        }


        public void QuickBarExecute(int key)
        {
            QuickActionFunc(*(long*)(BaseAddres + 0x112CC20), key);
        }


        public void ItemUse(long itemAddr)
        {
            //UseItemFunc(itemAddr);
            LuigiPipe.Instance.SendProcedure($"USEITEM;{itemAddr}");
        }


        public void UseItemByIco(long* iconAdr)
        {
            UseIcpItemFunc = (UseIcoItemAction)Marshal.GetDelegateForFunctionPointer(new IntPtr(*(long*)(*iconAdr + 0x18)), typeof(UseIcoItemAction));
            UseIcpItemFunc(iconAdr);
        }
    }
}
