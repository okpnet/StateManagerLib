using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.StateModels
{
    /// <summary>
    /// プロパティの状態インターフェイス
    /// </summary>
    public interface IPropertyState:IStateBase
    {
        /// <summary>
        /// 親の状態インターフェイス
        /// </summary>
        IStateBase Parent { get; }
        /// <summary>
        /// プロパティの名前
        /// </summary>
        string Name { get; }
        /// <summary>
        /// ルートからのこのプロパティまでのパス
        /// </summary>
        string[] Paths { get; }
        /// <summary>
        /// 監視イベント追加
        /// </summary>
        /// <param name="value"></param>
        void AddPropertyChangeObserver(object value);
    }
}
