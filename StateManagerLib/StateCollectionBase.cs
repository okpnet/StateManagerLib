using StateManagerLib.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StateManagerLib
{
    public abstract class StateCollectionBase : ICommandStack,IDisposable
    {
        protected Stack<IExecuteCommand> prevCommands = new();

        public void Dispose()
        {
            prevCommands.Clear();
            GC.SuppressFinalize(this);
        }

        public void PushCommandOnStack(IExecuteCommand command)
        {
            prevCommands.Push(command);
        }
    }
}
