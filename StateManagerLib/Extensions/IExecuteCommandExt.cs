using StateManagerLib.Commands;
using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Extensions
{
    public static class IExecuteCommandExt
    {
        public static void ToExecute(this IExecuteCommand command)
        {
            var owner=command.Owner as StateBase;
            if(owner is null)
            {
                throw new NullReferenceException($"IExecuteCommand has IStateBase is not Satatebase.");
            }
            owner.ExecuteTo(command);
        }
    }
}
