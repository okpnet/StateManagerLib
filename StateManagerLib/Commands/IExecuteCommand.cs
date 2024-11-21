using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Commands
{
    /// <summary>
    /// 実行コマンドインターフェイス
    /// </summary>
    public interface IExecuteCommand
    {
        /// <summary>
        /// オーナー
        /// </summary>
        IStateBase Owner { get; }
        /// <summary>
        /// セットされたインスタンス
        /// </summary>
        object? Value { get; }
        /// <summary>
        /// 戻す
        /// </summary>
        /// <returns></returns>
        bool Execute();
    }
}
