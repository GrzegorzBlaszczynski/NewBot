using Winebotv2.Actors;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;



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
            /*BaseNetworkClass = MemoryTools.GetVariableAddres("48 8B 47 28 48 8D 4F 28 FF 90 D8 01 00 00 48 8B 0D ?? ?? ?? ??").ToInt64();//2023.10.03

            UseItemFunc = (UseItemAction)Marshal.GetDelegateForFunctionPointer((IntPtr)MemoryTools.GetFunctionAddress("40 53 48 83 ec 20 48 83 79 30 00 48 8b d9"), typeof(UseItemAction)); //MSG#INV5 

            AttackWithSkillFunc = (AttackWithSkillAction)Marshal.GetDelegateForFunctionPointer(MemoryTools.GetCallAddress("4c 8d 44 24 20 8b d0 e8 ?? ?? ?? ??"), typeof(AttackWithSkillAction));

            NormalAttackFunc = (NormalAttackAction)Marshal.GetDelegateForFunctionPointer(MemoryTools.GetCallAddress("48 8b cf e8 ?? ?? ?? ?? 84 c0 0f 84 ?? ?? ?? ?? 40 84 f6 0f 84 ?? ?? ?? ?? 48 8b 0d ?? ?? ?? ?? 8b d3 48 81 c1 ?? ?? ?? ?? e8 ?? ?? ?? ??"), typeof(NormalAttackAction));

            MoveToPointFunc = (MoveToAction)Marshal.GetDelegateForFunctionPointer(MemoryTools.GetCallAddress("48 8b cf e8 ?? ?? ?? ?? 84 c0 ?? ?? ?? ?? ?? ?? 48 8b 0d ?? ?? ?? ?? 4c 8b c6 48 81 c1 ?? ?? ?? ?? 33 d2 e8 ?? ?? ?? ??"), typeof(MoveToAction));

            PickUpFunc = (PickUpAction)Marshal.GetDelegateForFunctionPointer(MemoryTools.GetCallAddress("48 85 f6 74 ?? 48 8b 06 48 8b ce 48 8b 1d ?? ?? ?? ?? ff 50 ?? 0f bf 56 ?? 48 8d 8b ?? ?? ?? ?? 4c 8b c0 e8 ?? ?? ?? ??"), typeof(PickUpAction)); //MSG#INV4*/
        }


        public void Logger(string text, int chatType = 5)
        {
           
        }

        public void TalkToNPC(ushort ID)
        {
            long* rcxAddr = (long*)BaseAddres + 0x16DAF00;
            TalkToNPCFunc(*rcxAddr, ID);
        }

        public void SendPacket(byte* packet)
        {
            long networkStructAddr = *((long*)(BaseAddres + 0x1526a00)) + 0x16b8 + 0x1e0;
            SendPacketFunc(networkStructAddr, packet);
        }


        public void PickUp(Item item)
        {
            LuigiPipe.Instance.SendProcedure($"PICKUP;{item.ObjectPointer};{item.ID}");
        }


        public void CastSpell(IObject target, int skillIndex)
        {
            LuigiPipe.Instance.SendProcedure($"CASTSPELLONTARGET;{target.ID};{skillIndex}");
        }


        public void MoveToPoint(Vector2 position)
        {
            Logger($"Go to: {position.ToString()}");
            float[] position2float = new float[]
            {
                position.X*100, position.Y*100,0
            };
            fixed (float* p = position2float)
            {
                MoveToPointFunc((*(long*)BaseNetworkClass) + 0x16b8, 0, p);
            }
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
