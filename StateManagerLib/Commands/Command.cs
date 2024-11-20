using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Commands
{
    public abstract class Command:IExecuteCommand
    {
        /// <summary>
        /// オーナー
        /// </summary>
        public IStateBase Owner { get; } = default!;
        /// <summary>
        /// 戻す
        /// </summary>
        /// <returns></returns>
        public abstract bool Execute(object refValue);

        protected Command(IStateBase owner)
        {
            Owner = owner;
        }
    }
}
