using Winebotv2.Actors;
using Winebotv2.MemoryTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp.Server;
using WebSocketSharp;
using Winebotv2.WebServ.Models;
using Winebotv2.PickupFilters;
using Winebotv2.WebServ.Models.PickUpFilter;
using Winebotv2.BotStates;

namespace Winebotv2
{
    public unsafe class WebSocketServices
    {
        public class MyWebSocketService : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Send($"{Player.GetPlayer.ToWSObject()}");
            }

        }

        public class NPCService : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                List<IObject> list = NPC.GetNPCsList();

                List<object> toSerialzie = new List<object>();
                NPC last = null;
                foreach (IObject npc in list)
                {
                    last = (NPC)npc;
                    toSerialzie.Add(((NPC)npc).ToWSObject());
                }

                Send($"{JsonConvert.SerializeObject(toSerialzie)}");


                if (cBot.BotContext.GetState<HuntState>("HUNT").HuntInstance.Target != null)
                {
                    Send(JsonConvert.SerializeObject(
                        new TargetInfoModel()
                        {
                            AttackedNPC = ((NPC)cBot.BotContext.GetState<HuntState>("HUNT").HuntInstance.Target).ToWSObject()
                        }));
                }
            }
        }


        public class SkillService : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                if (e.Data.Contains("setSkills"))
                {
                    SetSkillsModel newSkillSet = JsonConvert.DeserializeObject<SetSkillsModel>(e.Data);

                    cBot.BotContext.GetState<HuntState>("HUNT").HuntInstance.BotSkills.RemoveAll(x => 1 == 1);

                    foreach (int skillId in newSkillSet.setSkills)
                    {
                        cBot.BotContext.GetState<HuntState>("HUNT").HuntInstance.AddSkill(Skills.GetSkillByID(skillId),SkillTypes.Unknow);
                    }
                }
                else if (e.Data.Contains("GetSkills"))
                {
                    PlayerSkillModel skillsList = new PlayerSkillModel();
                    foreach (Skills singleSkill in Player.GetPlayer.GetSkillsList())
                    {
                        if (!(cBot.BotContext.GetState<HuntState>("HUNT")).HuntInstance.BotSkills.Any(x => x.skillInfo.ID == singleSkill.skillInfo.ID))
                            skillsList.UnUsedSkillList.Add(singleSkill.ToWSObject());

                        if ((cBot.BotContext.GetState<HuntState>("HUNT")).HuntInstance.BotSkills.Any(x => x.skillInfo.ID == singleSkill.skillInfo.ID))
                            skillsList.SkillInUseList.Add(singleSkill.ToWSObject());
                    }

                    Send($"{JsonConvert.SerializeObject(skillsList)}");
                }
            }
        }


        public class PickUpFilterService : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                if (e.Data.Contains("GetFilter"))
                {
                    var pickUpFilter = new SimpleFilterModel()
                    {
                        Name = "Simple",
                        Filter = ((QuickFilter)cBot.BotContext.Filter).pickTypeList
                    };

                    Send($"{JsonConvert.SerializeObject((object)pickUpFilter)}");
                }
            }
        }


        public class AutoPotionService : WebSocketBehavior
        {
            protected override void OnOpen()
            {
                List<IntPtr> items = new List<IntPtr>();//= GameHackFunc.Game.ClientData.getInventoryItems();

                List<ItemModel> ItemToSend = new List<ItemModel>();

                foreach (IntPtr item in items)
                {
                    if (item.ToInt64() != 0x0)
                    {
                     //   InvItem inv = new InvItem((long*)GameHackFunc.Game.ClientData.GetInventoryItemDetails(((long)item.ToInt64())), (long)item.ToInt64());

                     //   if (*inv.ItemType == 0xA)
                    //    {
                     //       ItemToSend.Add(inv.ToWSObject());
                    //    }
                    }
                }


   
                base.OnOpen();
            }
            protected override void OnMessage(MessageEventArgs e)
            {
                if (e.Data.Contains("GetAutoPotionSettings"))
                {
                   
                }
                if (e.Data.Contains("SetPotions"))
                {
                    dynamic setPotion = JsonConvert.DeserializeObject<dynamic>(e.Data);
                    InvItem[] items = Player.GetPlayer.GetConsumableItemsFromInventory(new List<InvItem>()).ToArray();

   
                }
            }
        }
    }
}
