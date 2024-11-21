using StateManagerLib.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// 状態インスタンスインターフェイス
    /// </summary>
    public interface IStateBase
    {
        /// <summary>
        /// 変更回数
        /// </summary>
        int ChangeCount { get; }
        /// <summary>
        /// 状態に対して行った変更
        /// </summary>
        IEnumerable<IExecuteCommand> Commands { get; }
        /// <summary>
        /// コマンドのスタック
        /// </summary>
        ICommandStack CommandStack { get; }
    }
}
