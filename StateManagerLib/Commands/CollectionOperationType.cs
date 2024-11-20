using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Commands
{
    /// <summary>
    /// コレクションの操作種類
    /// </summary>
    public enum CollectionOperationType : byte
    {
        CollectionAdd = 0x1,
        CollectionRemove = 0x2,
        CollectionReset=0x0,
    }
}
