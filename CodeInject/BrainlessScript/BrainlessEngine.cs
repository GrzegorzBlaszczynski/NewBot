using CodeInject.BrainlessScript.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInject.BrainlessScript
{
    internal class BrainlessEngine
    {
        List<ICommand> commands = new List<ICommand>();
        int commandIndex = 0;
        ICommand currentCommand;
        public bool IsFinished= true;

        public BrainlessEngine() 
        { 
        }

        public void LoadInstructions(List<ICommand> commands)
        {
            this.commands = commands;
            commandIndex = 0;
            IsFinished = false;
            currentCommand = this.commands[commandIndex];
        }

        public void Play()
        {
            if (!IsFinished)
               currentCommand.Execute();
        }

        public void Update()
        {
            if (IsFinished) return;


            Command comandInfo = (Command)currentCommand;

            ((ICommandUpdater)comandInfo).Update();


            if (comandInfo.IsFinished)
            {
                commandIndex++;

                if (commandIndex < commands.Count())
                {
                    currentCommand = commands[commandIndex];
                    currentCommand.Execute();
                }
                else
                {
                    IsFinished = true;
                }
            }
        }
    }
}
