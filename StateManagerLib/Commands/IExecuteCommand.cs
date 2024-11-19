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
        /// 戻す
        /// </summary>
        /// <returns></returns>
        bool ToPrev(object value);
        /// <summary>
        /// 進む
        /// </summary>
        /// <returns></returns>
        bool ToNext(object value);
    }
}
