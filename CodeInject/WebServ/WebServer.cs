using Winebotv2.Actors;
using Winebotv2.BotStates;
using Winebotv2.MemoryTools;
using Winebotv2.WebServ.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebSocketSharp.Server;
using static Winebotv2.WebSocketServices;

namespace Winebotv2
{
    internal class WebServer
    {
        WebSocketServer server;

        public void SetupWebSocketServer(int port = 2458)
        {
            server = new WebSocketServer($"ws://localhost:{port}");

            server.AddWebSocketService<MyWebSocketService>("/CharacterInfo");
            server.AddWebSocketService<AutoPotionService>("/AutoPotion");
            server.AddWebSocketService<NPCService>("/NpcList");
            server.AddWebSocketService<SkillService>("/SkillList");
            server.AddWebSocketService<PickUpFilterService>("/Filter");

            server.Start();
        }

        public void SendPlayerInformation()
        {
            string characterJson = Player.GetPlayer.ToWSObject();
            foreach (var session in server.WebSocketServices["/CharacterInfo"].Sessions.Sessions)
            {
                server.WebSocketServices["/CharacterInfo"].Sessions.SendTo(characterJson, session.ID);
            }
        }

        public void SendNPCsInformation()
        {
            List<IObject> list = NPC.GetNPCsList();

            List<object> toSerialzie = new List<object>();
            NPC last = null;
            foreach (IObject npc in list)
            {
                last = (NPC)npc;
                toSerialzie.Add(((NPC)npc).ToWSObject());
            }
            string npcListJson = JsonConvert.SerializeObject(toSerialzie);


            foreach (var session in server.WebSocketServices["/NpcList"].Sessions.Sessions)
            {
                server.WebSocketServices["/NpcList"].Sessions.SendTo(npcListJson, session.ID);

                /*if (WineBot.WineBot.Instance.BotContext.GetState<HuntState>("HUNT").HuntInstance.Target != null)
                {
                    server.WebSocketServices["/NpcList"].Sessions.SendTo(JsonConvert.SerializeObject(new TargetInfoModel()
                    {
                        AttackedNPC = ((NPC)WineBot.WineBot.Instance.BotContext.GetState<HuntState>("HUNT").HuntInstance.Target).ToWSObject()
                    }), session.ID);
                }*/
            }
        }

    }
}
