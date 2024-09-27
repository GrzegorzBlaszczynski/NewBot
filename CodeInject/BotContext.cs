using Winebotv2.Actors;
using Winebotv2.BotStates;
using Winebotv2.BotStates.States;
using Winebotv2.Hunt;
using Winebotv2.MemoryTools;
using Winebotv2.Modules;
using Winebotv2.PickupFilters;
using System.Collections.Generic;

namespace Winebotv2
{
    public class BotContext: ModuleConteiner
    {
        public IBotState CurrentBotState { get; set; }
        public Dictionary<string, IBotState> States { private set; get; } = new Dictionary<string, IBotState>();
        public IFilter Filter = new QuickFilter();


        public List<IObject> GetItemsNearby()
        {
            return Player.GetPlayer.GetItemsAroundPlayerFilteredList(Filter);
        }


        public BotContext()
        {
            StandbyState standBy = new StandbyState();
            States.Add("STANDBY", standBy);
            States.Add("PICK", new PickUpState());
            States.Add("HUNT",new HuntState(new DefaultHunt()));
            SetState("STANDBY");
        }

        public void Start(HuntState huntState)
        {
            ReplaceState("HUNT", huntState);
            SetState("HUNT");
        }

        public void Stop()
        {
            SetState("STANDBY");
        }

        public void SetState(string stateName)
        {
            if (States.ContainsKey(stateName) && CurrentBotState != States[stateName])
            {
                CurrentBotState = States[stateName];
                GameHackFunc.Game.Actions.Logger($"Change state: {stateName}");
            }
        }

        public void Update()
        {
            CurrentBotState.Work(this);
            base.ModuleExecute();
        }
 
        public void AddState(string key,IBotState state)
        {
            if (!States.ContainsKey(key))
            {
                States.Add(key, state);
            }
        }
        public void ReplaceState(string key, IBotState newState)
        {
            IBotState existState;
            if (States.TryGetValue(key, out existState))
            {
                States.Remove(key);
                States.Add(key, newState);
            }
            else
            {
                AddState(key, newState);
            }
        }

        public T GetState<T>(string key)
        {
            return (T)States[key];
        }
    }
}
