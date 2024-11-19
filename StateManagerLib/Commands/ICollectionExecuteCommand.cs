using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Commands
{
    public interface ICollectionExecuteCommand:IExecuteCommand
    {
        /// <summary>
        /// どんな操作がされたか
        /// </summary>
        CollectionOperationType Operation { get; }
        /// <summary>
        /// セットされるまえの値
        /// </summary>
        object? BeforeValue { get; }
    }
}
