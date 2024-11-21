using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StateManagerLib
{
    /// <summary>
    /// オブジェクトの元に戻す、進むを管理する
    /// </summary>
    public interface IStateCollecion:IList<object>
    {
        /// <summary>
        /// 戻す
        /// </summary>
        void Undo();
        /// <summary>
        /// 進む
        /// </summary>
        void Redo();
        /// <summary>
        /// valueを戻す
        /// </summary>
        /// <param name="value"></param>
        void Undo(object value);
        /// <summary>
        /// valueを進める
        /// </summary>
        /// <param name="value"></param>
        void Redo(object value);
    }
}
