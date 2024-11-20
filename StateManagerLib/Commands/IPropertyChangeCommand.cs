using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Commands
{
    /// <summary>
    ///プロパティのコマンドインターフェイス
    /// </summary>
    public interface IPropertyChangeCommand:IExecuteCommand
    {
        /// <summary>
        /// オーナー
        /// </summary>
        new IPropertyState Owner { get; }
        /// <summary>
        /// プロパティ名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// セットされたインスタンス
        /// </summary>
        object? Value { get; }
    }
}
