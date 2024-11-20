using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StateManagerLib
{
    public interface IStateCollecion
    {
        /// <summary>
        /// プロパティを検索する深さ
        /// </summary>
        int FindPropertyNest { get; set; }

        void CommandRegister()
    }
}
