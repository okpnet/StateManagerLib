using StateManagerLib.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Collections
{
    /// <summary>
    /// コマンドコレクション
    /// </summary>
    public interface ICommandCollection:ICollection<IExecuteCommand>
    {
    }
}
