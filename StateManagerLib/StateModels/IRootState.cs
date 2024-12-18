﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// ルートの状態インターフェイス
    /// </summary>
    public interface IRootState:IStateBase
    {
        /// <summary>
        /// コマンドのスタック
        /// </summary>
        ICommandStack CommandStack { get; }
        /// <summary>
        /// ルートオブジェクト
        /// </summary>
        object Value { get; }
    }
}
