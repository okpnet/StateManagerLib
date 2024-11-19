using StateManagerLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    public interface IStateBase
    {
        /// <summary>
        /// 状態に対して行った変更
        /// </summary>
        ICommandCollection Commands { get; }
    }
}
