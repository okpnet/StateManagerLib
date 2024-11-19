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
        /// セットしたプロパティのパス
        /// </summary>
        string PropertyPath { get; }
        /// <summary>
        /// セットされるまえの値
        /// </summary>
        object? BeforeValue { get; }
        /// <summary>
        /// セットされた後の値
        /// </summary>
        object? AfterValue { get; }
    }
}
