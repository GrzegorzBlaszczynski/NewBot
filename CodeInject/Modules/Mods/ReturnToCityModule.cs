using Winebotv2.MemoryTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeInject.BrainlessScript.Commands;
using CodeInject.Modules.Mods.ReturningConditions;
using CodeInject.BrainlessScript;
using Winebotv2.Hunt;
using Winebotv2.BotStates;

namespace Winebotv2.Modules
{
    internal class ReturnToCityModule : IModule
    {
        public string Name { get; set; } = "WALKMODULE";

        List<ICommand> _commands;
        BotContext _context;
        BrainlessEngine _scriptEngine { get; set; } = new BrainlessEngine();
        List<ICondition> _conditions = new List<ICondition>();
        bool _process =false;
 

        public ReturnToCityModule(BotContext _context)
        {
            AddCoondition(new CharacterIsDeadCondition());
            this._context = _context;
        }


   

        public ReturnToCityModule(BotContext _context,List<ICommand> commands)
        {
            this._context = _context;
            LoadScript(commands);
        }

        public void AddCoondition(ICondition condition)
        {
           int isAlreadyExistIndex = _conditions.FindIndex(x=>x.GetType() == condition.GetType());

            if (isAlreadyExistIndex != -1)
            {
                _conditions[isAlreadyExistIndex] = condition;
            }
            else
            {
                _conditions.Add(condition);
            }
        }

        public void ForceRun()
        {
            _process = true;
        }

        public void LoadScript(List<ICommand> commands)
        {
            _commands = commands;
            _scriptEngine.LoadInstructions(_commands);
        }

        public unsafe void Update()
        {
            if (!_process)
            {
                foreach (ICondition coondition in _conditions.ToArray())
                {
                    coondition.Update();
                    if (coondition.IsFulfilled)
                    {
                        _process = true;
                        LoadScript(_commands);
                        _context.SetState("WALK");
                    }
                }
            }else
            {
                _scriptEngine.Play();
                _scriptEngine.Update();

                if(_scriptEngine.IsFinished)
                {
                    _process = false;
                    (_context.States["HUNT"]as HuntState).HuntInstance.ResetTarget();
                    _context.SetState("HUNT");
                }
            }
        }
    }
}
